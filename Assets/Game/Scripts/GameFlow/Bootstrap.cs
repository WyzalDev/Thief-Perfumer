using System.Collections;
using Game.Scripts.SceneManagement;
using UnityEngine;

namespace Game.Scripts.GameFlow
{
    public class Bootstrap : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(SceneLoader.LoadScene(SceneName.Tutorial1));
        }
    }
}