using System.Collections;
using UnityEngine;

namespace Metroidvania.Menu
{
    [CreateAssetMenu(fileName = "FadeTransition", menuName = "Menu/Fade Transition")]
    public class FadeTransition : ScreenTransition
    {
        public override IEnumerator AnimateIn(RectTransform screen)
        {
            CanvasGroup group = screen.GetComponent<CanvasGroup>();
            if (group == null) group = screen.gameObject.AddComponent<CanvasGroup>();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                group.alpha = elapsed / duration;
                yield return null;
            }
            group.alpha = 1f;
        }

        public override IEnumerator AnimateOut(RectTransform screen)
        {
            CanvasGroup group = screen.GetComponent<CanvasGroup>();
            if (group == null) group = screen.gameObject.AddComponent<CanvasGroup>();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                group.alpha = 1f - (elapsed / duration);
                yield return null;
            }
            group.alpha = 0f;
        }
    }
}
