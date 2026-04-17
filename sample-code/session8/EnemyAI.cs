using UnityEngine;
using Metroidvania.Combat;

namespace Metroidvania.Enemy
{
    public abstract class EnemyAI : MonoBehaviour
    {
        [SerializeField] protected float chaseRange = 5f;
        [SerializeField] protected float attackRange = 1f;
        [SerializeField] protected float moveSpeed = 3f;

        protected EnemyState currentState;
        protected Transform player;
        protected Health health;

        protected virtual void Awake()
        {
            health = GetComponent<Health>();
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        protected virtual void Update()
        {
            if (health != null && health.CurrentHealth.Value <= 0)
            {
                ChangeState(EnemyState.Dead);
                return;
            }

            UpdateState();
        }

        protected abstract void UpdateState();

        protected virtual void ChangeState(EnemyState newState)
        {
            if (currentState == newState) return;
            currentState = newState;
        }

        public virtual void Initialize()
        {
            currentState = EnemyState.Idle;
        }
    }
}
