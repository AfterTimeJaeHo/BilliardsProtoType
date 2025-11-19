using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aftertime.SecretSome.UI
{
    public class DropdownToggle : Toggle
    {
        [Header("[Dropdown]")]
        [SerializeField] protected TextMeshProUGUI _text;
        [SerializeField] protected Image _image;
        [SerializeField] protected Image _offImage;
        [SerializeField] private Color _onTextColor;
        [SerializeField] private Color _onImageColor;
        [SerializeField] private Color _offTextColor;
        [SerializeField] private Color _offImageColor;
        [SerializeField] private float _fadeDuration = 0.15f;

        private bool _isStarted;
        
        protected override void Start()
        {
            base.Start();
            _isStarted = true;
        }

        public void OnValueChanged(bool value)
        {
            if (_isStarted)
                return;
            
            Color textColor = value ? _onTextColor : _offTextColor;
            Color imageColor = value ? _onImageColor : _offImageColor;

            _text.DOKill();
            _image.DOKill();
            
            _text.DOColor(textColor, _fadeDuration);
            _image.DOColor(imageColor, _fadeDuration);
        }
    }
   
}
