using Aftertime.StorylineEngine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ToggleEvent = UnityEngine.UI.Toggle.ToggleEvent;

namespace Aftertime.SecretSome.UI
{
    public class ExtendedToggle : Selectable, IPointerClickHandler, ISubmitHandler
    {
        public bool IsClicked { get; private set; }
        public TextMeshProUGUI TextMeshPro => _textMeshPro;

        [SerializeField] protected TextMeshProUGUI _textMeshPro;

        [Header("[Option]")] [SerializeField] private bool _useObject;
        [SerializeField] private bool _useSpriteColor;
        [SerializeField] private bool _useText;
        [SerializeField] protected float _colorDuration;

        [Header("[Default]")] [SerializeField] protected GameObject _defaultObject;
        [Header("[Default]")] [SerializeField] protected Sprite _defaultSprite;
        [Header("[Default]")] [SerializeField] protected Color _defaultSpriteColor;
        [SerializeField] protected Color _defaultTextColor;

        [Header("[Enter]")] [SerializeField] protected GameObject _pointerEnterObject;
        [Header("[Enter]")] [SerializeField] protected Sprite _pointerEnterSprite;
        [Header("[Enter]")] [SerializeField] protected Color _pointerEnterSpriteColor;
        [SerializeField] protected Color _pointerEnterTextColor;

        [Header("[Press]")] [SerializeField] private GameObject _pressedObject;
        [Header("[Press]")] [SerializeField] private Sprite _pressedSprite;
        [Header("[Press]")] [SerializeField] protected Color _pressedSpriteColor;
        [SerializeField] private Color _pressedTextColor;

        [Header("[On]")] [SerializeField] protected GameObject _onObject;
        [Header("[On]")] [SerializeField] protected Sprite _onSprite;
        [Header("[On]")] [SerializeField] protected Color _onSpriteColor;
        [SerializeField] protected Color _onTextColor;

        [Header("[Off]")] [SerializeField] protected GameObject _offObject;
        [Header("[Off]")] [SerializeField] protected Sprite _offSprite;
        [Header("[Off]")] [SerializeField] protected Color _offSpriteColor;
        [SerializeField] protected Color _offTextColor;

        [Header("[Focus]")] [SerializeField] protected GameObject _focusBGObject;

        [Header("[Sound]")] [SerializeField] private AudioClip _onClip;
        [SerializeField] private AudioClip _offClip;

        public bool IsOn
        {
            get => isOn;
            set
            {
                if (value)
                {
                    OnSelect();
                    isOn = true;
                }
                else
                {
                    OnDeselect();
                    isOn = false;
                }
            }
        }

        private bool isOn;
        public bool IsPressed { get; protected set; }

        public ToggleEvent onValueChanged;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (isOn)
                return;

            ExtendedButtonType type = GetButtonType();
            switch (type)
            {
                case ExtendedButtonType.Object:
                    SetActiveDefaultObject(false);
                    SetActiveEnterObject(true);
                    break;
                case ExtendedButtonType.ObjectAndText:
                    SetActiveDefaultObjectAndText(false);
                    SetActiveEnterObjectAndText(true);
                    break;
                case ExtendedButtonType.Sprite:
                    SetActiveDefaultSprite(false);
                    SetActiveEnterSprite(true);
                    break;
                case ExtendedButtonType.SpriteAndText:
                    SetActiveDefaultSpriteAndText(false);
                    SetActiveEnterSpriteAndText(true);
                    break;
                case ExtendedButtonType.SpriteColor:
                    SetActiveDefaultSpriteColor(false);
                    SetActiveEnterSpriteColor(true);
                    break;
                case ExtendedButtonType.SpriteColorAndText:
                    SetActiveDefaultSpriteColorAndText(false);
                    SetActiveEnterSpriteColorAndText(true);
                    break;
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            IsPressed = false;

            if (isOn)
                return;

            ExtendedButtonType type = GetButtonType();
            switch (type)
            {
                case ExtendedButtonType.Object:
                    SetActiveEnterObject(false);
                    SetActivePressObject(false);
                    SetActiveDefaultObject(true);
                    break;
                case ExtendedButtonType.ObjectAndText:
                    SetActiveEnterObjectAndText(false);
                    SetActivePressObjectAndText(false);
                    SetActiveDefaultObjectAndText(true);
                    break;
                case ExtendedButtonType.Sprite:
                    SetActiveEnterSprite(false);
                    SetActiveDefaultSprite(true);
                    break;
                case ExtendedButtonType.SpriteAndText:
                    SetActiveEnterSpriteAndText(false);
                    SetActiveDefaultSpriteAndText(true);
                    break;
                case ExtendedButtonType.SpriteColor:
                    SetActiveEnterSpriteColor(false);
                    SetActiveDefaultSpriteColor(true);
                    break;
                case ExtendedButtonType.SpriteColorAndText:
                    SetActiveDefaultSpriteColorAndText(true);
                    break;
            }
        }

