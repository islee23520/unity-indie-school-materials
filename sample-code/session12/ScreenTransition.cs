using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Metroidvania.Menu
{
    public abstract class ScreenTransition : ScriptableObject
    {
        [SerializeField] protected float duration = 0.5f;

        public abstract UniTask AnimateInAsync(VisualElement screen);
        public abstract UniTask AnimateOutAsync(VisualElement screen);
    }
}
