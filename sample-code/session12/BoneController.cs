using UnityEngine;
using VContainer;

namespace Metroidvania.Animation
{
    public sealed class BoneController : MonoBehaviour
    {
        private readonly int _headHash = Animator.StringToHash("Head");
        private readonly int _rightHandHash = Animator.StringToHash("RightHand");
        private Animator _animator;

        public Transform HeadBone { get; private set; }
        public Transform RightHandBone { get; private set; }
        public Transform SpineBone { get; private set; }
        public int HeadHash => _headHash;
        public int RightHandHash => _rightHandHash;

        [Inject]
        public void Construct(Animator animator)
        {
            _animator = animator;
        }

        private void Start()
        {
            CacheBones();
        }

        private void CacheBones()
        {
            HeadBone = _animator.GetBoneTransform(HumanBodyBones.Head);
            RightHandBone = _animator.GetBoneTransform(HumanBodyBones.RightHand);
            SpineBone = _animator.GetBoneTransform(HumanBodyBones.Spine);
        }
    }
}
