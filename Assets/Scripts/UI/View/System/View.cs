using System;
using Aftertime.SecretSome.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Aftertime.SecretSome
{
#if UNITY_EDITOR
    [RequireComponent(typeof(CanvasGroup))]
#endif
    public abstract class View : MonoBehaviour
    {
        public bool IsVisible => _canvasGroup.alpha > 0;
        public bool IsOn => _canvasGroup.interactable;
        public Selectable LastSelected { get; protected set; }
        public event Action onReset = delegate { };

        protected CanvasGroup _canvasGroup;
        protected Navigation _activeNavigation;
        protected Navigation _inActiveNavigation;


        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        public void FadeCanvasGroup(float value, float duration, Action onCompleted = null)
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.DOKill();
            _canvasGroup.DOFade(value, duration)
                .SetUpdate(true) // Run tween updates in unscaled time
                .onComplete += () => onCompleted?.Invoke();
        }

        public void SetCanvasGroupAlpha(float value)
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.DOKill();
            _canvasGroup.alpha = value;
        }

        public virtual async void SetInteractable(bool enable)
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.interactable = enable;
        }

        public void SetBlocksRaycasts(bool enable)
        {
            if (_canvasGroup == null)
                return;

            _canvasGroup.blocksRaycasts = enable;
        }
    }
}
