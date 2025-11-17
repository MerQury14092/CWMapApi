using HarmonyLib;
using UnityEngine;

namespace CWMapApi.Patches
{
    [HarmonyPatch(typeof(Level))]
    internal class LevelPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        internal static bool Prefix(Level __instance)
        {
            CustomMapInitializer initializer = CustomMapInitializer.instance;
            if (!MapApi.ModInitialized || initializer == null)
                return true;
            
            if (!initializer.States.LevelLoaded)
            {
                initializer.States.LevelLoaded = true;
                return true;
            }
            
            Object.DestroyImmediate(__instance.gameObject);
            return false;
        }
    }
}