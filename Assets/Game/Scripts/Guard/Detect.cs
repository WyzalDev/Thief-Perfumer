using System;
using System.Collections;
using Game.Scripts.Events;
using UnityEngine;

namespace Game.Scripts.Guard
{
    public class Detect : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float guardNewDetectionDelay = 0.5f;

        private Guard _guard;
        private float _detectionRadius;
        private float _criticalDetectionRadius;

        private float _currentDistanceToPlayer = float.MaxValue;
        
        private void Awake()
        {
            _guard = GetComponent<Guard>();
        }

        private void Start()
        {
            _detectionRadius = _guard.DetectionRadius;
            _criticalDetectionRadius = _guard.CriticalDetectionRadius;
            StartCoroutine(DetectPlayer());
        }

        private void Update()
        { 
            _currentDistanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);
        }

        private IEnumerator DetectPlayer()
        {
            while (_guard.GuardState is GuardState.Patrol or GuardState.Detect)
            {
                yield return new WaitUntil(() => _currentDistanceToPlayer < _detectionRadius);
                if (_currentDistanceToPlayer < _criticalDetectionRadius)
                {
                    EventManager.InvokeOnCriticalPlayerDetect(playerTransform.position);
                }
                else
                {
                    EventManager.InvokeOnPlayerDetect(playerTransform.position, _guard.GuardID);
                    yield return new WaitForSecondsRealtime(guardNewDetectionDelay);
                }
            }
        }
    }

    public enum DetectType
    {
        Standard,
        Critical
    }
}