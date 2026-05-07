using System;
using R3;
using UnityEngine;
using VContainer;

namespace Metroidvania.Animation
{
    public sealed class WeaponAnimationSystem : MonoBehaviour, IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private Animator _animator;
        private WeaponAnimationSettings _settings;
        private AnimatorOverrideController _overrideController;

        public ReactiveProperty<WeaponType> CurrentWeapon { get; } = new(WeaponType.None);

        [Inject]
        public void Construct(Animator animator, WeaponAnimationSettings settings)
        {
            _animator = animator;
            _settings = settings;
        }

        private void Start()
        {
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;

            CurrentWeapon
                .Where(type => type != WeaponType.None)
                .Subscribe(ApplyWeaponAnimation)
                .AddTo(_disposables);
        }

        public void EquipWeapon(WeaponType type)
        {
            CurrentWeapon.Value = type;
        }

        private void ApplyWeaponAnimation(WeaponType type)
        {
            WeaponData weapon = Array.Find(_settings.Weapons, item => item.Type == type);
            if (weapon == null)
            {
                return;
            }

            _overrideController["Attack"] = weapon.AttackClip;
            _overrideController["Idle_Combat"] = weapon.IdleClip;
            _overrideController["Reload"] = weapon.ReloadClip;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            CurrentWeapon.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }

    public sealed class WeaponAnimationSettings
    {
        public WeaponData[] Weapons { get; }

        public WeaponAnimationSettings(WeaponData[] weapons)
        {
            Weapons = weapons;
        }
    }
}
