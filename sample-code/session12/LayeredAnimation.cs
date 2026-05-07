using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using VContainer;

namespace Metroidvania.Animation
{
    public sealed class LayeredAnimation : MonoBehaviour
    {
        private Animator _animator;
        private LayeredAnimationSettings _settings;
        private MotionHandle _layerWeightHandle;
        private CancellationTokenSource _blendCancellation;

        [Inject]
        public void Construct(Animator animator, LayeredAnimationSettings settings)
        {
            _animator = animator;
            _settings = settings;
        }

        private void Start()
        {
            _animator.SetLayerWeight(_settings.UpperBodyLayerIndex, 1f);
        }

        public void PlayUpperBodyAction(string actionName)
        {
            _animator.Play(actionName, _settings.UpperBodyLayerIndex, 0f);
        }

        public UniTask SetAimingAsync(bool isAiming)
        {
            float targetWeight = isAiming ? 1f : 0f;
            return BlendLayerWeightAsync(_settings.AimingLayerIndex, targetWeight);
        }

        private async UniTask BlendLayerWeightAsync(int layerIndex, float targetWeight)
        {
            CancelLayerBlend();
            _blendCancellation = new CancellationTokenSource();

            float currentWeight = _animator.GetLayerWeight(layerIndex);
            _layerWeightHandle = LMotion.Create(currentWeight, targetWeight, _settings.BlendDuration)
                .WithEase(Ease.OutQuad)
                .WithOnUpdate(value => _animator.SetLayerWeight(layerIndex, value))
                .RunWithoutBinding();

            await _layerWeightHandle.ToUniTask(_blendCancellation.Token);
        }

        private void CancelLayerBlend()
        {
            if (_layerWeightHandle.IsActive())
            {
                _layerWeightHandle.Cancel();
            }

            _blendCancellation?.Cancel();
            _blendCancellation?.Dispose();
            _blendCancellation = null;
        }

        private void OnDisable()
        {
            CancelLayerBlend();
        }
    }

    public sealed class LayeredAnimationSettings
    {
        public int UpperBodyLayerIndex { get; }
        public int AimingLayerIndex { get; }
        public float BlendDuration { get; }

        public LayeredAnimationSettings(int upperBodyLayerIndex, int aimingLayerIndex, float blendDuration)
        {
            UpperBodyLayerIndex = upperBodyLayerIndex;
            AimingLayerIndex = aimingLayerIndex;
            BlendDuration = blendDuration;
        }
    }
}
