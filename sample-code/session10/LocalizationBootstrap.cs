using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Metroidvania.Session10
{
    public interface ILocalizationService
    {
        ReadOnlyReactiveProperty<string> CurrentLocaleCode { get; }
        UniTask ChangeLocaleAsync(string localeCode);
    }

    public sealed class LocalizationService : ILocalizationService
    {
        private readonly ReactiveProperty<string> _currentLocaleCode = new("ko");

        public ReadOnlyReactiveProperty<string> CurrentLocaleCode => _currentLocaleCode;

        public async UniTask ChangeLocaleAsync(string localeCode)
        {
            await LocalizationSettings.InitializationOperation.ToUniTask();

            var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            if (locale == null)
            {
                return;
            }

            LocalizationSettings.SelectedLocale = locale;
            _currentLocaleCode.Value = locale.Identifier.Code;
        }
    }

    public sealed class LocalizationBootstrap : MonoBehaviour
    {
        private ISaveService _saveService;
        private ILocalizationService _localizationService;

        public async UniTask InitializeAsync(ISaveService saveService, ILocalizationService localizationService)
        {
            _saveService = saveService;
            _localizationService = localizationService;

            var saveData = _saveService.Load();
            await _localizationService.ChangeLocaleAsync(saveData.LocaleCode);
        }

        public async UniTask ChangeLocaleAndSaveAsync(string localeCode)
        {
            await _localizationService.ChangeLocaleAsync(localeCode);

            var saveData = _saveService.Load();
            saveData.LocaleCode = localeCode;
            _saveService.Save(saveData);
        }

        public Locale[] GetAvailableLocales()
        {
            return LocalizationSettings.AvailableLocales.Locales.ToArray();
        }
    }
}
