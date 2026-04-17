using UnityEngine;
using System;

namespace Metroidvania.Combat
{
    public class Hurtbox : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Defense defense;

        public event Action<DamageInfo> OnHit;

        private void Awake()
        {
            if (health == null) health = GetComponent<Health>();
            if (defense == null) defense = GetComponent<Defense>();
        }

        public void TakeHit(DamageInfo damageInfo)
        {
            float finalDamage = damageInfo.GetFinalDamage();

            if (defense != null)
            {
                finalDamage = defense.ApplyDefense(damageInfo);
            }

            if (health != null)
            {
                health.TakeDamage(finalDamage);
            }

            OnHit?.Invoke(damageInfo);
            ApplyKnockback(damageInfo, finalDamage);
        }

        private void ApplyKnockback(DamageInfo damageInfo, float finalDamage)
        {
            KnockbackReceiver knockback = GetComponent<KnockbackReceiver>();
            if (knockback != null && damageInfo.Attacker != null)
            {
                Vector2 direction = (transform.position - damageInfo.Attacker.transform.position).normalized;
                knockback.ApplyKnockback(direction, finalDamage);
            }
        }
    }
}
