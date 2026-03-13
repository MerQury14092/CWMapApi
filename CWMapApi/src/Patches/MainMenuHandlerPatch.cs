using System;
using System.Collections;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CWMapApi.Patches
{
    [HarmonyPatch(typeof(MainMenuHandler))]
    internal class MainMenuHandlerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        internal static void PostAwake(MainMenuHandler __instance)
        {
            __instance.StartCoroutine(MapApiInit(__instance.gameObject.scene));
        }

        private static IEnumerator MapApiInit(Scene scene)
        {
            if (MapApi.ModInitialized)
                yield break;
            MapApi.log.LogInfo("Loading mod");
            var canvas = PluginTools.FindObjectInScene(scene, "Canvas");
            var mainPage = PluginTools.FindChildInParent(canvas, "MainPage");
            var loadingPage = PluginTools.FindChildInParent(canvas, "LoadingPage");
            mainPage.SetActive(false);
            loadingPage.GetComponent<MainMenuLoadingPage>().SetText("Map Api loading");
            loadingPage.SetActive(true);
            yield return MapApi.InitializeMod();
            loadingPage.SetActive(false);
            mainPage.SetActive(true);
        }
    }
}