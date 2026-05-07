using UnityEngine;
using VContainer;

namespace Metroidvania.Animation
{
    public sealed class CharacterSkinChanger : MonoBehaviour
    {
        private Animator _animator;
        private CharacterSkinSettings _settings;
        private AnimatorOverrideController _overrideController;

        [Inject]
        public void Construct(Animator animator, CharacterSkinSettings settings)
        {
            _animator = animator;
            _settings = settings;
        }

        private void Start()
        {
            _overrideController = new AnimatorOverrideController(_settings.BaseController);
            _animator.runtimeAnimatorController = _overrideController;
        }

        public void ChangeSkin(AnimationClip idleClip, AnimationClip walkClip, AnimationClip attackClip)
        {
            _overrideController["Idle"] = idleClip;
            _overrideController["Walk"] = walkClip;
            _overrideController["Attack"] = attackClip;
        }
    }

    public sealed class CharacterSkinSettings
    {
        public RuntimeAnimatorController BaseController { get; }

        public CharacterSkinSettings(RuntimeAnimatorController baseController)
        {
            BaseController = baseController;
        }
    }
}
