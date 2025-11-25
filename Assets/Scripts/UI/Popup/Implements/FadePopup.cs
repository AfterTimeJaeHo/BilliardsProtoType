using Aftertime.SecretSome.UI.Popup;
using UnityEngine;

namespace Waving.UI
{
    public class FadePopup : PopupBase
    {
        private FadePopupView _fadePopupView;

        protected override void Awake()
        {
            base.Awake();
            _fadePopupView = _view as FadePopupView;
        }
    }
   
}