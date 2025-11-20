using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Transform _fillTransform;
        [SerializeField] private bool _hideWhenEmpty;

        private CharacterHealth _boundHealth;
        private Vector3 _baseScale = Vector3.one;
        private Vector3 _baseLocalPosition = Vector3.zero;

        private void Awake()
        {
            if (_fillTransform != null)
            {
                _baseScale = _fillTransform.localScale;
                _baseLocalPosition = _fillTransform.localPosition;
            }
        }

        private void OnDestroy()
        {
            if (_boundHealth != null)
                _boundHealth.onHealthChanged -= UpdateFill;
        }

        public void Initialize(Transform fillTransform)
        {
            _fillTransform = fillTransform;
            if (_fillTransform != null)
            {
                _baseScale = _fillTransform.localScale;
                _baseLocalPosition = _fillTransform.localPosition;
            }
        }

        public void Bind(CharacterHealth health)
        {
            if (_boundHealth != null)
                _boundHealth.onHealthChanged -= UpdateFill;

            _boundHealth = health;
            if (_boundHealth != null)
            {
                _boundHealth.onHealthChanged += UpdateFill;
                UpdateFill(_boundHealth.NormalizedHealth);
            }
        }

        private void UpdateFill(float normalized)
        {
            if (_fillTransform == null)
                return;

            float amount = Mathf.Clamp01(normalized);
            Vector3 scale = _baseScale;
            scale.x = _baseScale.x * amount;
            _fillTransform.localScale = scale;

            float widthDelta = _baseScale.x - scale.x;
            Vector3 anchoredPosition = _baseLocalPosition;
            anchoredPosition.x -= widthDelta * 0.5f;
            _fillTransform.localPosition = anchoredPosition;

            if (_hideWhenEmpty)
                gameObject.SetActive(amount > 0f);
        }
    }
}
