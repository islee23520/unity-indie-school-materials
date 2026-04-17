using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace Metroidvania.Session2
{
    /// <summary>
    /// Session 2: Architecture & DI
    /// Demonstrates: VContainer, DI setup
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private PlayerSettings _playerSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register ScriptableObject as a plain object
            builder.RegisterInstance(_playerSettings);

            // Register services/logic
            // builder.Register<IInputService, InputService>(Lifetime.Singleton);
            
            // Register entry points
            // builder.RegisterEntryPoint<GameManager>();
        }
    }
}
