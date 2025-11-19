using System;
using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public class CharacterHealth : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 20f;
        [SerializeField] private bool _deactivateOnDeath = true;
        [SerializeField] private HealthBarUI _healthBar;

        public event Action<float> onHealthChanged = delegate { };
        public event Action onDeath = delegate { };

        public float CurrentHealth => _currentHealth;
        public float NormalizedHealth => Mathf.Approximately(_maxHealth, 0f) ? 0f : _currentHealth / _maxHealth;
        public bool IsDead => _currentHealth <= 0f;

        private float _currentHealth;

        private void Awake()
        {
            if (_maxHealth <= 0f)
                _maxHealth = 1f;

            _currentHealth = _maxHealth;
            if (_healthBar != null)
                _healthBar.Bind(this);

            onHealthChanged(NormalizedHealth);
        }

        public void SetMaxHealth(float value)
        {
            _maxHealth = Mathf.Max(1f, value);
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
            onHealthChanged(NormalizedHealth);
        }

        public void AssignHealthBar(HealthBarUI bar)
        {
            _healthBar = bar;
            _healthBar?.Bind(this);
        }

        public void SetDeactivateOnDeath(bool value)
        {
            _deactivateOnDeath = value;
        }

        public void TakeDamage(float amount)
        {
            if (IsDead)
                return;

            _currentHealth = Mathf.Max(0f, _currentHealth - Mathf.Max(0f, amount));
            onHealthChanged(NormalizedHealth);

            if (IsDead)
                Die();
        }

        public void Heal(float amount)
        {
            if (IsDead)
                return;

            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + Mathf.Max(0f, amount));
            onHealthChanged(NormalizedHealth);
        }

        private void Die()
        {
            onDeath();
            if (_deactivateOnDeath)
                gameObject.SetActive(false);
        }
    }
}
