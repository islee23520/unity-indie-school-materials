using LitMotion;
using R3;
using UnityEngine;

namespace Metroidvania.Session4
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class ReactiveLitMotionMovement : MonoBehaviour
    {
        [SerializeField] private InputActionR3Adapter _input;
        [SerializeField] private float _maxSpeed = 7f;
        [SerializeField] private float _accelerationDuration = 0.12f;
        [SerializeField] private float _jumpForce = 12f;

        private readonly CompositeDisposable _disposables = new();
        private Rigidbody2D _rb;
        private MotionHandle _moveHandle;
        private float _smoothedHorizontal;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _input.MoveInput
                .Subscribe(OnMoveInputChanged)
                .AddTo(_disposables);

            _input.JumpPressed
                .Where(pressed => pressed)
                .Subscribe(_ => Jump())
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            if (_moveHandle.IsActive())
            {
                _moveHandle.Cancel();
            }

            _disposables.Clear();
        }

        private void FixedUpdate()
        {
            Vector2 velocity = _rb.velocity;
            velocity.x = _smoothedHorizontal * _maxSpeed;
            _rb.velocity = velocity;
        }

        private void OnMoveInputChanged(Vector2 move)
        {
            if (_moveHandle.IsActive())
            {
                _moveHandle.Cancel();
            }

            _moveHandle = LMotion.Create(_smoothedHorizontal, move.x, _accelerationDuration)
                .WithEase(Ease.OutQuad)
                .WithOnUpdate(value => _smoothedHorizontal = value)
                .RunWithoutBinding();
        }

        private void Jump()
        {
            Vector2 velocity = _rb.velocity;
            velocity.y = _jumpForce;
            _rb.velocity = velocity;
        }
    }
}
