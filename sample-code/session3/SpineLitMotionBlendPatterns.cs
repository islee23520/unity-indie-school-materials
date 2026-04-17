using LitMotion;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Metroidvania.Session3
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class SpineLitMotionBlendPatterns : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation _skeletonAnimation;
        [SerializeField] private string _aimBoneName = "aim";
        [SerializeField] private float _flipDuration = 0.12f;

        private MotionHandle _flipHandle;

        private void Awake()
        {
            if (_skeletonAnimation == null)
            {
                _skeletonAnimation = GetComponent<SkeletonAnimation>();
            }
        }

        public void BlendFaceDirection(float moveX)
        {
            if (Mathf.Approximately(moveX, 0f))
            {
                return;
            }

            float targetScaleX = moveX > 0f ? 1f : -1f;

            if (_flipHandle.IsActive())
            {
                _flipHandle.Cancel();
            }

            _flipHandle = LMotion.Create(transform.localScale.x, targetScaleX, _flipDuration)
                .WithEase(Ease.OutQuad)
                .WithOnUpdate(x =>
                {
                    Vector3 scale = transform.localScale;
                    scale.x = x;
                    transform.localScale = scale;
                })
                .RunWithoutBinding();
        }

        public void BlendAimBoneWeight(float from, float to, float duration)
        {
            IkConstraint aimIk = _skeletonAnimation.Skeleton.FindIkConstraint(_aimBoneName);
            if (aimIk == null)
            {
                return;
            }

            LMotion.Create(from, to, duration)
                .WithEase(Ease.InOutQuad)
                .WithOnUpdate(weight => aimIk.Mix = weight)
                .RunWithoutBinding();
        }

        public void SetDamageFlash(Color hitColor)
        {
            Skeleton skeleton = _skeletonAnimation.Skeleton;
            Color original = skeleton.GetColor();

            LMotion.Create(original, hitColor, 0.05f)
                .WithEase(Ease.OutQuad)
                .WithOnUpdate(color => skeleton.SetColor(color))
                .RunWithoutBinding();

            LMotion.Create(hitColor, original, 0.18f)
                .WithDelay(0.05f)
                .WithEase(Ease.InOutQuad)
                .WithOnUpdate(color => skeleton.SetColor(color))
                .RunWithoutBinding();
        }
    }
}
