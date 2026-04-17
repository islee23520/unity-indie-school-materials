using UnityEngine;

namespace Metroidvania.Session2
{
    /// <summary>
    /// Session 2: Data Management
    /// Demonstrates: ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Metroidvania/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        public float MoveSpeed = 5f;
        public float JumpForce = 10f;

        [Header("Combat Settings")]
        public int MaxHealth = 100;
        public float AttackCooldown = 0.5f;
    }
}
