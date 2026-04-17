using System;
using VContainer;
using VContainer.Unity;

namespace Metroidvania.Session9
{
    public sealed class AbilityLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var unlockedByDefault = new[]
            {
                AbilityType.DoubleJump,
            };

            builder.RegisterInstance(unlockedByDefault);
            builder.Register<AbilityManager>(Lifetime.Singleton);
            builder.RegisterEntryPoint<AbilityUnlockPresenter>();
        }
    }

    public sealed class AbilityUnlockPresenter : IStartable, IDisposable
    {
        private readonly AbilityManager _abilityManager;
        private readonly DisposableBag _disposables = new();

        public AbilityUnlockPresenter(AbilityManager abilityManager)
        {
            _abilityManager = abilityManager;
        }

        public void Start()
        {
            _abilityManager.AbilityUnlocked.Subscribe(_ => { }).AddTo(ref _disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
