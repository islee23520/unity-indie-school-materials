using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Metroidvania.Session4
{
    public sealed class InputActionR3Adapter : MonoBehaviour
    {
        [SerializeField] private InputActionReference _moveActionReference;
        [SerializeField] private InputActionReference _jumpActionReference;

        private InputAction _moveAction;
        private InputAction _jumpAction;

        private readonly ReactiveProperty<Vector2> _moveInput = new(Vector2.zero);
        private readonly ReactiveProperty<bool> _jumpPressed = new(false);

        public ReadOnlyReactiveProperty<Vector2> MoveInput => _moveInput;
        public ReadOnlyReactiveProperty<bool> JumpPressed => _jumpPressed;

        private void Awake()
        {
            _moveAction = _moveActionReference.action;
            _jumpAction = _jumpActionReference.action;
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _jumpAction.Enable();

            _moveAction.performed += OnMovePerformed;
            _moveAction.canceled += OnMoveCanceled;
            _jumpAction.started += OnJumpStarted;
            _jumpAction.canceled += OnJumpCanceled;
        }

        private void OnDisable()
        {
            _moveAction.performed -= OnMovePerformed;
            _moveAction.canceled -= OnMoveCanceled;
            _jumpAction.started -= OnJumpStarted;
            _jumpAction.canceled -= OnJumpCanceled;

            _moveAction.Disable();
            _jumpAction.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _moveInput.Value = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _moveInput.Value = Vector2.zero;
        }

        private void OnJumpStarted(InputAction.CallbackContext context)
        {
            _jumpPressed.Value = true;
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            _jumpPressed.Value = false;
        }
    }
}
