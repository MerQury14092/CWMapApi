using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CWMapApi
{
    internal class PluginTools
    {
        internal static void CopyTransform(Transform from, Transform to)
        {
            to.position = from.position;
            to.rotation = from.rotation;
            to.localScale = from.localScale;
        }
        
        internal static GameObject FindObjectInSceneRecursive(string sceneName, string objName)
        {
            return FindObjectInSceneRecursive(SceneManager.GetSceneByName(sceneName), objName); 
        }
        
        internal static GameObject FindObjectInSceneRecursive(Scene scene, string objName)
        {
            if (!scene.IsValid())
            {
                MapApi.log.LogError("FindObjectInSceneRecursive failed");
                return null;
            }

            foreach (var rootObj in scene.GetRootGameObjects())
            {
                GameObject result = FindObjectRecursive(rootObj, objName);
                if (result != null)
                    return result;
            }

            return null;
        }

        private static GameObject FindObjectRecursive(GameObject parent, string objName)
        {
            if (parent.name == objName)
                return parent;

            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject child = parent.transform.GetChild(i).gameObject;
                GameObject result = FindObjectRecursive(child, objName);
                if (result != null)
                    return result;
            }

            return null;
        }
        
        internal static void AddCubeToPoint(Transform point)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Visual";
            cube.transform.SetParent(point);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one;
            cube.SetActive(false);
        }

        internal static GameObject FindChildInParent(GameObject parent, string childName)
        {
            GameObject res = null;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                if (parent.transform.GetChild(i).name == childName)
                    res = parent.transform.GetChild(i).gameObject;
            }
            return res;
        }

        internal static GameObject FindObjectInScene(string sceneName, string objName)
        {
            return FindObjectInScene(SceneManager.GetSceneByName(sceneName), objName); 
        }
        
        internal static GameObject FindObjectInScene(Scene scene, string objName)
        {
            GameObject res = null;
            foreach (var obj in scene.GetRootGameObjects())
            {
                if(obj.name == objName)
                    res = obj;
            }

            return res;
        }

        internal static IEnumerator WaitForSceneIsLoaded(Scene scene)
        {
            while (!scene.isLoaded)
            {
                yield return null;
            }
        }
    }
}