using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Metroidvania.Menu
{
    [CreateAssetMenu(fileName = "FadeTransition", menuName = "Menu/Fade Transition")]
    public class FadeTransition : ScreenTransition
    {
        public override async UniTask AnimateInAsync(VisualElement screen)
        {
            screen.style.display = DisplayStyle.Flex;
            screen.style.opacity = 0f;

            await LMotion.Create(0f, 1f, duration)
                .WithEase(Ease.OutQuad)
                .BindWithState(screen, (value, element) => element.style.opacity = value)
                .ToUniTask();
        }

        public override async UniTask AnimateOutAsync(VisualElement screen)
        {
            await LMotion.Create(1f, 0f, duration)
                .WithEase(Ease.OutQuad)
                .BindWithState(screen, (value, element) => element.style.opacity = value)
                .ToUniTask();

            screen.style.display = DisplayStyle.None;
        }
    }
}
