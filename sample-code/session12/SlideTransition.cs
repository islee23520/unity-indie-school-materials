using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Metroidvania.Menu
{
    public enum SlideDirection
    {
        Left,
        Right
    }

    [CreateAssetMenu(fileName = "SlideTransition", menuName = "Menu/Slide Transition")]
    public sealed class SlideTransition : ScreenTransition
    {
        [SerializeField] private SlideDirection direction = SlideDirection.Right;

        public override async UniTask AnimateInAsync(VisualElement screen)
        {
            screen.style.display = DisplayStyle.Flex;
            screen.style.translate = new Translate(GetStartOffset(), 0);

            await LMotion.Create(GetStartOffset(), 0f, duration)
                .WithEase(Ease.OutQuad)
                .BindWithState(screen, (value, element) => element.style.translate = new Translate(value, 0))
                .ToUniTask();
        }

        public override async UniTask AnimateOutAsync(VisualElement screen)
        {
            await LMotion.Create(0f, GetStartOffset(), duration)
                .WithEase(Ease.OutQuad)
                .BindWithState(screen, (value, element) => element.style.translate = new Translate(value, 0))
                .ToUniTask();

            screen.style.display = DisplayStyle.None;
            screen.style.translate = new Translate(0, 0);
        }

        private float GetStartOffset()
        {
            return direction switch
            {
                SlideDirection.Left => -640f,
                SlideDirection.Right => 640f,
                _ => 0f
            };
        }
    }
}
