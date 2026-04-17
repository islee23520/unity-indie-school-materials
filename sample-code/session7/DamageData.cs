using UnityEngine;

namespace Metroidvania.Combat
{
    public enum DamageType
    {
        Physical,
        Magical,
        True
    }

    public struct DamageInfo
    {
        public float BaseDamage;
        public float CriticalMultiplier;
        public bool IsCritical;
        public DamageType DamageType;
        public GameObject Attacker;

        public float GetFinalDamage()
        {
            float final = BaseDamage;
            if (IsCritical)
            {
                final *= CriticalMultiplier;
            }
            return final;
        }
    }
}
