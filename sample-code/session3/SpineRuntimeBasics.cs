using Spine;
using Spine.Unity;
using UnityEngine;

namespace Metroidvania.Session3
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SpineRuntimeBasics : MonoBehaviour
    {
        [SpineAnimation] [SerializeField] private string _idleAnimation = "idle";
        [SpineAnimation] [SerializeField] private string _runAnimation = "run";
        [SpineAnimation] [SerializeField] private string _attackAnimation = "attack";

        private SkeletonAnimation _skeletonAnimation;
        private AnimationState _animationState;

        private const int BaseTrack = 0;
        private const int ActionTrack = 1;
        private const string SpineboyExample = "Spineboy";

        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _animationState = _skeletonAnimation.AnimationState;
        }

        private void Start()
        {
            _animationState.SetAnimation(BaseTrack, _idleAnimation, true);
            Debug.Log($"Spine runtime sample target: {SpineboyExample}");

            _animationState.Start += OnAnimationStart;
            _animationState.Complete += OnAnimationComplete;
            _animationState.Event += OnSpineEvent;

            ConfigureMixDurations();
        }

        private void OnDestroy()
        {
            if (_animationState == null)
            {
                return;
            }

            _animationState.Start -= OnAnimationStart;
            _animationState.Complete -= OnAnimationComplete;
            _animationState.Event -= OnSpineEvent;
        }

        public void PlayLocomotion(bool isMoving)
        {
            string target = isMoving ? _runAnimation : _idleAnimation;
            _animationState.SetAnimation(BaseTrack, target, true);
        }

        public void PlayAttackOnce()
        {
            _animationState.SetAnimation(ActionTrack, _attackAnimation, false);
            _animationState.AddAnimation(ActionTrack, _idleAnimation, true, 0f);
        }

        private void ConfigureMixDurations()
        {
            _animationState.Data.DefaultMix = 0.2f;
            _animationState.Data.SetMix(_idleAnimation, _runAnimation, 0.15f);
            _animationState.Data.SetMix(_runAnimation, _idleAnimation, 0.2f);
            _animationState.Data.SetMix(_runAnimation, _attackAnimation, 0.08f);
            _animationState.Data.SetMix(_attackAnimation, _idleAnimation, 0.22f);
        }

        private void OnAnimationStart(TrackEntry entry)
        {
            Debug.Log($"Spine AnimationState.Start => {entry.Animation.Name}");
        }

        private void OnAnimationComplete(TrackEntry entry)
        {
            Debug.Log($"Spine AnimationState.Complete => {entry.Animation.Name}");
        }

        private void OnSpineEvent(TrackEntry entry, Event e)
        {
            Debug.Log($"Spine Event => {e.Data.Name}");
        }
    }
}
