using System;
using Aftertime.SecretSome.UI;
using Aftertime.SecretSome.UI.Popup;
using UnityEngine;

namespace Waving.MyTinyStreamer.UI
{
    public class NoticePopup : PopupBase
    {
        public event Action onClickConfirm = delegate { };
        private NoticePopupView _noticePopupView;
        
        protected override void Awake()
        {
            base.Awake();
            _noticePopupView = _view as NoticePopupView;

            RegisterEvent();
        }

        private void RegisterEvent()
        {
            ExtendedButton confirmButton = _noticePopupView.ConfirmButton;
            confirmButton.onClick.AddListener(() => PopupManager.Instance.Hide<NoticePopup>());
            confirmButton.onClick.AddListener(() => onClickConfirm());
            
            ExtendedButton cancelButton = _noticePopupView.ConfirmButton;
            cancelButton.onClick.AddListener(() => PopupManager.Instance.Hide<NoticePopup>());
            cancelButton.onClick.AddListener(() => onClickConfirm());
        }

        public void ShowMessage(string message,Action onClickAction)
        {
            _noticePopupView.MessageText.text = message;
            onClickConfirm = null;
            onClickConfirm += onClickAction;
        }
    }
   
}