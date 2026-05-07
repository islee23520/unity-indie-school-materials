using UnityEngine;
using VContainer;

namespace Metroidvania.Animation
{
    public sealed class FootIK : MonoBehaviour
    {
        private Animator _animator;
        private FootIKSettings _settings;

        [Inject]
        public void Construct(Animator animator, FootIKSettings settings)
        {
            _animator = animator;
            _settings = settings;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            AdjustFootIK(AvatarIKGoal.LeftFoot, AvatarIKHint.LeftKnee);
            AdjustFootIK(AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee);
        }

        private void AdjustFootIK(AvatarIKGoal foot, AvatarIKHint knee)
        {
            Vector3 footPosition = _animator.GetIKPosition(foot);
            Vector3 rayOrigin = footPosition + Vector3.up;

            if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, _settings.RayDistance, _settings.GroundLayer))
            {
                _animator.SetIKPositionWeight(foot, 0f);
                _animator.SetIKRotationWeight(foot, 0f);
                _animator.SetIKHintPositionWeight(knee, 0f);
                return;
            }

            Vector3 targetPosition = hit.point + Vector3.up * _settings.FootOffset;
            Quaternion targetRotation = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(transform.forward, hit.normal),
                hit.normal);

            _animator.SetIKPosition(foot, targetPosition);
            _animator.SetIKRotation(foot, targetRotation);
            _animator.SetIKPositionWeight(foot, 1f);
            _animator.SetIKRotationWeight(foot, 1f);
            _animator.SetIKHintPositionWeight(knee, 0.5f);
        }
    }

    public sealed class FootIKSettings
    {
        public float RayDistance { get; }
        public LayerMask GroundLayer { get; }
        public float FootOffset { get; }

        public FootIKSettings(float rayDistance, LayerMask groundLayer, float footOffset)
        {
            RayDistance = rayDistance;
            GroundLayer = groundLayer;
            FootOffset = footOffset;
        }
    }
}
