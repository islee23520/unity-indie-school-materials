using UnityEngine;
using UnityEngine.UIElements;

namespace Metroidvania.Menu
{
    public abstract class MenuScreen : MonoBehaviour
    {
        [SerializeField] private string contentElementName;

        public VisualElement Content { get; private set; }

        public void Bind(VisualElement root)
        {
            Content = root.Q<VisualElement>(contentElementName);
            if (Content == null)
            {
                Debug.LogWarning($"UI Toolkit element '{contentElementName}' was not found.");
            }
        }

        public virtual void Show()
        {
            if (Content == null)
            {
                return;
            }

            Content.style.display = DisplayStyle.Flex;
            OnShow();
        }

        public virtual void Hide()
        {
            if (Content == null)
            {
                return;
            }

            OnHide();
            Content.style.display = DisplayStyle.None;
        }

        protected abstract void OnShow();
        protected abstract void OnHide();
    }
}
