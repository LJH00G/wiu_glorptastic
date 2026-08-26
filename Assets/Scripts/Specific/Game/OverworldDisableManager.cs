using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace Game.OverworldDisableManager
{
    public class OverworldDisableManager
    {

        public static bool DisableAllObjects(Scene scene, List<GameObject> ignorableObjects)
        {
            GameObject[] allObjs = scene.GetRootGameObjects();
            List<Transform> validTransform = new List<Transform>();
            validTransform = allObjs.Join(ignorableObjects, obj => obj.transform, ignoreObj => ignoreObj.transform.root, (obj, ignoreObj) => obj.transform).Distinct().ToList();
            
            Debug.Log("Ignorable Object Dump:");
            foreach (GameObject obj in ignorableObjects)
            {
                Debug.Log($"Obj: {obj.name}");
            }

            foreach (GameObject obj in allObjs)
                if (!validTransform.Contains(obj.transform))
                    obj.SetActive(false);
                
            

            return true;
        }

        public static bool DisableAllObjects(Scene scene)
        {
            GameObject[] allObjs = scene.GetRootGameObjects();
            
            foreach (GameObject obj in allObjs)
            {
                obj.SetActive(false);
            }

            return true;
        }

        public static bool EnableAllObjects(Scene scene, List<GameObject> ignorableObjects)
        {
            GameObject[] allObjs = scene.GetRootGameObjects();


            Debug.Log("Ignorable Object Dump:");
            foreach(GameObject obj in ignorableObjects)
            {
                Debug.Log($"Obj: {obj.name}");
            }

            List<Transform> validTransform = new List<Transform>();
            validTransform = allObjs.Join(ignorableObjects, obj => obj.transform, ignoreObj => ignoreObj.transform.root, (obj, ignoreObj) => obj.transform).Distinct().ToList();

            foreach (GameObject obj in allObjs)
                if (!validTransform.Contains(obj.transform))
                    obj.SetActive(true);

            return true;
        }


        public static bool EnableAllObjects(Scene scene)
        {
            GameObject[] allObjs = scene.GetRootGameObjects();

            foreach (GameObject obj in allObjs)
            {
                obj.SetActive(true);
            }

            return true;
        }
    }
}

