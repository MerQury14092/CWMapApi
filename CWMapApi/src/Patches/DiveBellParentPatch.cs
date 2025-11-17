using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CWMapApi.Patches
{
    [HarmonyPatch(typeof(DiveBellParent))]
    internal class DiveBellParentPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        internal static bool DiveBellParent_Awake(DiveBellParent __instance)
        {
            CustomMapInitializer initializer = CustomMapInitializer.instance;
            if (!MapApi.ModInitialized || initializer == null)
                return true;
            if (!initializer.States.DiveBellLoaded)
            {
                initializer.States.DiveBellLoaded = true;
                return true;
            }
            Object.Destroy(__instance.gameObject);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("GetSpawn")]
        internal static bool DiveBellParent_GetSpawn(DiveBellParent __instance, ref SpawnPoint __result)
        {
            Random.State state = Random.state;
            Random.InitState(GameAPI.seed);

            List<Transform> spawnPoints = new List<Transform>();
            for (int i = 0; i < __instance.transform.childCount; i++)
            {
                spawnPoints.Add(__instance.transform.GetChild(i));
            }

            int p = Random.Range(0, spawnPoints.Count);
            Transform transform = spawnPoints[p];
            
            for (int i = 0; i < __instance.transform.childCount; i++)
            {
                if (__instance.transform.GetChild(i) == transform)
                {
                    __instance.transform.GetChild(i).gameObject.SetActive(value: true);
                }
                else
                {
                    __instance.transform.GetChild(i).gameObject.SetActive(value: false);
                }
            }
            Random.state = state;
            __result = transform.GetComponentInChildren<SpawnPoint>();
            return false;
        }
    }
}