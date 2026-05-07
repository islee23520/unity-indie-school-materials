using UnityEngine;

namespace Metroidvania.Animation
{
    public enum WeaponType
    {
        None,
        Sword,
        Dagger,
        Bow,
        Staff,
    }

    [CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon")]
    public sealed class WeaponData : ScriptableObject
    {
        public string WeaponName;
        public WeaponType Type;
        public float Damage;
        public float AttackSpeed;

        [Header("Animations")]
        public AnimationClip IdleClip;
        public AnimationClip AttackClip;
        public AnimationClip ReloadClip;
        public AnimationClip EquipClip;
    }
}
