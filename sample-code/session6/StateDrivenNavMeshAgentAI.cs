using UnityEngine;
using UnityEngine.AI;

namespace Metroidvania.Session6
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class StateDrivenNavMeshAgentAI : MonoBehaviour
    {
        private enum AIState
        {
            Idle,
            Patrol,
            Chase,
            Return
        }

        [Header("Targets")]
        [SerializeField] private Transform _player;
        [SerializeField] private Transform[] _patrolPoints;

        [Header("Ranges")]
        [SerializeField] private float _detectRange = 7f;
        [SerializeField] private float _loseRange = 10f;

        [Header("Movement")]
        [SerializeField] private float _patrolSpeed = 2f;
        [SerializeField] private float _chaseSpeed = 4f;
        [SerializeField] private float _rotationLerp = 12f;

        private NavMeshAgent _agent;
        private AIState _state;
        private int _patrolIndex;
        private Vector3 _spawnPosition;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _spawnPosition = transform.position;
            ChangeState(AIState.Patrol);
        }

        private void Update()
        {
            switch (_state)
            {
                case AIState.Idle:
                    UpdateIdle();
                    break;
                case AIState.Patrol:
                    UpdatePatrol();
                    break;
                case AIState.Chase:
                    UpdateChase();
                    break;
                case AIState.Return:
                    UpdateReturn();
                    break;
            }

            UpdateRotation();
        }

        private void UpdateIdle()
        {
            if (CanDetectPlayer())
            {
                ChangeState(AIState.Chase);
            }
        }

        private void UpdatePatrol()
        {
            _agent.speed = _patrolSpeed;

            if (_patrolPoints != null && _patrolPoints.Length > 0)
            {
                if (!_agent.hasPath || _agent.remainingDistance <= 0.3f)
                {
                    _agent.SetDestination(_patrolPoints[_patrolIndex].position);
                    _patrolIndex = (_patrolIndex + 1) % _patrolPoints.Length;
                }
            }

            if (CanDetectPlayer())
            {
                ChangeState(AIState.Chase);
            }
        }

        private void UpdateChase()
        {
            if (_player == null)
            {
                ChangeState(AIState.Return);
                return;
            }

            _agent.speed = _chaseSpeed;
            _agent.SetDestination(_player.position);

            float distance = Vector2.Distance(transform.position, _player.position);
            if (distance > _loseRange)
            {
                ChangeState(AIState.Return);
            }
        }

        private void UpdateReturn()
        {
            _agent.speed = _patrolSpeed;
            _agent.SetDestination(_spawnPosition);

            if (_agent.remainingDistance <= 0.3f)
            {
                ChangeState(AIState.Patrol);
            }
        }

        private void ChangeState(AIState next)
        {
            _state = next;
        }

        private bool CanDetectPlayer()
        {
            if (_player == null)
            {
                return false;
            }

            return Vector2.Distance(transform.position, _player.position) <= _detectRange;
        }

        private void UpdateRotation()
        {
            Vector2 velocity = _agent.desiredVelocity;
            if (velocity.sqrMagnitude < 0.0001f)
            {
                return;
            }

            float targetAngle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _rotationLerp);
        }
    }
}
