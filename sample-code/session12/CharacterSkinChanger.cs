using UnityEngine;

namespace Metroidvania.Animation
{
    public class CharacterSkinChanger : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private RuntimeAnimatorController baseController;
        
        private AnimatorOverrideController _overrideController;

        private void Start()
        {
            _overrideController = new AnimatorOverrideController(baseController);
            animator.runtimeAnimatorController = _overrideController;
        }

        public void ChangeSkin(AnimationClip idleClip, AnimationClip walkClip, AnimationClip attackClip)
        {
            _overrideController["Idle"] = idleClip;
            _overrideController["Walk"] = walkClip;
            _overrideController["Attack"] = attackClip;
            
            Debug.Log("Skin changed via Animator Override Controller");
        }
    }
}
