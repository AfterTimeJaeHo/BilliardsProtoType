using System;
using System.Collections;
using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public class EnemyController : MonoBehaviour
    {
        [Serializable]
        private class SpritePhase
        {
            [Range(0f, 1f)] public float healthPercent = 0.5f;
            public Sprite sprite;
        }

        [SerializeField] private CharacterHealth _enemyHealth;
        [SerializeField] private CharacterHealth _playerHealth;
        [SerializeField] private Transform _modelRoot;
        [SerializeField] private float _attackDamage = 5f;
        [SerializeField] private float _shakeDuration = 0.6f;
        [SerializeField] private float _shakeAmplitude = 0.25f;
        [SerializeField] private float _attackDelayAfterVolley = 0.4f;
        [SerializeField] private SpriteRenderer _enemyRenderer;
        [SerializeField] private SpritePhase[] _spritePhases = new SpritePhase[0];

        private Coroutine _attackRoutine;
        private Coroutine _pendingAttackRoutine;
        private Vector3 _defaultLocalPos;
        [SerializeField] private Sprite _defaultSprite;

        private void Awake()
        {
            if (_modelRoot == null)
                _modelRoot = transform;

            if (_enemyRenderer == null && _modelRoot != null)
                _enemyRenderer = _modelRoot.GetComponentInChildren<SpriteRenderer>();

            if (_enemyRenderer != null && _defaultSprite == null)
                _defaultSprite = _enemyRenderer.sprite;

            _defaultLocalPos = _modelRoot.localPosition;
        }

        private void OnEnable()
        {
            ArrowShooter.onVolleySequenceCompleted += HandleVolleySequenceCompleted;
            RegisterEnemyEvents(_enemyHealth);
        }

        private void OnDisable()
        {
            ArrowShooter.onVolleySequenceCompleted -= HandleVolleySequenceCompleted;
            UnregisterEnemyEvents(_enemyHealth);
            StopPendingAttack();
            StopAttack();
        }

        public void Configure(CharacterHealth enemy, CharacterHealth player, Transform modelRoot = null)
        {
            UnregisterEnemyEvents(_enemyHealth);

            _enemyHealth = enemy;
            _playerHealth = player;

            if (modelRoot != null)
            {
                _modelRoot = modelRoot;
                _defaultLocalPos = _modelRoot.localPosition;
            }

            if (_enemyRenderer == null && _modelRoot != null)
                _enemyRenderer = _modelRoot.GetComponentInChildren<SpriteRenderer>();

            if (_enemyRenderer != null && _defaultSprite == null)
                _defaultSprite = _enemyRenderer.sprite;

            RegisterEnemyEvents(_enemyHealth);
        }

        private void HandleVolleySequenceCompleted()
        {
            if (isActiveAndEnabled == false)
                return;

            StopPendingAttack();
            _pendingAttackRoutine = StartCoroutine(DelayedAttackRoutine());
        }

        private IEnumerator DelayedAttackRoutine()
        {
            float delay = Mathf.Max(0f, _attackDelayAfterVolley);
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            TryBeginAttack();
            _pendingAttackRoutine = null;
        }

        private void HandleEnemyHealthChanged(float normalizedHealth)
        {
            UpdateEnemySprite(normalizedHealth);
        }

        private void RegisterEnemyEvents(CharacterHealth enemy)
        {
            if (enemy == null)
                return;

            enemy.onDeath += StopAttack;
            enemy.onHealthChanged += HandleEnemyHealthChanged;
            HandleEnemyHealthChanged(enemy.NormalizedHealth);
        }

        private void UnregisterEnemyEvents(CharacterHealth enemy)
        {
            if (enemy == null)
                return;

            enemy.onDeath -= StopAttack;
            enemy.onHealthChanged -= HandleEnemyHealthChanged;
        }

        private void TryBeginAttack()
        {
            if (_enemyHealth != null && _enemyHealth.IsDead)
                return;

            if (_playerHealth != null && _playerHealth.IsDead)
                return;

            if (_attackRoutine == null)
                _attackRoutine = StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            float elapsed = 0f;
            while (elapsed < _shakeDuration)
            {
                float offset = Mathf.Sin(elapsed * Mathf.PI * 4f) * _shakeAmplitude;
                if (_modelRoot != null)
                    _modelRoot.localPosition = _defaultLocalPos + new Vector3(offset, 0f, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (_modelRoot != null)
                _modelRoot.localPosition = _defaultLocalPos;

            _playerHealth?.TakeDamage(_attackDamage);
            _attackRoutine = null;
        }

        private void StopPendingAttack()
        {
            if (_pendingAttackRoutine == null)
                return;

            StopCoroutine(_pendingAttackRoutine);
            _pendingAttackRoutine = null;
        }

        private void StopAttack()
        {
            StopPendingAttack();

            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
                _attackRoutine = null;
            }

            if (_modelRoot != null)
                _modelRoot.localPosition = _defaultLocalPos;
        }

        private void UpdateEnemySprite(float normalizedHealth)
        {
            if (_enemyRenderer == null)
                return;

            float clampedHealth = Mathf.Clamp01(normalizedHealth);
            Sprite selectedSprite = _defaultSprite;

            if (_spritePhases != null)
            {
                for (int i = 0; i < _spritePhases.Length; i++)
                {
                    SpritePhase phase = _spritePhases[i];
                    if (phase == null || phase.sprite == null)
                        continue;

                    if (clampedHealth <= phase.healthPercent)
                        selectedSprite = phase.sprite;
                }
            }

            if (selectedSprite != null && _enemyRenderer.sprite != selectedSprite)
                _enemyRenderer.sprite = selectedSprite;
        }
    }
}
