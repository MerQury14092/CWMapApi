using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Zorro.Core;

namespace CWMapApi.Patches
{
    [HarmonyPatch(typeof(DivingBell))]
    internal class DivingBellPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("RPC_GoToUnderground")]
        static bool RPC_GoToUnderground_Prefix(DivingBell __instance)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }
        
            SurfaceNetworkHandler instance = SurfaceNetworkHandler.Instance;
            if (instance == null)
            {
                return false;
            }
        
            if (!(__instance.StateMachine.CurrentState is DivingBellReadyState))
            {
                return false;
            }
        
            if (instance.PreCheckHeadToUnderWorld())
            {
                __instance.photonView.RPC("RPC_StartTransition", RpcTarget.Others);
                PhotonGameLobbyHandler.Instance.CheckForIllegalItems();
                Common.InvokeMethod(__instance, "TransitionGameFeel", Array.Empty<object>());
                
                RetrievableResourceSingleton<TransitionHandler>.Instance.TransitionToBlack(0.15f, delegate
                {
                    string text;
                    List<string> levels = new List<string> { "FactoryScene", "MinesScene", "HarbourScene" };
                    int levelToPlay = SurfaceNetworkHandler.RoomStats.LevelToPlay;
                    if (levelToPlay < levels.Count)
                        text = levels[levelToPlay];
                    else
                        text = MapApi.CustomMaps[levelToPlay - 3].GetSceneName();
                    RetrievableSingleton<PersistentObjectsHolder>.Instance.FindPersistantSurfaceObjects();
                    MapApi.log.LogInfo($"Loading map with scene: {text} (level: {levelToPlay})");
                    PhotonNetwork.LoadLevel(text);
                    CWMapApiManager.view.RPC("RPC_LoadCustomMap", RpcTarget.All, levelToPlay - 3);
                }, 3f);
            }
        
            return false;
        }
    }
}