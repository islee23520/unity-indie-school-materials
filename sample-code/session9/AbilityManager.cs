using System;
using System.Collections.Generic;
using R3;

namespace Metroidvania.Session9
{
    public enum AbilityType
    {
        DoubleJump,
        Dash,
        WallJump,
        DashAttack,
    }

    public sealed class AbilityManager : IDisposable
    {
        private readonly HashSet<AbilityType> _unlocked = new();
        private readonly Subject<AbilityType> _abilityUnlocked = new();

        public Observable<AbilityType> AbilityUnlocked => _abilityUnlocked;
        public ReactiveProperty<AbilityType?> CurrentAbility { get; } = new(null);

        public AbilityManager(IEnumerable<AbilityType> unlockedByDefault)
        {
            foreach (var ability in unlockedByDefault)
            {
                _unlocked.Add(ability);
            }
        }

        public bool IsUnlocked(AbilityType ability)
        {
            return _unlocked.Contains(ability);
        }

        public bool Unlock(AbilityType ability)
        {
            if (!_unlocked.Add(ability))
            {
                return false;
            }

            CurrentAbility.Value = ability;
            _abilityUnlocked.OnNext(ability);
            return true;
        }

        public void Dispose()
        {
            _abilityUnlocked.Dispose();
            CurrentAbility.Dispose();
        }
    }
}
