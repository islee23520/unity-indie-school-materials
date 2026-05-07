using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Metroidvania.Menu
{
    public sealed class MenuViewModel : IDisposable
    {
        private readonly Stack<string> _history = new();
        private readonly ReactiveProperty<int> _historyCount = new(0);
        private readonly CompositeDisposable _disposables = new();

        public ReactiveProperty<string> CurrentScreenName { get; } = new(string.Empty);
        public ReadOnlyReactiveProperty<bool> CanGoBack { get; }

        public MenuViewModel()
        {
            CanGoBack = _historyCount
                .Select(count => count > 0)
                .ToReadOnlyReactiveProperty();

            _historyCount.AddTo(_disposables);
            CurrentScreenName.AddTo(_disposables);
            CanGoBack.AddTo(_disposables);
        }

        public void RequestOpen(string screenName)
        {
            if (string.IsNullOrWhiteSpace(screenName))
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(CurrentScreenName.Value))
            {
                _history.Push(CurrentScreenName.Value);
                _historyCount.Value = _history.Count;
            }

            CurrentScreenName.Value = screenName;
        }

        public void RequestBack()
        {
            if (_history.Count == 0)
            {
                return;
            }

            CurrentScreenName.Value = _history.Pop();
            _historyCount.Value = _history.Count;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
