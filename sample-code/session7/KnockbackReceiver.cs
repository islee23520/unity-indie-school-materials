using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace Metroidvania.Combat
{
    public class KnockbackReceiver : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float baseKnockbackForce = 5f;
        [SerializeField] private float knockbackDuration = 0.3f;

        private MotionHandle currentKnockback;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
        }

        public void ApplyKnockback(Vector2 direction, float damageAmount)
        {
            if (currentKnockback.IsActive())
            {
                currentKnockback.Cancel();
            }

            float force = baseKnockbackForce + (damageAmount * 0.05f);
            Vector2 targetPosition = rb.position + (direction * force);

            currentKnockback = LMotion.Create(rb.position, targetPosition, knockbackDuration)
                .WithEase(Ease.OutQuad)
                .BindToPosition(rb.transform);
        }
    }
}
