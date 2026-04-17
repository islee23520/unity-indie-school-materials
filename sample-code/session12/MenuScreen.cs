using UnityEngine;

namespace Metroidvania.Menu
{
    public abstract class MenuScreen : MonoBehaviour
    {
        [SerializeField] protected RectTransform content;
        
        public RectTransform Content => content;

        public virtual void Show()
        {
            content.gameObject.SetActive(true);
            OnShow();
        }

        public virtual void Hide()
        {
            OnHide();
            content.gameObject.SetActive(false);
        }

        protected abstract void OnShow();
        protected abstract void OnHide();
    }
}
