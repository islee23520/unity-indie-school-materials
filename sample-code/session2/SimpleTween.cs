using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace Metroidvania.Session2
{
    /// <summary>
    /// Session 2: Animation & Tweening
    /// Demonstrates: LitMotion
    /// </summary>
    public class SimpleTween : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _duration = 1f;

        public void AnimateScale()
        {
            // Simple scale tween using LitMotion
            LMotion.Create(Vector3.one, Vector3.one * 1.5f, _duration)
                .WithEase(Ease.OutBack)
                .BindToLocalScale(_target);
        }

        public void AnimateColor(SpriteRenderer spriteRenderer)
        {
            // Color tween example
            LMotion.Create(Color.white, Color.red, _duration)
                .WithLoops(2, LoopType.Yoyo)
                .BindToColor(spriteRenderer);
        }
    }
}
