using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aftertime.SecretSome.UI
{
    public class ToggleSwitch : ExtendedToggle
    {
        [Header("[ToggleSwitch]")] 
        [SerializeField] private Image _switchImage;
        [SerializeField] private Color _onColor;
        [SerializeField] private Color _offColor;
        [SerializeField] private float _onPosX;
        [SerializeField] private float _offPosX;
        

        public override void OnSelect()
        {
            if (IsOn)
                return;
            
            base.OnSelect();

            _textMeshPro.text = "ON";
            
            _switchImage.DOKill();
            _switchImage.rectTransform.DOKill();
            
            _switchImage.DOColor(_onColor, _colorDuration).SetUpdate(true);;
            _switchImage.rectTransform.DOAnchorPosX(_onPosX, _colorDuration).SetUpdate(true);;
        }

        public override void OnDeselect()
        {
            base.OnDeselect();
            
            _textMeshPro.text = "OFF";
            
            _switchImage.DOKill();
            _switchImage.rectTransform.DOKill();
            
            _switchImage.DOColor(_offColor, _colorDuration).SetUpdate(true);;
            _switchImage.rectTransform.DOAnchorPosX(_offPosX, _colorDuration).SetUpdate(true);;
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (UIInputAction.Instance.IsAnyMousePressed())
                return;
            
            base.OnPointerEnter(eventData);
        }
    }
   
}
