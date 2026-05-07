using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Metroidvania.Combat
{
    public class AttackController : MonoBehaviour
    {
        [SerializeField] private Hitbox hitbox;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float hitboxActiveDuration = 0.2f;
        [SerializeField] private float windupDuration = 0.1f;

        private float lastAttackTime;
        private bool canAttack = true;

        private void Awake()
        {
            if (hitbox != null)
            {
                hitbox.SetAttacker(gameObject);
                hitbox.gameObject.SetActive(false);
            }
        }

        public void PerformAttack()
        {
            if (!canAttack || Time.time - lastAttackTime < attackCooldown) return;
            AttackSequenceAsync().Forget();
        }

        private async UniTaskVoid AttackSequenceAsync()
        {
            canAttack = false;
            lastAttackTime = Time.time;

            // 1. Windup
            await UniTask.WaitForSeconds(windupDuration);

            // 2. Hitbox Active
            hitbox.gameObject.SetActive(true);
            await UniTask.WaitForSeconds(hitboxActiveDuration);
            hitbox.gameObject.SetActive(false);

            // 3. Cooldown
            float remainingCooldown = attackCooldown - windupDuration - hitboxActiveDuration;
            if (remainingCooldown > 0)
            {
                await UniTask.WaitForSeconds(remainingCooldown);
            }

            canAttack = true;
        }
    }
}
