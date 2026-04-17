using UnityEngine;

namespace Metroidvania.Session1
{
    /// <summary>
    /// Session 1: C# & Unity Basics
    /// Demonstrates: class, Rigidbody2D, velocity
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        private Rigidbody2D _rb;

        private void Awake()
        {
            // Get the Rigidbody2D component attached to this GameObject
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Basic horizontal movement input
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            
            // Apply movement using velocity
            Move(horizontalInput);
        }

        private void Move(float direction)
        {
            // Setting the velocity directly for responsive platformer movement
            _rb.velocity = new Vector2(direction * _moveSpeed, _rb.velocity.y);
        }
    }
}
