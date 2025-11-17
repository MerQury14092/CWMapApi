using HarmonyLib;
using Random = System.Random;

namespace CWMapApi.Patches
{
    [HarmonyPatch(typeof(RoomStatsHolder))]
    public class RoomStatsHolderPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("NewMapToPlay")]
        private static void NewMapToPlayPrefix()
        {
            CustomMapInitializer.instance = null;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch("NewMapToPlay")]
        private static void NewMapToPlayPatch(RoomStatsHolder __instance)
        {
            if (MapApi.EnableRandomMapPatch)
            {
                __instance.LevelToPlay = 3;
                // __instance.LevelToPlay = new Random().Next(0, 3 + MapApi.CustomMaps.Count);
            }
        }
    }
}