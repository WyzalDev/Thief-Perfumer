using System.Collections;
using Game.Scripts.Events;
using Game.Scripts.SceneManagement;
using Game.Scripts.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour, IBlockable
    {
        [Header("Movement settings")]
        [SerializeField] private float speed;
        [Header("Dash settings")]
        [SerializeField] private float dashForce;
        [SerializeField] private float dashDuration;
        [SerializeField] private float dashCooldown;
        [Header("AudioSettings")]
        [SerializeField] private float footStepPeriodicity;
        public bool IsBlocked { get; private set; }

        private Rigidbody2D _rigidbody;
        private InputAction _move;
        private InputAction _dash;
        private Vector2 _direction;
        private Vector2 _lastDirection;
        private Coroutine _dashCoroutine;
        private Animator _animator;
        private bool _moving;

        private float currentStepTime;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
            IsBlocked = false;
            _move = InputSystem.actions.FindAction("Move");
            _dash = InputSystem.actions.FindAction("Dash");
            _dash.performed += Dash;
            EventManager.PlayerCaught += EndGameBlock;
            EventManager.PlayerGetTarget += EndGameBlock;
        }

        public void FixedUpdate()
        {
            if(IsBlocked) return;

            _rigidbody.linearVelocity = _direction * speed;
            // var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            // _rigidbody.rotation = angle;
        }

        private void Update()
        {
            _direction = _move.ReadValue<Vector2>();
            if(Vector2.zero != _direction) _lastDirection = _direction;

            if (_rigidbody.linearVelocity.magnitude != 0 && !IsBlocked)
            {
              currentStepTime = Mathf.Clamp(currentStepTime + Time.deltaTime, 0, footStepPeriodicity);
              if (Mathf.Approximately(currentStepTime, footStepPeriodicity))
              {
                  currentStepTime = 0;
                  EventManager.InvokeOnStep();
              }
            }
            else
            {
                currentStepTime = 0;
            }
            
            Animate();
        }

        private void Animate()
        {
            if (_direction.magnitude > 0.1f || _direction.magnitude < - 0.1f)
            {
                _moving = true;
            }
            else
            {
                _moving = false;
            }

            if (_moving)
            {
                _animator.SetFloat("X", _direction.x);
                _animator.SetFloat("Y", _direction.y);
            }
            
            _animator.SetBool("Moving", _moving);
        }

        private void Dash(InputAction.CallbackContext context)
        {
            Debug.Log("Dash");
            if(_dashCoroutine == null && !IsBlocked)
                _dashCoroutine = StartCoroutine(DoDash());
        }

        private IEnumerator DoDash()
        {
            EventManager.InvokeOnDash();
            IsBlocked = true;
            Debug.Log(_lastDirection.normalized);
            _rigidbody.linearVelocity = _lastDirection.normalized * dashForce;
            yield return new WaitForSeconds(dashDuration);
            IsBlocked = false;
            yield return new WaitForSeconds(dashCooldown);
            _dashCoroutine = null;
        }

        private void EndGameBlock(SceneName sceneName)
        {
            EndGameBlock();
        }
        
        private void EndGameBlock()
        {
            Block();
            _rigidbody.linearVelocity = Vector2.zero;
        }
        
        public void Block()
        {
            if (_dashCoroutine != null) ClearCoroutine();
            IsBlocked = true;
        }

        public void UnBlock()
        {
            if (_dashCoroutine != null) ClearCoroutine();
            IsBlocked = false;
        }

        private void ClearCoroutine()
        {
            StopCoroutine(_dashCoroutine);
            _dashCoroutine = null;
            _rigidbody.linearVelocity = Vector2.zero;
        }
        
        private void OnDestroy()
        {
            _dash.performed -= Dash;
            EventManager.PlayerCaught -= EndGameBlock;
            EventManager.PlayerGetTarget -= EndGameBlock;
        }
    }
}