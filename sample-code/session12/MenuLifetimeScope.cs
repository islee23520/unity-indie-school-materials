using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace Metroidvania.Menu
{
    public sealed class MenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private ScreenTransition defaultTransition;
        [SerializeField] private MenuScreenDefinition[] menuScreens;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(uiDocument);
            builder.RegisterInstance(defaultTransition);

            foreach (MenuScreenDefinition screen in menuScreens)
            {
                builder.RegisterInstance(screen).As<IMenuScreen>();
            }

            builder.RegisterInstance<IReadOnlyList<IMenuScreen>>(menuScreens);
            builder.Register<MenuViewModel>(Lifetime.Singleton);
            builder.RegisterEntryPoint<MenuManager>();
        }
    }
}