        public virtual void OnSelect()
        {
            isOn = true;

            ExtendedButtonType type = GetButtonType();
            switch (type)
            {
                case ExtendedButtonType.Object:
                    SetActiveDefaultObject(false);
                    SetActiveEnterObject(false);
                    SetActiveSelectObject(true);
                    break;
                case ExtendedButtonType.ObjectAndText:
                    SetActiveDefaultObjectAndText(false);
                    SetActiveEnterObjectAndText(false);
                    SetActiveSelectObjectAndText(true);
                    break;
                case ExtendedButtonType.Sprite:
                    if (_onSprite != null)
                        image.sprite = _onSprite;
                    break;
                case ExtendedButtonType.SpriteAndText:
                    SetActiveDefaultSpriteAndText(false);
                    SetActiveSelectSpriteAndText(true);
                    break;
                case ExtendedButtonType.SpriteColor:
                    SetActiveDefaultSpriteColor(false);
                    SetActiveSelectSpriteColor(true);
                    break;
                case ExtendedButtonType.SpriteColorAndText:
                    SetActiveDefaultSpriteColorAndText(false);
                    SetActiveSelectSpriteColorAndText(true);
                    break;
            }

            onValueChanged.Invoke(true);
        }

        public virtual void OnDeselect()
        {
            isOn = false;

            ExtendedButtonType type = GetButtonType();
            switch (type)
            {
                case ExtendedButtonType.Object:
                    SetActiveSelectObject(false);
                    SetActiveDefaultObject(true);
                    break;
                case ExtendedButtonType.ObjectAndText:
                    SetActiveSelectObjectAndText(false);
                    SetActiveDefaultObjectAndText(true);
                    break;
                case ExtendedButtonType.Sprite:
                    if (_offSprite != null)
                        image.sprite = _offSprite;
                    break;
                case ExtendedButtonType.SpriteAndText:
                    Color textColor = _offTextColor;
                    SetActiveTextColor(false, textColor, 0);
                    break;
                case ExtendedButtonType.SpriteColor:
                    SetActiveSelectSpriteColor(false);
                    SetActiveDefaultSpriteColor(true);
                    break;
                case ExtendedButtonType.SpriteColorAndText:
                    SetActiveSelectSpriteColorAndText(false);
                    SetActiveDefaultSpriteColorAndText(true);
                    break;
            }

            onValueChanged.Invoke(false);
        }


        // Press On
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            IsPressed = true;

