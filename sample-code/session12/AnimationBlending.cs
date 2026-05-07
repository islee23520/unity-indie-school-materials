using System;
using R3;
using UnityEngine;
using VContainer;

namespace Metroidvania.Animation
{
    public sealed class AnimationBlending : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();
        private Animator _animator;
        private AnimationInputSource _inputSource;
        private AnimationBlendingSettings _settings;
        private int _locomotionBlendHash;

        public ReactiveProperty<float> LocomotionSpeed { get; } = new(0f);

        [Inject]
        public void Construct(Animator animator, AnimationInputSource inputSource, AnimationBlendingSettings settings)
        {
            _animator = animator;
            _inputSource = inputSource;
            _settings = settings;
        }

        private void Start()
        {
            _locomotionBlendHash = Animator.StringToHash(_settings.LocomotionBlendParameter);

            LocomotionSpeed
                .Subscribe(UpdateLocomotionBlend)
                .AddTo(_disposables);

            _inputSource.MoveInput
                .Select(moveInput => Mathf.Clamp01(moveInput.magnitude))
                .Subscribe(speed => LocomotionSpeed.Value = speed)
                .AddTo(_disposables);
        }

        public void SetMoveInput(Vector2 moveInput)
        {
            _inputSource.MoveInput.Value = moveInput;
        }

        private void UpdateLocomotionBlend(float speed)
        {
            _animator.SetFloat(_locomotionBlendHash, speed, _settings.DampTime, Time.deltaTime);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            LocomotionSpeed.Dispose();
        }
    }

    public sealed class AnimationInputSource : IDisposable
    {
        public ReactiveProperty<Vector2> MoveInput { get; } = new(Vector2.zero);

        public void Dispose()
        {
            MoveInput.Dispose();
        }
    }

    public sealed class AnimationBlendingSettings
    {
        public string LocomotionBlendParameter { get; }
        public float DampTime { get; }

        public AnimationBlendingSettings(string locomotionBlendParameter, float dampTime)
        {
            LocomotionBlendParameter = locomotionBlendParameter;
            DampTime = dampTime;
        }
    }
}
