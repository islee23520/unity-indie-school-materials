using System;
using System.Collections.Generic;
using UnityEngine;

namespace Metroidvania.Menu
{
    public class ScreenManager : MonoBehaviour
    {
        public static ScreenManager Instance { get; private set; }

        [SerializeField] private MenuScreen[] menuScreens;
        [SerializeField] private ScreenTransition defaultTransition;

        private Stack<MenuScreen> _history = new();
        private MenuScreen _currentScreen;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void OpenScreen(string screenName)
        {
            MenuScreen target = Array.Find(menuScreens, s => s.name == screenName);
            if (target == null) return;

            if (_currentScreen != null)
            {
                _currentScreen.Hide();
                _history.Push(_currentScreen);
            }

            _currentScreen = target;
            _currentScreen.Show();
        }

        public void Back()
        {
            if (_history.Count == 0) return;

            if (_currentScreen != null)
            {
                _currentScreen.Hide();
            }

            _currentScreen = _history.Pop();
            _currentScreen.Show();
        }
    }
}
