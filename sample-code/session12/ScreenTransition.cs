using System.Collections;
using UnityEngine;

namespace Metroidvania.Menu
{
    public abstract class ScreenTransition : ScriptableObject
    {
        [SerializeField] protected float duration = 0.5f;
        
        public abstract IEnumerator AnimateIn(RectTransform screen);
        public abstract IEnumerator AnimateOut(RectTransform screen);
    }
}
