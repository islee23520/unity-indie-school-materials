using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace Metroidvania.Menu
{
    public sealed class MenuManager : IStartable, System.IDisposable
    {
        private readonly UIDocument _uiDocument;
        private readonly IReadOnlyList<IMenuScreen> _menuScreens;
        private readonly ScreenTransition _defaultTransition;
        private readonly MenuViewModel _viewModel;
        private readonly Dictionary<string, IMenuScreen> _lookup = new();
        private readonly DisposableBag _disposables = new();

        private VisualElement _root;
        private IMenuScreen _currentScreen;

        [Inject]
        public MenuManager(
            UIDocument uiDocument,
            IReadOnlyList<IMenuScreen> menuScreens,
            ScreenTransition defaultTransition,
            MenuViewModel viewModel)
        {
            _uiDocument = uiDocument;
            _menuScreens = menuScreens;
            _defaultTransition = defaultTransition;
            _viewModel = viewModel;
        }

        public void Start()
        {
            _root = _uiDocument.rootVisualElement;

            foreach (IMenuScreen screen in _menuScreens)
            {
                VisualElement content = _root.Q<VisualElement>(screen.ContentElementName);
                screen.Bind(content);
                screen.Hide();
                _lookup[screen.ScreenName] = screen;
            }

            _viewModel.CurrentScreenName
                .Subscribe(screenName => OpenScreenAsync(screenName).Forget())
                .AddTo(ref _disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private async UniTask OpenScreenAsync(string screenName)
        {
            if (string.IsNullOrWhiteSpace(screenName))
            {
                return;
            }

            if (!_lookup.TryGetValue(screenName, out IMenuScreen target))
            {
                Debug.LogWarning($"Menu screen '{screenName}' was not registered.");
                return;
            }

            if (_currentScreen == target)
            {
                return;
            }

            if (_currentScreen != null)
            {
                await _defaultTransition.AnimateOutAsync(_currentScreen.Content);
                _currentScreen.Hide();
            }

            _currentScreen = target;
            _currentScreen.Show();
            await _defaultTransition.AnimateInAsync(_currentScreen.Content);
        }
    }
}
