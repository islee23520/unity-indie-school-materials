using UnityEngine;
using R3;
using System;
using Cysharp.Threading.Tasks;

namespace Metroidvania.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float invincibilityDuration = 0.5f;

        private readonly ReactiveProperty<float> currentHealth = new();
        private bool isInvincible = false;
        private SpriteRenderer spriteRenderer;

        public ReadOnlyReactiveProperty<float> CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercent => currentHealth.Value / maxHealth;

        public Subject<float> OnDamaged = new();
        public Subject<Unit> OnDeath = new();

        private void Awake()
        {
            currentHealth.Value = maxHealth;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void TakeDamage(float damage)
        {
            if (isInvincible || currentHealth.Value <= 0 || damage <= 0) return;

            currentHealth.Value = Mathf.Max(0, currentHealth.Value - damage);
            OnDamaged.OnNext(damage);

            if (currentHealth.Value <= 0)
            {
                Die();
            }
            else
            {
                InvincibilityAsync().Forget();
            }
        }

        public void Heal(float amount)
        {
            if (amount <= 0 || currentHealth.Value >= maxHealth) return;
            currentHealth.Value = Mathf.Min(maxHealth, currentHealth.Value + amount);
        }

        private void Die()
        {
            OnDeath.OnNext(Unit.Default);
            Debug.Log($"{gameObject.name} died!");
        }

        private async UniTaskVoid InvincibilityAsync()
        {
            isInvincible = true;

            if (spriteRenderer != null)
            {
                int blinkCount = Mathf.CeilToInt(invincibilityDuration / 0.2f);
                for (int i = 0; i < blinkCount; i++)
                {
                    spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                    await UniTask.Delay(100);
                    spriteRenderer.color = Color.white;
                    await UniTask.Delay(100);
                }
            }
            else
            {
                await UniTask.Delay((int)(invincibilityDuration * 1000));
            }

            isInvincible = false;
        }
    }
}
