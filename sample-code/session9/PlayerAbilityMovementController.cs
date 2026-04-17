using LitMotion;
using UnityEngine;

namespace Metroidvania.Session9
{
    public sealed class PlayerAbilityMovementController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _jumpForce = 12f;
        [SerializeField] private float _groundRadius = 0.15f;
        [SerializeField] private float _dashDistance = 5f;
        [SerializeField] private float _dashDuration = 0.3f;
        [SerializeField] private float _dashCooldown = 2f;

        private AbilityManager _abilityManager;
        private int _jumpCount;
        private bool _isDashing;
        private float _lastDashAt = -999f;

        public void Construct(AbilityManager abilityManager)
        {
            _abilityManager = abilityManager;
        }

        public void OnRelicCollected(AbilityType unlockedAbility)
        {
            if (_abilityManager == null)
            {
                return;
            }

            _abilityManager.Unlock(unlockedAbility);
        }

        private void Update()
        {
            if (_abilityManager == null)
            {
                return;
            }

            if (IsGrounded())
            {
                _jumpCount = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryJump();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                TryDash(Vector2.right);
            }
        }

        private void TryJump()
        {
            var maxJumpCount = _abilityManager.IsUnlocked(AbilityType.DoubleJump) ? 2 : 1;
            if (_jumpCount >= maxJumpCount)
            {
                return;
            }

            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
            _jumpCount++;
        }

        public bool TryDash(Vector2 direction)
        {
            if (!_abilityManager.IsUnlocked(AbilityType.Dash))
            {
                return false;
            }

            if (_isDashing)
            {
                return false;
            }

            if (Time.time - _lastDashAt < _dashCooldown)
            {
                return false;
            }

            var start = (Vector2)transform.position;
            var end = start + direction.normalized * _dashDistance;
            _isDashing = true;
            _lastDashAt = Time.time;

            LMotion.Create(start, end, _dashDuration)
                .Bind(value => transform.position = value)
                .AddTo(gameObject)
                .WithOnComplete(() => _isDashing = false);

            return true;
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(_groundCheck.position, _groundRadius, _groundMask);
        }
    }
}
