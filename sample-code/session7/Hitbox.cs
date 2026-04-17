using UnityEngine;

namespace Metroidvania.Combat
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float criticalChance = 0.2f;
        [SerializeField] private float criticalMultiplier = 1.5f;
        [SerializeField] private DamageType damageType = DamageType.Physical;
        [SerializeField] private LayerMask targetLayers;

        private GameObject attacker;

        public void SetAttacker(GameObject attacker)
        {
            this.attacker = attacker;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == attacker) return;
            if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

            Hurtbox hurtbox = other.GetComponent<Hurtbox>();
            if (hurtbox != null)
            {
                hurtbox.TakeHit(CreateDamageInfo());
            }
        }

        private DamageInfo CreateDamageInfo()
        {
            bool isCritical = Random.value <= criticalChance;
            return new DamageInfo
            {
                BaseDamage = damage,
                CriticalMultiplier = criticalMultiplier,
                IsCritical = isCritical,
                DamageType = damageType,
                Attacker = attacker
            };
        }
    }
}
