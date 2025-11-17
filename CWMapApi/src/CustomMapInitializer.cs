using System;
using System.Collections;
using CWMapApi.Api;
using CWMapApi.Patches;
using Photon.Pun;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace CWMapApi
{
    internal class CustomMapInitializer: MonoBehaviour
    {
        internal static GameObject diveBellPrefab;
        internal static Material worldMaterial;
        internal static GameObject world;
        private static CustomMap customMap;
        
        internal static CustomMapInitializer instance;

        internal MapStates States;


        private void Awake()
        {
            StartCoroutine(Initialize()); 
        }

        internal static IEnumerator InitializeMap(CustomMap map)
        {
            customMap = map;
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == map.GetSceneName());
            yield return new WaitUntil(() => SceneManager.GetActiveScene().IsValid());
            var initializer = new GameObject("MapInitializer");
            SceneManager.MoveGameObjectToScene(initializer, SceneManager.GetActiveScene());
            initializer.AddComponent<CustomMapInitializer>();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator Initialize()
        {
            instance = this;
            yield return PluginTools.WaitForSceneIsLoaded(gameObject.scene);
            yield return InitializeDiveBells();
            yield return InitializePatrolPoints();
            yield return InitializeNavMeshes();
            yield return customMap.Prefix();
            yield return SceneManager.LoadSceneAsync("FactoryScene",  LoadSceneMode.Additive);
            yield return Postprocessing();
            yield return customMap.Postfix();
            Destroy(gameObject);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator InitializeDiveBells()
        {
            GameObject diveBellParent = new GameObject("CustomDiveBells");
            GameObject customDiveBellSpawns = customMap.GetDiveBells();

            if (!customDiveBellSpawns)
            {
                MapApi.log.LogError("Cannot find dive bell spawns in custom map");
            } 
            
            for (int i = 0; i < customDiveBellSpawns.transform.childCount; i++)
            {
                GameObject customDiveBellSpawn = Instantiate(diveBellPrefab, diveBellParent.transform);
                customDiveBellSpawn.name = $"DiveBell ({i})";
                PluginTools.CopyTransform(customDiveBellSpawns.transform.GetChild(i), customDiveBellSpawn.transform);
                var photonView = customDiveBellSpawn.GetComponent<PhotonView>();
                photonView.ViewID = 225235412+i;
            }
            diveBellParent.AddComponent<DiveBellParent>();
            Destroy(customDiveBellSpawns);
            yield return null;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator InitializePatrolPoints()
        {
            try
            {
                world = customMap.GetLevel();
                world.AddComponent<Level>();
                if(PluginTools.FindObjectInScene(gameObject.scene, "CWMapApi_PatrolPoints") != null)
                    throw new Exception("Object CWMapApi_PatrolPoints is busy, please remove it from the scene, due to it needs for init custom map");
                GameObject patrolPoints = new GameObject("CWMapApi_PatrolPoints");
                patrolPoints.transform.SetParent(world.transform);
                GameObject customPatrolPoints = customMap.GetPatrolPoints();

                if (!customPatrolPoints)
                {
                    MapApi.log.LogError("Cannot find patrol points in custom map");
                }

                MapApi.log.LogInfo("Initializing patrol points 6");
                for (int i = 0; i < customPatrolPoints.transform.childCount; i++)
                {
                    GameObject customPatrolPoint = new GameObject($"PatrolPoint ({i})");
                    customPatrolPoint.transform.SetParent(patrolPoints.transform);
                    PluginTools.CopyTransform(customPatrolPoints.transform.GetChild(i), customPatrolPoint.transform);
                    var point = customPatrolPoint.AddComponent<PatrolPoint>();
                    point.group = PatrolPoint.PatrolGroup.Bear;
                } 

                Destroy(customPatrolPoints); 
            }
            catch (Exception e)
            {
                MapApi.log.LogError(e.Message);
            }
            yield return null;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator InitializeNavMeshes()
        {
            try
            {
                var navmeshWide = customMap.GetNavmeshWide().GetComponent<NavMeshSurface>();
                if (!navmeshWide)
                {
                    throw new Exception("Cannot find navmesh wide in custom map");
                }
                States.WideAgentTypeId = navmeshWide.agentTypeID;
                var navmesh = customMap.GetNavmesh().GetComponent<NavMeshSurface>();
                if (!navmesh)
                {
                    throw new Exception("Cannot find navmesh in custom map");
                }
                States.AgentTypeId = navmesh.agentTypeID;
            }
            catch (Exception e)
            {
                MapApi.log.LogError(e.Message);
            }
            yield return null;
        }

        private IEnumerator Postprocessing()
        {
            var randomTraps = PluginTools.FindObjectInScene(SceneManager.GetSceneByName("FactoryScene"), "RandomTraps");
            Destroy(randomTraps);
            yield return null;
        }
    }
}