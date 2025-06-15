using System.Collections;
using Game.Scripts.Events;
using Game.Scripts.SceneManagement;
using UnityEngine;

namespace Game.Scripts.GameFlow
{
    public class EndGame : MonoBehaviour
    {

        [SerializeField] private float resetDelay;
        [SerializeField] private SceneName sceneName;
        
        private void Start()
        {
            EventManager.PlayerCaught += Caught;
            EventManager.PlayerGetTarget += PlayerGetTarget;
        }

        private void Caught()
        {
            StartCoroutine(RestartLevel());
        }

        private void PlayerGetTarget(SceneName sceneName)
        {
            StartCoroutine(GoToNextLevel(sceneName));
        }
        
        private IEnumerator RestartLevel()
        {
            yield return new WaitForSeconds(resetDelay);
            StartCoroutine(SceneLoader.LoadScene(sceneName));
        }

        private IEnumerator GoToNextLevel(SceneName sceneName)
        {
            yield return new WaitForSeconds(resetDelay);
            StartCoroutine(SceneLoader.LoadScene(sceneName));
        }

        private void OnDestroy()
        {
            EventManager.PlayerCaught -= Caught;
            EventManager.PlayerGetTarget -= PlayerGetTarget;
        }
    }
}