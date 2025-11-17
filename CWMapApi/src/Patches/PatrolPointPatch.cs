using HarmonyLib;

namespace CWMapApi.Patches
{
    [HarmonyPatch(typeof(PatrolPoint))]
    internal class PatrolPointPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        internal static void Prefix(PatrolPoint __instance)
        {
            PluginTools.AddCubeToPoint(__instance.transform);
        }
    }
}