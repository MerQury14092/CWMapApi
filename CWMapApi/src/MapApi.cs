using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using CWMapApi.Api;
using CWMapApi.Patches;
using HarmonyLib;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace CWMapApi
{
    [BepInPlugin("com.merqury.cw.map_api", "CW Map API", "1.0.0")]
    public class MapApi : BaseUnityPlugin
    {
        internal static ManualLogSource log;
        internal static PluginInfo PInfo;
        internal static bool ModInitialized;
        internal static List<CustomMap> CustomMaps;
        internal static bool EnableRandomMapPatch = true;
        internal static GameObject ParentGameObject;
        
        private void Awake()
        {
            log = Logger;
            PInfo = Info;
            
            Harmony.CreateAndPatchAll(typeof(MainMenuHandlerPatch));
            Harmony.CreateAndPatchAll(typeof(DiveBellParentPatch));
            Harmony.CreateAndPatchAll(typeof(LevelPatch));
            Harmony.CreateAndPatchAll(typeof(Bot_Nav_NavmeshPatch));
            Harmony.CreateAndPatchAll(typeof(PatrolPointPatch));
            Harmony.CreateAndPatchAll(typeof(DivingBellPatch));
            Harmony.CreateAndPatchAll(typeof(RoomStatsHolderPatch));

            CustomMaps = new List<CustomMap>();

            log.LogInfo($"CWMapApi Loaded!");
        }

        internal static void Loop()
        {
            foreach (var agent in Bot_Nav_NavmeshPatch.Agents)
            {
                try
                {
                    if(agent.agentTypeID != 0 || CustomMapInitializer.instance)
                        agent.agentTypeID = CustomMapInitializer.instance.States.WideAgentTypeId;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            
        }
        
        public static void RegisterCustomMap(CustomMap customMap)
        {
            var mapsCheck = (from map in CustomMaps
                where map.GetMapName() == customMap.GetMapName()
                select map).ToList();
            if (mapsCheck.Count > 0)
                throw new Exception($"Custom map \"{customMap.GetMapName()}\" is already registered!");
            CustomMaps.Add(customMap);
            log.LogInfo($"Map \"{customMap.GetMapName()}\" has been registered!");
        }

        public static void EnableMapPoolPatch(bool enable)
        {
            EnableRandomMapPatch = enable;
        } 

        public static void UnregisterCustomMap(CustomMap customMap)
        {
            var mapsCheck = (from map in CustomMaps
                where map.GetMapName() == customMap.GetMapName()
                select map).ToList();
            if (mapsCheck.Count > 0)
                throw new Exception($"Custom map \"{customMap.GetMapName()}\" doesn't exist!");
            CustomMaps.RemoveAll(map => map.GetMapName() == customMap.GetMapName());
            log.LogInfo($"Map \"{customMap.GetMapName()}\" has been unregistered!");
        }

        public static IReadOnlyList<CustomMap> GetRegisteredCustomMaps()
        {
            IReadOnlyList<CustomMap> maps = CustomMaps;
            return maps;
        }
        
        internal static IEnumerator InitializeMod()
        {
            var loadingScene = SceneManager.LoadSceneAsync("FactoryScene", LoadSceneMode.Additive);
            yield return loadingScene;
            foreach (var rootGameObject in SceneManager.GetSceneByName("FactoryScene").GetRootGameObjects())
            {
                rootGameObject.SetActive(false);
            }

            ParentGameObject = new GameObject("CWMapApi");
            DontDestroyOnLoad(ParentGameObject);
            
            CWMapApiManager.Initialize();
            var templates = new GameObject("Templates");
            templates.transform.SetParent(ParentGameObject.transform);
            var spawns = PluginTools.FindObjectInScene(SceneManager.GetSceneByName("FactoryScene"), "Spawns");
            var divebell = spawns.transform.GetChild(0).gameObject;
            divebell.SetActive(false);
            var prefab = Instantiate(divebell, templates.transform, true);
            prefab.name = "DiveBell_Template";
            CustomMapInitializer.diveBellPrefab = prefab;
            var m1 = PluginTools.FindObjectInScene(SceneManager.GetSceneByName("FactoryScene"),
                "Factory Blockout");
            var m2 = PluginTools.FindChildInParent(m1, "Floor");
            var m3 = PluginTools.FindChildInParent(m2, "Roof (3)");
            var renderer = m3.GetComponent<MeshRenderer>();
            CustomMapInitializer.worldMaterial = renderer.material;
            var unloadScene = SceneManager.UnloadSceneAsync("FactoryScene");
            yield return unloadScene;
            ModInitialized = true;
        }
    }
}