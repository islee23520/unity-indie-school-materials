using UnityEngine;

namespace Metroidvania.Combat
{
    public class Defense : MonoBehaviour
    {
        [SerializeField] private float physicalDefense = 10f;
        [SerializeField] private float magicalDefense = 5f;

        public float CalculateDamageReduction(DamageType type)
        {
            float defense = type switch
            {
                DamageType.Physical => physicalDefense,
                DamageType.Magical => magicalDefense,
                DamageType.True => 0f,
                _ => 0f
            };

            // Formula: reduction = defense / (defense + 100)
            return defense / (defense + 100f);
        }

        public float ApplyDefense(DamageInfo damageInfo)
        {
            if (damageInfo.DamageType == DamageType.True)
            {
                return damageInfo.GetFinalDamage();
            }

            float reduction = CalculateDamageReduction(damageInfo.DamageType);
            return damageInfo.GetFinalDamage() * (1f - reduction);
        }
    }
}
