using Cysharp.Threading.Tasks;
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
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                screen.style.opacity = Mathf.Clamp01(elapsed / duration);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            screen.style.opacity = 1f;
        }

        public override async UniTask AnimateOutAsync(VisualElement screen)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                screen.style.opacity = 1f - Mathf.Clamp01(elapsed / duration);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            screen.style.opacity = 0f;
            screen.style.display = DisplayStyle.None;
        }
    }
}
