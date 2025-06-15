using System;
using Game.Scripts.Events;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Guard
{
    public class FootStepGuard : MonoBehaviour
    {
        [SerializeField] private float footstepDistance;

        private NavMeshAgent _agent;
        private float _currentFootstepDistance;
        private Vector3 _lastPosition;
        private AudioSource _audio;
        private Animator _animator;

        private bool _moving;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _audio = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
            _lastPosition = transform.position;
        }
        
        void Update()
        {
            if (_agent.velocity.magnitude > 0.1f) // Движется
            {
                // Считаем пройденное расстояние
                _currentFootstepDistance += Vector3.Distance(transform.position, _lastPosition);
                _lastPosition = transform.position;

                // Проверяем порог шага
                if (_currentFootstepDistance >= footstepDistance)
                {
                    EventManager.InvokeOnGuardStep(_audio);
                    _currentFootstepDistance = 0;
                }
            }
            else
            {
                _currentFootstepDistance = 0; // Сброс при остановке
            }

            Animate();
        }
        
        private void Animate()
        {
            var direction = _agent.desiredVelocity.normalized;
            if (direction.magnitude > 0.1f || direction.magnitude < - 0.1f)
            {
                _moving = true;
            }
            else
            {
                _moving = false;
            }

            if (_moving)
            {
                _animator.SetFloat("X", direction.x);
                _animator.SetFloat("Y", direction.y);
            }
            
            _animator.SetBool("Moving", _moving);
        }
        
        
    }
}