using UnityEngine;
using UnityEngine.UIElements;
using R3;
using LitMotion;
using LitMotion.Extensions;

namespace Metroidvania.UI
{
    /// <summary>
    /// UI Toolkit (UXML/USS) + MVVM + R3 Sample
    /// </summary>
    public class PlayerUIView : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        private PlayerViewModel _viewModel;
        private readonly CompositeDisposable _disposables = new();

        private VisualElement _hpFill;
        private Label _hpText;
        private Label _nameLabel;
        private Button _damageBtn;

        private void Start()
        {
            _viewModel = new PlayerViewModel();
            var root = uiDocument.rootVisualElement;

            _hpFill = root.Q<VisualElement>("hp-fill");
            _hpText = root.Q<Label>("hp-text");
            _nameLabel = root.Q<Label>("player-name");
            _damageBtn = root.Q<Button>("damage-btn");

            _viewModel.PlayerName
                .Subscribe(name => _nameLabel.text = name)
                .AddTo(_disposables);

            _viewModel.HPPercent
                .Subscribe(percent =>
                {
                    float targetWidth = percent * 100f;
                    LMotion.Create(_hpFill.style.width.value.value, targetWidth, 0.3f)
                        .WithEase(Ease.OutQuad)
                        .BindWithState(_hpFill, (v, element) => 
                        {
                            element.style.width = Length.Percent(v);
                        })
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);

            _viewModel.HPText
                .Subscribe(text => _hpText.text = text)
                .AddTo(_disposables);

            _damageBtn.clicked += () => _viewModel.TakeDamage(10);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
