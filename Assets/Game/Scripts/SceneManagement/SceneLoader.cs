using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.SceneManagement
{
    public static class SceneLoader
    {
        public static bool isSceneLoading = false;
        
        public static IEnumerator LoadScene(SceneName sceneName)
        {
            isSceneLoading = true;
            
            var scene = SceneManager.LoadSceneAsync(sceneName.ToString());
            if (scene is not null)
            {
                scene.allowSceneActivation = false;
            }
            else
            {
                Debug.LogWarning($"Scene {sceneName} is not loaded");
                yield break;
            }
            
            do
            {
                yield return null;
            } while (scene.progress < 0.9f);
            scene.allowSceneActivation = true;
            isSceneLoading = false;
        }
    }
    
    public enum SceneName
    {
        SampleScene,
        Tutorial1,
        Tutorial2,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        WinScene
    }
}