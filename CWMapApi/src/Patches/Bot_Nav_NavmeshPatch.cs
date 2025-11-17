using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.AI;

namespace CWMapApi.Patches
{
    [HarmonyPatch(typeof(Bot_Nav_Navmesh))]
    internal class Bot_Nav_NavmeshPatch
    {
        internal static List<NavMeshAgent> Agents = new List<NavMeshAgent>();
        
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        public static void Postfix(Bot_Nav_Navmesh __instance)
        {
            MapApi.log.LogInfo($"Add agent from {__instance.gameObject.transform.parent.gameObject.name}");
            Agents.Add(__instance.GetComponent<NavMeshAgent>());
        }
    }
}