using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Metroidvania.Animation
{
    public sealed class AnimationLifetimeScope : LifetimeScope
    {
        [Header("Scene References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _headTrackingTarget;

        [Header("Animator Override")]
        [SerializeField] private RuntimeAnimatorController _characterBaseController;
        [SerializeField] private WeaponData[] _weapons;

        [Header("IK Settings")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _footRayDistance = 1f;
        [SerializeField] private float _footOffset = 0.1f;
        [SerializeField] private float _headRotationDuration = 0.12f;
        [SerializeField] private float _maxHeadRotationAngle = 60f;
        [SerializeField] private float _headRetargetThreshold = 0.5f;

        [Header("Blending Settings")]
        [SerializeField] private string _locomotionBlendParameter = "LocomotionBlend";
        [SerializeField] private float _locomotionDampTime = 0.1f;
        [SerializeField] private int _upperBodyLayerIndex = 1;
        [SerializeField] private int _aimingLayerIndex = 2;
        [SerializeField] private float _layerBlendDuration = 0.2f;

        [Header("Animation Systems")]
        [SerializeField] private CharacterSkinChanger _characterSkinChanger;
        [SerializeField] private WeaponAnimationSystem _weaponAnimationSystem;
        [SerializeField] private BoneController _boneController;
        [SerializeField] private HeadTracking _headTracking;
        [SerializeField] private FootIK _footIK;
        [SerializeField] private AnimationBlending _animationBlending;
        [SerializeField] private LayeredAnimation _layeredAnimation;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_animator);
            builder.RegisterInstance(_headTrackingTarget);
            builder.RegisterInstance(new CharacterSkinSettings(_characterBaseController));
            builder.RegisterInstance(new WeaponAnimationSettings(_weapons));
            builder.RegisterInstance(new HeadTrackingSettings(
                _headRotationDuration,
                _maxHeadRotationAngle,
                _headRetargetThreshold));
            builder.RegisterInstance(new FootIKSettings(_footRayDistance, _groundLayer, _footOffset));
            builder.RegisterInstance(new AnimationBlendingSettings(_locomotionBlendParameter, _locomotionDampTime));
            builder.RegisterInstance(new LayeredAnimationSettings(
                _upperBodyLayerIndex,
                _aimingLayerIndex,
                _layerBlendDuration));
            builder.Register<AnimationInputSource>(Lifetime.Singleton);

            builder.RegisterComponent(_characterSkinChanger);
            builder.RegisterComponent(_weaponAnimationSystem);
            builder.RegisterComponent(_boneController);
            builder.RegisterComponent(_headTracking);
            builder.RegisterComponent(_footIK);
            builder.RegisterComponent(_animationBlending);
            builder.RegisterComponent(_layeredAnimation);
        }
    }
}
