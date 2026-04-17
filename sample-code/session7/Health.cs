using UnityEngine;
using R3;
using System;
using System.Collections;

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
                StartCoroutine(InvincibilityCoroutine());
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

        private IEnumerator InvincibilityCoroutine()
        {
            isInvincible = true;

            if (spriteRenderer != null)
            {
                float elapsed = 0f;
                while (elapsed < invincibilityDuration)
                {
                    spriteRenderer.color = new Color(1, 1, 1, 0.5f);
                    yield return new WaitForSeconds(0.1f);
                    spriteRenderer.color = Color.white;
                    yield return new WaitForSeconds(0.1f);
                    elapsed += 0.2f;
                }
            }
            else
            {
                yield return new WaitForSeconds(invincibilityDuration);
            }

            isInvincible = false;
        }
    }
}
