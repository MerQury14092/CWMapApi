using System;
using System.Collections;
using System.Collections.Generic;
using CWMapApi.Api;
using Photon.Pun;
using UnityEngine;

namespace CWMapApi
{
    internal class CWMapApiManager : MonoBehaviourPunCallbacks
    {
        private static GameObject hostObject;
        private static CWMapApiManager hostComponent;
        internal static PhotonView view;
    
        internal static void Initialize()
        {
            if (hostObject != null) return;
        
            hostObject = new GameObject("Manager");
            hostObject.transform.SetParent(MapApi.ParentGameObject.transform);
            hostComponent = hostObject.AddComponent<CWMapApiManager>();
            view = hostObject.AddComponent<PhotonView>();
            view.ViewID = 225235411;
        }

        private new static void StartCoroutine(IEnumerator coroutine)
        {
            Initialize();
            ((MonoBehaviour)hostComponent).StartCoroutine(coroutine);
        }

        [PunRPC]
        private void RPC_LoadCustomMap(int index)
        {
            StartCoroutine(CustomMapInitializer.InitializeMap(MapApi.CustomMaps[index]));
        }
            
        private void Update()
        {
            MapApi.Loop();
        }
    }
}