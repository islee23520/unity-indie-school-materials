using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Metroidvania.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private MenuScreen[] menuScreens;
        [SerializeField] private ScreenTransition defaultTransition;

        private readonly Stack<MenuScreen> _history = new();
        private readonly Dictionary<string, MenuScreen> _lookup = new();
        private MenuScreen _currentScreen;
        private VisualElement _root;

        private void Awake()
        {
            _root = uiDocument.rootVisualElement;

            foreach (MenuScreen screen in menuScreens)
            {
                screen.Bind(_root);
                screen.Hide();
                _lookup[screen.name] = screen;
            }
        }

        public async UniTask OpenScreenAsync(string screenName)
        {
            if (!_lookup.TryGetValue(screenName, out MenuScreen target))
            {
                return;
            }

            if (_currentScreen != null)
            {
                await defaultTransition.AnimateOutAsync(_currentScreen.Content);
                _currentScreen.Hide();
                _history.Push(_currentScreen);
            }

            _currentScreen = target;
            _currentScreen.Show();
            await defaultTransition.AnimateInAsync(_currentScreen.Content);
        }

        public async UniTask BackAsync()
        {
            if (_history.Count == 0)
            {
                return;
            }

            if (_currentScreen != null)
            {
                await defaultTransition.AnimateOutAsync(_currentScreen.Content);
                _currentScreen.Hide();
            }

            _currentScreen = _history.Pop();
            _currentScreen.Show();
            await defaultTransition.AnimateInAsync(_currentScreen.Content);
        }
    }
}