            ExtendedButtonType type = GetButtonType();
            switch (type)
            {
                case ExtendedButtonType.Object:
                    SetActiveDefaultObject(false);
                    SetActiveSelectObject(false);
                    SetActivePressObject(true);
                    break;
                case ExtendedButtonType.ObjectAndText:
                    SetActiveDefaultObjectAndText(false);
                    SetActiveSelectObjectAndText(false);
                    SetActivePressObjectAndText(true);
                    break;
                case ExtendedButtonType.Sprite:
                    SetActiveDefaultSprite(false);
                    SetActivePressSprite(true);
                    break;
                case ExtendedButtonType.SpriteAndText:
                    SetActiveDefaultSpriteAndText(false);
                    SetActivePressSpriteAndText(true);
                    break;
                case ExtendedButtonType.SpriteColor:
                    SetActiveDefaultSpriteColor(false);
                    SetActivePressSpriteColor(true);
                    break;
                case ExtendedButtonType.SpriteColorAndText:
                    SetActiveDefaultSpriteColorAndText(false);
                    SetActivePressSpriteColorAndText(true);
                    break;
            }
        }

        // Press Off
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            IsPressed = false;

            ExtendedButtonType type = GetButtonType();
            switch (type)
            {
                case ExtendedButtonType.Object:
                    SetActivePressObject(false);
                    break;
                case ExtendedButtonType.ObjectAndText:
                    SetActivePressObjectAndText(false);
                    break;
                case ExtendedButtonType.SpriteAndText:
                    SetActivePressSpriteAndText(false);
                    break;
                case ExtendedButtonType.SpriteColor:
                    SetActivePressSpriteColor(false);
                    break;
                case ExtendedButtonType.SpriteColorAndText:
                    SetActivePressSpriteColorAndText(false);
                    break;
            }
        }

        private ExtendedButtonType GetButtonType()
        {
            ExtendedButtonType type;
            if (_useObject)
            {
                if (_useText)
                {
                    type = ExtendedButtonType.ObjectAndText;
                }
                else
                {
                    type = ExtendedButtonType.Object;
                }
            }
            else
            {
                if (_useSpriteColor)
                {
                    if (_useText)
                    {
                        type = ExtendedButtonType.SpriteColorAndText;
                    }
                    else
                    {
                        type = ExtendedButtonType.SpriteColor;
                    }
                }
                else
                {
                    if (_useText)
                    {
                        type = ExtendedButtonType.SpriteAndText;
                    }
                    else
                    {
                        type = ExtendedButtonType.Sprite;
                    }
                }
            }

            return type;
        }

        private void SetActiveDefaultObject(bool enable)
        {
            if (_defaultObject != null)
                _defaultObject.SetActive(enable);
        }

        private void SetActiveDefaultObjectAndText(bool enable)
        {
            SetActiveDefaultObject(enable);
            SetActiveTextColor(enable, _defaultTextColor, 0);
        }

        private void SetActiveDefaultSprite(bool enable)
        {
            if (enable)
            {
                if (_defaultSprite != null)
                    image.sprite = _defaultSprite;
            }
        }

        private void SetActiveDefaultSpriteAndText(bool enable)
        {
            SetActiveDefaultSprite(enable);

        }

        private void SetActiveDefaultSpriteColor(bool enable)
        {
            if (enable)
            {
                if (_defaultSpriteColor != Color.clear)
                    image.DOColor(_defaultSpriteColor, _colorDuration).SetUpdate(true);
            }
            else
            {
                image.DOColor(Color.white, _colorDuration).SetUpdate(true);
            }
        }

        private void SetActiveDefaultSpriteColorAndText(bool enable)
        {
            SetActiveDefaultSpriteColor(enable);
            SetActiveTextColor(enable, _defaultTextColor, _colorDuration);
        }

        private void SetActiveEnterObject(bool enable)
        {
            if (_pointerEnterObject != null)
            {
                _pointerEnterObject.SetActive(enable);
            }
            else
            {
                _defaultObject.SetActive(enable);
            }
        }

        private void SetActiveEnterObjectAndText(bool enable)
        {
            SetActiveEnterObject(enable);
            SetActiveTextColor(enable, _pointerEnterTextColor, 0);
        }


        private void SetActiveEnterSprite(bool enable)
        {
            if (enable)
            {
                if (_pointerEnterSprite != null)
                {
                    image.sprite = _pointerEnterSprite;
                }
                else
                {
                    if (_defaultSprite != null)
                        image.sprite = _defaultSprite;
                }
            }
            else
            {
                if (_defaultSprite != null)
                    image.sprite = _defaultSprite;
            }
        }

        private void SetActiveEnterSpriteAndText(bool enable)
        {
            SetActiveEnterSprite(enable);
            SetActiveTextColor(enable, _pointerEnterTextColor, 0);
        }

        private void SetActiveEnterSpriteColor(bool enable)
        {
            image.DOKill();

            if (enable)
            {
                if (_pointerEnterSpriteColor != Color.clear)
                    image.DOColor(_pointerEnterSpriteColor, _colorDuration).SetUpdate(true);
                ;
            }
            else
            {
                if (_defaultSpriteColor != Color.clear)
                    image.DOColor(_defaultSpriteColor, _colorDuration).SetUpdate(true);
                ;
            }
        }

        private void SetActiveEnterSpriteColorAndText(bool enable)
        {
            SetActiveEnterSpriteColor(enable);
            SetActiveTextColor(enable, _pointerEnterTextColor, _colorDuration);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            IsClicked = true;
            if (isOn)
            {
                OnDeselect();
                SoundManager.Instance.PlaySFX(_offClip);
            }
            else
            {
                OnSelect();
                SoundManager.Instance.PlaySFX(_onClip);
            }
            IsClicked = false;
        }

        private void SetActiveSelectObject(bool enable)
        {
            if (enable)
            {
                if (_onObject != null)
                {
                    _onObject.SetActive(true);
                    _offObject.SetActive(false);
                }
                else if (_pointerEnterObject != null)
                {
                    _pointerEnterObject.SetActive(true);
                }
                else
                {
                    if (_defaultObject != null)
                        _defaultObject.SetActive(true);
                }
            }
            else
            {
                if (_offObject != null)
                {
                    _onObject.SetActive(false);
                    _offObject.SetActive(true);
                }
                else if (_pointerEnterObject != null)
                {
                    _pointerEnterObject.SetActive(false);
                }
                else
                {
                    if (_defaultObject != null)
                        _defaultObject.SetActive(true);
                }
            }
        }

        private void SetActiveSelectObjectAndText(bool enable)
        {
            SetActiveSelectObject(enable);

            Color textColor = enable ? _onTextColor : _offTextColor;
            SetActiveTextColor(enable, textColor, 0);
        }

        private void SetActiveSelectSprite(bool enable)
        {
            if (enable)
            {
                if (_onSprite != null)
                {
                    image.sprite = _onSprite;
                }
                else
                {
                    if (_defaultSprite != null)
                        image.sprite = _defaultSprite;
                }
            }
            else
            {
                if (_defaultSprite != null)
                    image.sprite = _defaultSprite;
            }
        }

        private void SetActiveSelectSpriteAndText(bool enable)
        {
            SetActiveSelectSpriteColorAndText(enable);
        }

        private void SetActiveSelectSpriteColor(bool enable)
        {
            image.DOKill();

            if (enable)
            {
                if (_onSpriteColor != Color.clear)
                {
                    image.DOColor(_onSpriteColor, _colorDuration).SetUpdate(true);
                }
                else
                {
                    if (_defaultSpriteColor != Color.clear)
                        image.DOColor(_defaultSpriteColor, _colorDuration).SetUpdate(true);
                    ;
                }
            }
            else
            {
                if (_defaultSpriteColor != Color.clear)
                    image.DOColor(_defaultSpriteColor, _colorDuration).SetUpdate(true);
                ;
            }
        }

        private void SetActiveSelectSpriteColorAndText(bool enable)
        {
            SetActiveSelectSpriteColor(enable);
            Color textColor = enable ? _onTextColor : _offTextColor;
            SetActiveTextColor(enable, textColor, _colorDuration);
        }


        private void SetActivePressObject(bool enable)
        {
            if (enable)
            {
                if (_pointerEnterObject != null)
                    _pointerEnterObject.SetActive(false);

                if (_pressedObject != null)
                {
                    _pressedObject.SetActive(true);
                }
                else
                {
                    if (_defaultObject != null)
                        _defaultObject.SetActive(true);
                }
            }
            else
            {
                if (_pressedObject != null)
                {
                    _pressedObject.SetActive(false);
                }
            }
        }

        private void SetActivePressObjectAndText(bool enable)
        {
            SetActivePressObject(enable);
            SetActiveTextColor(enable, _pressedTextColor, 0);
        }

        private void SetActivePressSprite(bool enable)
        {
            if (enable)
            {
                if (_pressedSprite != null)
                {
                    image.sprite = _pressedSprite;
                }
                else
                {
                    if (_defaultSprite != null)
                        image.sprite = _defaultSprite;
                }
            }
            else
            {
                if (_defaultSprite != null)
                    image.sprite = _defaultSprite;
            }
        }

        private void SetActivePressSpriteAndText(bool enable)
        {
            SetActivePressSprite(enable);
            SetActiveTextColor(enable, _pressedTextColor, 0);
        }

        private void SetActivePressSpriteColor(bool enable)
        {
            image.DOKill();

            if (enable)
            {
                if (_pressedSpriteColor != Color.clear)
                {
                    image.DOColor(_pressedSpriteColor, _colorDuration).SetUpdate(true);
                    ;
                }
                else
                {
                    if (_defaultSpriteColor != Color.clear)
                    {
                        image.DOColor(_defaultSpriteColor, _colorDuration).SetUpdate(true);
                        ;
                    }
                }
            }
            else
            {
                if (_defaultSpriteColor != Color.clear)
                    image.DOColor(_defaultSpriteColor, _colorDuration).SetUpdate(true);
                ;
            }
        }

        private void SetActivePressSpriteColorAndText(bool enable)
        {
            SetActivePressSpriteColor(enable);
            SetActiveTextColor(enable, _pressedTextColor, _colorDuration);
        }

        private void SetActiveTextColor(bool enable, Color targetColor, float duration)
        {
            if (enable)
            {
                if (targetColor != Color.clear)
                {
                    _textMeshPro.DOKill();
                    _textMeshPro.DOColor(targetColor, duration).SetUpdate(true);
                    ;
                }
                else
                {
                    if (_defaultTextColor != Color.clear)
                    {
                        _textMeshPro.DOKill();
                        _textMeshPro.DOColor(_defaultTextColor, duration).SetUpdate(true);
                        ;
                    }
                }
            }
            else
            {
                if (targetColor != Color.clear)
                {
                    _textMeshPro.DOKill();
                    _textMeshPro.DOColor(targetColor, duration).SetUpdate(true);
                    ;
                }

                if (_defaultTextColor != Color.clear)
                {
                    _textMeshPro.DOKill();
                    _textMeshPro.DOColor(_defaultTextColor, duration).SetUpdate(true);
                    ;
                }
            }
        }

        // Navigation Focus
        public override void OnSelect(BaseEventData eventData)
        {
            OnPointerEnter(null);
            base.OnSelect(eventData);

            bool mouseLeftDown = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            if (_focusBGObject != null && !mouseLeftDown)
                _focusBGObject.SetActive(true);
        }

        // Navigation UnFocus
        public override void OnDeselect(BaseEventData eventData)
        {
            OnPointerExit(null);
            base.OnDeselect(eventData);

            if (_focusBGObject != null)
                _focusBGObject.SetActive(false);
        }

        // Navigation Click
        public void OnSubmit(BaseEventData eventData)
        {
            OnPointerClick(null);

            SoundManager.Instance.PlaySFX(_offClip);
        }
    }
}