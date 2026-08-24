using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Game.OverworldDisableManager
{
    public class OverworldDisableManager
    {

        public static bool DisableAllObjects(Scene scene, List<GameObject> ignorableObjects)
        {
            GameObject[] allObjs = scene.GetRootGameObjects();
            bool disabler = true;

            Debug.Log("Ignorable Object Dump:");
            foreach (GameObject obj in ignorableObjects)
            {
                Debug.Log($"Obj: {obj.name}");
            }

            foreach (GameObject obj in allObjs)
            {
                if (ignorableObjects.Contains(obj))
                    disabler = false;
                foreach(Transform child in obj.transform)
                {
                    GameObject childObj = child.gameObject;

                    if(ignorableObjects.Contains(childObj))
                    {
                        disabler = false;
                        break;
                    }


                }

                if(disabler)
                obj.SetActive(false);
                disabler = true;

            }

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

            bool enabler = true;

            Debug.Log("Ignorable Object Dump:");
            foreach(GameObject obj in ignorableObjects)
            {
                Debug.Log($"Obj: {obj.name}");
            }

            foreach (GameObject obj in allObjs)
            {
                foreach (Transform child in obj.transform)
                {
                    GameObject childObj = child.gameObject;

                    if (ignorableObjects.Contains(childObj))
                    {
                        enabler = false;
                        break;
                    }


                }

                if (enabler)
                    obj.SetActive(true);


            }

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

