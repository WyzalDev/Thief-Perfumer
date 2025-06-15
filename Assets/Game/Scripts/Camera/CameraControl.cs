using Game.Scripts.Events;
using Game.Scripts.SceneManagement;
using Game.Scripts.Utility;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Scripts.Camera
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private float size;
        [SerializeField] private float sizeMultiplier;
        [SerializeField] private float zoomDuration;
        
        private CinemachineCamera _camera;

        private void Start()
        {
            EventManager.PlayerCaught += EndGameZoom;
            EventManager.PlayerGetTarget += EndGameZoom;
            _camera = GetComponent<CinemachineCamera>();
            _camera.Lens.OrthographicSize = size * sizeMultiplier;
            _camera.DOOrthoSize(size, zoomDuration);
        }

        private void EndGameZoom(SceneName sceneName)
        {
            EndGameZoom();
        }
        
        private void EndGameZoom()
        {
            _camera.DOOrthoSize(size*sizeMultiplier, zoomDuration);
            Debug.Log("EndGameZoom");
        }

        private void OnDestroy()
        {
            EventManager.PlayerCaught -= EndGameZoom;
            EventManager.PlayerGetTarget -= EndGameZoom;
        }
    }
}