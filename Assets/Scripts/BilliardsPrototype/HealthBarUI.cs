using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Transform _fillTransform;
        [SerializeField] private bool _hideWhenEmpty;

        private CharacterHealth _boundHealth;
        private Vector3 _baseScale = Vector3.one;

        private void Awake()
        {
            if (_fillTransform != null)
                _baseScale = _fillTransform.localScale;
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
                _baseScale = _fillTransform.localScale;
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

            if (_hideWhenEmpty)
                gameObject.SetActive(amount > 0f);
        }
    }
}
