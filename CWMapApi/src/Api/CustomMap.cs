using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CWMapApi.Api
{
    public abstract class CustomMap
    {
        public abstract string GetMapName();
        public abstract string GetSceneName();
        public abstract GameObject GetPatrolPoints();
        public abstract GameObject GetDiveBells();
        public abstract GameObject GetNavmesh();
        public abstract GameObject GetNavmeshWide();
        public abstract GameObject GetLevel();

        public virtual string GetMapIconPath()
        {
            return null;
        }

        protected GameObject GetGameObjectInMyMap(string path)
        {
            string[] parts = path.Split('/');
            if(parts.Length == 0)
                return null;

            try
            {
                var scene = SceneManager.GetSceneByName(GetSceneName());
                if(parts.Length >= 1)
                    return PluginTools.FindObjectInSceneRecursive(scene, parts[0]);
            }
            catch (Exception)
            {
                return null;
            }
            
            return null;
        }
        
        protected void SetDefaultWorldMaterial(GameObject obj)
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            if (renderer)
            {
                renderer.material = CustomMapInitializer.worldMaterial;
            }
        }

        protected void SetDefaultSkybox()
        {
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;
        }
        
        protected void SetDefaultWorldMaterialRecursive(GameObject obj)
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            if (renderer)
            {
                renderer.material = CustomMapInitializer.worldMaterial;
            }

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                SetDefaultWorldMaterialRecursive(obj.transform.GetChild(i).gameObject);
            }
        }

        public virtual IEnumerator Prefix()
        {
            yield return null;
        }
        
        public virtual IEnumerator Postfix()
        {
            yield return null;
        }
    }
}