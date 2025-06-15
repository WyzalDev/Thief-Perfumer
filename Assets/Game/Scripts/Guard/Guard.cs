using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.Events;
using Game.Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Guard
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Guard : MonoBehaviour
    {
        [Header("MovementSettings")]
        [SerializeField] private float rotationSpeed;

        [Header("Patrol Settings")]
        [SerializeField] private List<Transform> patrolPoints;
        [SerializeField] private float waitTimeAtPoint;

        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius;
        [SerializeField] private float criticalDetectionRadius;
        [SerializeField] private FieldOfView fov;
        [SerializeField] private Material materialPatrol;
        [SerializeField] private Material materialDetect;
        [SerializeField] private Material materialCriticalDetect;

        [Header("Reaction Settings")]
        [SerializeField] private float reactionDelay;
        [SerializeField] private float returnToPatrolWaitTime;
        public GuardState GuardState { get; private set; }
        public int GuardID { get; private set; }

        public float DetectionRadius
        {
            get => detectionRadius;
            private set => detectionRadius = value;
        }

        public float CriticalDetectionRadius
        {
            get => criticalDetectionRadius;
            private set => criticalDetectionRadius = value;
        }


        private static int _guardsCount;
        private NavMeshAgent _agent;
        private int _currentPatrolPoint;
        private Vector3 _lastSeenPlayerPoint;

        //Cooroutines
        private Coroutine _reactionCoroutine;
        private Coroutine _waitOnPointCoroutine;
        private Coroutine _stateCoroutine;

        //rotation
        private float _currentAngle;
        private float _angleVelocity;
        
        private void Awake()
        {
            GuardID = _guardsCount;
            _guardsCount++;
            _agent = GetComponent<NavMeshAgent>();
            EventManager.OnPlayerDetect += Detect;
            EventManager.OnCriticalPlayerDetect += CriticalDetect;
            EventManager.PlayerGetTarget += EndGameBlock;
            EventManager.PlayerCaught += EndGameBlock;
        }

        private void Start()
        {
            _agent.autoBraking = true;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;

            SetState(GuardState.Patrol);

            _currentAngle = transform.eulerAngles.z;
        }

        private void Update()
        {
            // if (_agent.desiredVelocity.sqrMagnitude > 0.01f)
            // {
            //     var direction = _agent.desiredVelocity.normalized;
            //     var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //
            //     _currentAngle = Mathf.SmoothDampAngle(
            //         _currentAngle,
            //         targetAngle,
            //         ref _angleVelocity,
            //         rotationSpeed * Time.deltaTime
            //     );
            //
            //     transform.rotation = Quaternion.Euler(0, 0, _currentAngle);
            // }
            
            fov.SetOrigin(transform.position);
            fov.SetAimDirection(_agent.desiredVelocity.normalized);
        }

        private void SetState(GuardState newState)
        {
            if (GuardState == GuardState.CriticalDetect) return;
            GuardState = newState;
            if (_stateCoroutine != null) StopCoroutine(_stateCoroutine);
            switch (newState)
            {
                case GuardState.Patrol:
                    fov.SetMaterial(materialPatrol);
                    _stateCoroutine = StartCoroutine(PatrolState());
                    break;
                case GuardState.Detect:
                    fov.SetMaterial(materialDetect);
                    _stateCoroutine = StartCoroutine(DetectState());
                    break;
            }
        }

        private IEnumerator PatrolState()
        {
            while (GuardState == GuardState.Patrol)
            {
                yield return new WaitUntil(() => IsAgentDestinationReached(0.5f));
                if (GuardState != GuardState.Patrol) continue;
                _waitOnPointCoroutine = StartCoroutine(WaitAtPatrolPoint());
                yield return _waitOnPointCoroutine;
                if (GuardState != GuardState.Patrol) continue;
                GoToNextWayPoint();
            }
        }

        private IEnumerator DetectState()
        {
            while (GuardState == GuardState.Detect)
            {
                yield return new WaitUntil(() => IsAgentDestinationReached(0.2f));
                if (GuardState != GuardState.Detect) continue;
                yield return ReturnToPatrol();
            }
        }

        private bool IsAgentDestinationReached(float customThreshold = -1)
        {
            var threshold = customThreshold > 0 ? customThreshold : _agent.stoppingDistance;
            return !_agent.pathPending
                   && _agent.remainingDistance <= threshold
                   && _agent.velocity.sqrMagnitude == 0;
        }

        private void Detect(Vector3 playerPoint, int guardID)
        {
            if (guardID != GuardID) return;

            if (IsObstacleOnWay(playerPoint, detectionRadius)) return;

            StopAllBehaviorCoroutines();
            Debug.Log("Detect player");
            _reactionCoroutine = StartCoroutine(ReactToPlayer(playerPoint));
        }

        private bool IsObstacleOnWay(Vector3 playerPoint, float radius)
        {
            var direction = playerPoint - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, radius,
                LayerMask.GetMask("Obstacle"));
            return hit.collider != null && hit.collider.CompareTag("Obstacle") && !hit.collider.CompareTag("DeadZone");
        }

        private IEnumerator ReactToPlayer(Vector3 playerPoint)
        {
            _lastSeenPlayerPoint = playerPoint;
            _agent.SetDestination(_lastSeenPlayerPoint);
            _agent.isStopped = true;
            EventManager.InvokeOnDetect();

            var isAlreadyDetectState = GuardState == GuardState.Detect;
            SetState(GuardState.Detect);

            //var direction = playerPoint - transform.position;
            //var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //transform.DORotate(new Vector3(0, 0, targetAngle), reactionDelay / 10);

            if (!isAlreadyDetectState)
                yield return new WaitForSeconds(reactionDelay);

            _agent.isStopped = false;
        }

        private void CriticalDetect(Vector3 playerPoint)
        {
            if (IsObstacleOnWay(playerPoint, criticalDetectionRadius)) return;
            
            Debug.Log("Critical detect player");
            StopAllBehaviorCoroutines(true);
            GuardState = GuardState.CriticalDetect;
            _agent.isStopped = true;
            _guardsCount = 0;

            fov.SetMaterial(materialCriticalDetect);
            EventManager.InvokePlayerCaught();
        }

        private IEnumerator WaitAtPatrolPoint()
        {
            yield return new WaitForSeconds(waitTimeAtPoint);
        }

        private void StopAllBehaviorCoroutines(bool stopState = false)
        {
            if (stopState)
                if (_stateCoroutine != null)
                    StopCoroutine(_stateCoroutine);
            if (_reactionCoroutine != null) StopCoroutine(_reactionCoroutine);
            if (_waitOnPointCoroutine != null) StopCoroutine(_waitOnPointCoroutine);
        }

        private void GoToNextWayPoint(bool isReturn = false)
        {
            if (patrolPoints.Count == 0) return;
            _agent.destination = patrolPoints[_currentPatrolPoint].position;
            if (!isReturn)
                _currentPatrolPoint = (_currentPatrolPoint + 1) % patrolPoints.Count;
        }

        private IEnumerator ReturnToPatrol()
        {
            yield return new WaitForSeconds(returnToPatrolWaitTime);
            SetState(GuardState.Patrol);
            GoToNextWayPoint(true);
        }

        private void EndGameBlock(SceneName sceneName)
        {
            EndGameBlock();
        }
        
        private void EndGameBlock()
        {
            StopAllBehaviorCoroutines();
            _agent.isStopped = true;
        }

        private void OnDestroy()
        {
            EventManager.OnPlayerDetect -= Detect;
            EventManager.OnCriticalPlayerDetect -= CriticalDetect;
            EventManager.PlayerGetTarget -= EndGameBlock;
            EventManager.PlayerCaught -= EndGameBlock;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, criticalDetectionRadius);
            if (patrolPoints.Count == 0) return;
            if (patrolPoints[0] == null) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolPoints[0].position, 0.2f);
            if (patrolPoints.Count == 1)
            {
                return;
            }

            if (patrolPoints[1] == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(patrolPoints[1].position, 0.2f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLineStrip(patrolPoints.Where(p => p != null).Select(p => p.position).ToArray(), true);
        }
    }

    public enum GuardState
    {
        Patrol,
        Detect,
        CriticalDetect
    }
}