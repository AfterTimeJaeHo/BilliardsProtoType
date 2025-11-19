using Aftertime.SecretSome.UI.Popup;
using Aftertime.TeachingAssistantLife.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Aftertime.SecretSome.UI.Page
{
#if UNITY_EDITOR
    [RequireComponent(typeof(PageView))]
#endif
    public class PageBase : MonoBehaviour
    {
        public int PageIndex => _pageIndex;
        public bool IsOn => _view.IsOn;

        [SerializeField] protected PopupBase _parentPopup;
        [SerializeField] private int _pageIndex;
        [SerializeField] protected float _fadeInDuration = 0.5f;
        [SerializeField] protected float _fadeOutDuration = 0.5f;
        protected PageView _view;

        protected virtual void Awake()
        {
            _view = GetComponent<PageView>();
        }

        public virtual void Show(bool hasDuration)
        {
            if (_view.IsOn)
                return;
            
            float duration = hasDuration ? _fadeInDuration : 0;

            _view.SetInteractable(true);
            _view.SetBlocksRaycasts(true);
            _view.FadeCanvasGroup(1, duration);
            PopupManager.Instance.PageStackMap.Push(_parentPopup, this);
        }

        public virtual void Hide(bool hasDuration)
        {
            if (_view.IsOn == false)
                return;
            
            float duration = hasDuration ? _fadeOutDuration : 0;

            _view.SetInteractable(false);
            _view.SetBlocksRaycasts(false);
            _view.FadeCanvasGroup(0, duration);
            PopupManager.Instance.PageStackMap.Pop(_parentPopup);
        }

        public virtual void Show(float duration)
        {
            float originDuration = _fadeInDuration;
            _fadeInDuration = duration;
            Show(true);
            _fadeInDuration = originDuration;
        }

        public virtual void Hide(float duration)
        {
            float originDuration = _fadeOutDuration;
            _fadeOutDuration = duration;
            Hide(true);
            _fadeOutDuration = originDuration;
        }

        public void ActivateLastSelectedObject()
        {
            if (UIInputAction.Instance.IsAnyMousePressed())
                return;
            
            if (_view.LastSelected != null)
                EventSystem.current.SetSelectedGameObject(_view.LastSelected.gameObject);
        }
    }
}
