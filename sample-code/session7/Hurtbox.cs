using UnityEngine;
using System;
using R3;

namespace Metroidvania.Combat
{
    public class Hurtbox : MonoBehaviour, IDisposable
    {
        [SerializeField] private Health health;
        [SerializeField] private Defense defense;

        private readonly Subject<DamageInfo> _onHit = new();

        public Observable<DamageInfo> OnHit => _onHit;

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

            _onHit.OnNext(damageInfo);
            ApplyKnockback(damageInfo, finalDamage);
        }

        public void Dispose()
        {
            _onHit.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
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
