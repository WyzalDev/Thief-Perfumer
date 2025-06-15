using Game.Scripts.Events;
using Game.Scripts.SceneManagement;
using UnityEngine;

namespace Game.Scripts.Target
{
    public class Target : MonoBehaviour
    {
        [SerializeField] private SceneName _nextScene;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                EventManager.InvokePlayerGetTarget(_nextScene);
            }
        }
    }
}