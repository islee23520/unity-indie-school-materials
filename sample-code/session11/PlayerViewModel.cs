using R3;
using UnityEngine;

namespace Metroidvania.UI
{
    public class PlayerViewModel
    {
        public ReactiveProperty<string> PlayerName { get; } = new("Hero");
        public ReactiveProperty<int> CurrentHP { get; } = new(100);
        public ReactiveProperty<int> MaxHP { get; } = new(100);

        public ReadOnlyReactiveProperty<float> HPPercent => 
            CurrentHP.CombineLatest(MaxHP, (cur, max) => (float)cur / max)
                .ToReadOnlyReactiveProperty();

        public ReadOnlyReactiveProperty<string> HPText => 
            CurrentHP.CombineLatest(MaxHP, (cur, max) => $"{cur} / {max}")
                .ToReadOnlyReactiveProperty();

        public void TakeDamage(int amount)
        {
            CurrentHP.Value = Mathf.Max(0, CurrentHP.Value - amount);
        }
    }
}
