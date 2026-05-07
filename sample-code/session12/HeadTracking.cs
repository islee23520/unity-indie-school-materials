using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using VContainer;

namespace Metroidvania.Animation
{
    public sealed class HeadTracking : MonoBehaviour
    {
        private Transform _target;
        private HeadTrackingSettings _settings;
        private Transform _headBone;
        private Quaternion _initialRotation;
        private Quaternion _lastTargetRotation;
        private MotionHandle _rotationHandle;

        [Inject]
        public void Construct(Animator animator, Transform target, HeadTrackingSettings settings)
        {
            _target = target;
            _settings = settings;
            _headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            _initialRotation = _headBone.rotation;
            _lastTargetRotation = _initialRotation;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_target == null || _headBone == null)
            {
                return;
            }

            Vector3 direction = _target.position - _headBone.position;
            direction.y = 0f;

            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            angle = Mathf.Clamp(angle, -_settings.MaxRotationAngle, _settings.MaxRotationAngle);

            Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f) * _initialRotation;
            if (Quaternion.Angle(_lastTargetRotation, targetRotation) < _settings.RetargetThreshold)
            {
                return;
            }

            if (_rotationHandle.IsActive())
            {
                _rotationHandle.Cancel();
            }

            _lastTargetRotation = targetRotation;
            _rotationHandle = LMotion.Create(_headBone.rotation, targetRotation, _settings.RotationDuration)
                .WithEase(Ease.OutQuad)
                .WithOnUpdate(value => _headBone.rotation = value)
                .RunWithoutBinding();
        }

        private void OnDisable()
        {
            if (_rotationHandle.IsActive())
            {
                _rotationHandle.Cancel();
            }
        }
    }

    public sealed class HeadTrackingSettings
    {
        public float RotationDuration { get; }
        public float MaxRotationAngle { get; }
        public float RetargetThreshold { get; }

        public HeadTrackingSettings(float rotationDuration, float maxRotationAngle, float retargetThreshold)
        {
            RotationDuration = rotationDuration;
            MaxRotationAngle = maxRotationAngle;
            RetargetThreshold = retargetThreshold;
        }
    }
}
