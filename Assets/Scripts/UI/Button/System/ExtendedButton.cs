using System;
using Aftertime.StorylineEngine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Aftertime.SecretSome.UI
{
    public enum ExtendedButtonType
    {
        Object,
        ObjectAndText,
        Sprite,
        SpriteAndText,
        SpriteColor,
        SpriteColorAndText,
    }

    public class ExtendedButton : Button
    {
        public TextMeshProUGUI TextMeshPro => _textMeshPro;
        public RectTransform RectTransform { get; private set; }
        public AudioClip ClickClip => _clickClip;
        public event Action onPointerEnter;
        public event Action onPointerExit;
        public event Action onPointerClick;
        public bool IsMouseEnter { get; protected set; }

        [SerializeField] protected TextMeshProUGUI _textMeshPro;

        [Header("[Option]")] [SerializeField] private bool _useSelectState = true;
        [SerializeField] private bool _useUpdateGraphic;
        [SerializeField] private bool _useObject;
        [SerializeField] private bool _useSpriteColor;
        [SerializeField] private bool _useText;
        [SerializeField] private float _colorDuration;

        [Header("[Default]")] [SerializeField] protected GameObject _defaultObject;
        [Header("[Default]")] [SerializeField] protected Sprite _defaultSprite;
        [Header("[Default]")] [SerializeField] protected Color _defaultSpriteColor;
        [SerializeField] protected Color _defaultTextColor;

        [Header("[Enter]")] [SerializeField] protected GameObject _pointerEnterObject;
        [Header("[Enter]")] [SerializeField] protected Sprite _pointerEnterSprite;
        [Header("[Enter]")] [SerializeField] protected Color _pointerEnterSpriteColor;
        [SerializeField] protected Color _pointerEnterTextColor;

        [Header("[Select]")] [SerializeField] protected GameObject _selectedObject;
        [Header("[Select]")] [SerializeField] protected Sprite _selectedSprite;
        [Header("[Select]")] [SerializeField] protected Color _selectedSpriteColor;
        [SerializeField] protected Color _selectedTextColor;

        [Header("[Press]")] [SerializeField] private GameObject _pressedObject;
        [Header("[Press]")] [SerializeField] protected Sprite _pressedSprite;
        [Header("[Press]")] [SerializeField] protected Color _pressedSpriteColor;
        [SerializeField] private Color _pressedTextColor;

        [Header("[Focus]")] [SerializeField] public GameObject _focusBGObject;

        [Header("[Sound]")] [SerializeField] private AudioClip _clickClip;

        public bool IsSelected { get; protected set; }
        public bool IsPressed { get; protected set; }

        private float Alpha => image.color.a;

        protected override void Awake()
        {
            base.Awake();
            RectTransform = GetComponent<RectTransform>();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData != null)
                IsMouseEnter = true;

            if (interactable == false)
                return;

            base.OnPointerEnter(eventData);

            UpdateGraphicOnPointerEnter();

            onPointerEnter?.Invoke();
        }

        private void UpdateGraphicOnPointerEnter()
        {
            if (!_useUpdateGraphic)
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
            IsMouseEnter = false;

            if (IsSelected || interactable == false)
                return;

            base.OnPointerExit(eventData);

            IsPressed = false;
            UpdateGraphicOnPointerExit();

            onPointerExit?.Invoke();
        }

        public void UpdateGraphicOnPointerExit()
        {
            if (!_useUpdateGraphic)
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


        // Press On
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            base.OnPointerDown(eventData);

            IsPressed = true;
            UpdateGraphicOnPointerDown();
        }

        public void UpdateGraphicOnPointerDown()
        {
            if (!_useUpdateGraphic)
                return;

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
            if (interactable == false)
                return;

            base.OnPointerUp(eventData);

            IsPressed = false;
            UpdateGraphicOnPointerUp();
        }

        public void UpdateGraphicOnPointerUp()
        {
            if (!_useUpdateGraphic)
                return;

            ExtendedButtonType type = GetButtonType();
            switch (type)
            {
                case ExtendedButtonType.Object:
                    SetActivePressObject(false);
                    break;
                case ExtendedButtonType.ObjectAndText:
                    SetActivePressObjectAndText(false);
                    break;
                case ExtendedButtonType.Sprite:
                    SetActivePressSprite(false);
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

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (interactable == false)
                return;

            OnSelect();
            SoundManager.Instance.PlaySFX(_clickClip);

            onPointerClick?.Invoke();
        }

        public virtual void OnSelect()
        {
            if (_useSelectState)
                IsSelected = true;

            UpdateGraphicOnSelect();

            onClick?.Invoke();
        }

        public void UpdateGraphicOnSelect()
        {
            if (!_useUpdateGraphic)
                return;

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
                    SetActiveDefaultSprite(false);
                    SetActiveSelectSprite(true);
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
        }

        public virtual void OnDeselect()
        {
            IsSelected = false;
            UpdateGraphicOnDeSelect();
        }

        public void UpdateGraphicOnDeSelect()
        {
            if (!_useUpdateGraphic)
                return;

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
                    SetActiveSelectSprite(false);
                    SetActiveDefaultSprite(true);
                    break;
                case ExtendedButtonType.SpriteAndText:
                    SetActiveSelectSpriteAndText(false);
                    SetActiveDefaultSpriteAndText(true);
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
            else
            {
                image.sprite = null;
            }
        }

        private void SetActiveDefaultSpriteAndText(bool enable)
        {
            SetActiveDefaultSprite(enable);
            SetActiveTextColor(enable, _defaultTextColor, 0);
        }

        private void SetActiveDefaultSpriteColor(bool enable)
        {
            image.DOKill();

            if (enable)
            {
                if (_defaultSpriteColor != Color.clear)
                {
                    Color color = new Color(_defaultSpriteColor.r, _defaultSpriteColor.g, _defaultSpriteColor.b, Alpha);
                    image.DOColor(color, _colorDuration).SetUpdate(true);
                }
            }
            else
            {
                image.DOColor(Color.white, _colorDuration).SetUpdate(true);
                ;
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
                {
                    Color color = new Color(_pointerEnterSpriteColor.r, _pointerEnterSpriteColor.g,
                        _pointerEnterSpriteColor.b, Alpha);
                    image.DOColor(color, _colorDuration).SetUpdate(true);
                }
            }
            else
            {
                if (_defaultSpriteColor != Color.clear)
                {
                    Color color = new Color(_defaultSpriteColor.r, _defaultSpriteColor.g,
                        _defaultSpriteColor.b, Alpha);
                    image.DOColor(color, _colorDuration).SetUpdate(true);
                    ;
                }
            }
        }

        private void SetActiveEnterSpriteColorAndText(bool enable)
        {
            SetActiveEnterSpriteColor(enable);
            SetActiveTextColor(enable, _pointerEnterTextColor, _colorDuration);
        }

        private void SetActiveSelectObject(bool enable)
        {
            if (enable)
            {
                if (_selectedObject != null)
                {
                    _selectedObject.SetActive(true);
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
                if (_selectedObject != null)
                {
                    _selectedObject.SetActive(false);
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
            SetActiveTextColor(enable, _selectedTextColor, 0);
        }

        private void SetActiveSelectSprite(bool enable)
        {
            if (enable)
            {
                if (_selectedSprite != null)
                {
                    image.sprite = _selectedSprite;
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
            SetActiveSelectSprite(enable);
            SetActiveTextColor(enable, _selectedTextColor, 0);
        }

        private void SetActiveSelectSpriteColor(bool enable)
        {
            image.DOKill();

            if (enable)
            {
                if (_selectedSpriteColor != Color.clear)
                {
                    Color color = new Color(_selectedSpriteColor.r, _selectedSpriteColor.g,
                        _selectedSpriteColor.b, Alpha);
                    image.DOColor(_selectedSpriteColor, _colorDuration).SetUpdate(true);
                    ;
                }
                else
                {
                    if (_defaultSpriteColor != Color.clear)
                    {
                        Color color = new Color(_defaultSpriteColor.r, _defaultSpriteColor.g,
                            _defaultSpriteColor.b, Alpha);
                        image.DOColor(color, _colorDuration).SetUpdate(true);
                        ;
                    }
                }
            }
            else
            {
                if (_defaultSpriteColor != Color.clear)
                {
                    Color color = new Color(_defaultSpriteColor.r, _defaultSpriteColor.g,
                        _defaultSpriteColor.b, Alpha);
                    image.DOColor(color, _colorDuration).SetUpdate(true);
                    ;
                }
            }
        }

        private void SetActiveSelectSpriteColorAndText(bool enable)
        {
            SetActiveSelectSpriteColor(enable);
            SetActiveTextColor(enable, _selectedTextColor, _colorDuration);
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
                    Color color = new Color(_pressedSpriteColor.r, _pressedSpriteColor.g,
                        _pressedSpriteColor.b, Alpha);
                    image.DOColor(color, _colorDuration).SetUpdate(true);
                    ;
                }
                else
                {
                    if (_defaultSpriteColor != Color.clear)
                    {
                        Color color = new Color(_defaultSpriteColor.r, _defaultSpriteColor.g,
                            _defaultSpriteColor.b, Alpha);
                        image.DOColor(color, _colorDuration).SetUpdate(true);
                        ;
                    }
                }
            }
            else
            {
                if (_defaultSpriteColor != Color.clear)
                {
                    Color color = new Color(_defaultSpriteColor.r, _defaultSpriteColor.g,
                        _defaultSpriteColor.b, Alpha);
                    image.DOColor(color, _colorDuration).SetUpdate(true);
                    ;
                }
            }
        }

        private void SetActivePressSpriteColorAndText(bool enable)
        {
            SetActivePressSpriteColor(enable);
            SetActiveTextColor(enable, _pressedTextColor, _colorDuration);
        }

        private void SetActiveTextColor(bool enable, Color targetColor, float duration)
        {
            _textMeshPro.DOKill();

            if (enable)
            {
                if (targetColor != Color.clear)
                {
                    _textMeshPro.DOColor(targetColor, duration).SetUpdate(true);
                    ;
                }
                else
                {
                    if (_defaultTextColor != Color.clear)
                        _textMeshPro.DOColor(_defaultTextColor, duration).SetUpdate(true);
                    ;
                }
            }
            else
            {
                if (_defaultTextColor != Color.clear)
                    _textMeshPro.DOColor(_defaultTextColor, duration).SetUpdate(true);
            }
        }

        public void RemoveAllPointerEnterEvent()
        {
            onPointerEnter = () => { };
        }

        // Navigation Focus
        public override void OnSelect(BaseEventData eventData)
        {
            if (interactable == false)
                return;

            OnPointerEnter(null);
            base.OnSelect(eventData);

            bool mouseLeftDown = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            if (_focusBGObject != null && !mouseLeftDown)
                _focusBGObject.SetActive(true);
        }

        // Navigation UnFocus
        public override void OnDeselect(BaseEventData eventData)
        {
            if (interactable == false)
                return;

            OnPointerExit(null);
            base.OnDeselect(eventData);

            if (_focusBGObject != null)
                _focusBGObject.SetActive(false);
        }

        // Navigation Click
        public override void OnSubmit(BaseEventData eventData)
        {
            if (interactable == false)
                return;

            base.OnSubmit(eventData);
            UpdateGraphicOnSelect();

            if (!IsSelected)
                SoundManager.Instance.PlaySFX(ClickClip);

            if (_useSelectState)
                IsSelected = true;
        }
    }
}