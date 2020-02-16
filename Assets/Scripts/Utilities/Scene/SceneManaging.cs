using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Debug;


namespace Clement.Utilities
{
    public static class SceneManaging
    {



        public static int CurrentLevelIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }

        public static string CurrentLevelName()
        {
            return SceneManager.GetActiveScene().name;
        }






        // This method finds all objects of type T in Scene, excluding Prefabs:
        public static T[] SearchObjectsInSceneOfTypeIncludingDisabled<T>()
        {
            var ActiveScene = SceneManager.GetActiveScene();
            var RootObjects = ActiveScene.GetRootGameObjects();
            var MatchObjects = new List<T>();

            foreach (var ro in RootObjects)
            {
                var Matches = ro.GetComponentsInChildren<T>(true);
                MatchObjects.AddRange(Matches);
            }

            return MatchObjects.ToArray();
        }
    }
}
