using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Metroidvania.Menu
{
    public interface IMenuScreen
    {
        string ScreenName { get; }
        string ContentElementName { get; }
        VisualElement Content { get; }

        void Bind(VisualElement content);
        void Show();
        void Hide();
    }

    [Serializable]
    public sealed class MenuScreenDefinition : IMenuScreen
    {
        [SerializeField] private string screenName;
        [SerializeField] private string contentElementName;

        public string ScreenName => screenName;
        public string ContentElementName => contentElementName;
        public VisualElement Content { get; private set; }

        public void Bind(VisualElement content)
        {
            Content = content;
        }

        public void Show()
        {
            if (Content == null)
            {
                return;
            }

            Content.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            if (Content == null)
            {
                return;
            }

            Content.style.display = DisplayStyle.None;
        }
    }
}
