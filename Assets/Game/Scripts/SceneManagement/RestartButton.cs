using UnityEngine;

namespace Game.Scripts.SceneManagement
{
    public class RestartButton : MonoBehaviour
    {
        public void Restart()
        {
            StartCoroutine(SceneLoader.LoadScene(SceneName.Tutorial1));
        }
    }
}