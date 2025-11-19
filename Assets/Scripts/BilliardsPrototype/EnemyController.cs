using System;
using System.Collections;
using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public class EnemyController : MonoBehaviour
    {
        public static event Action onPlayerTurnBegan = delegate { };

        [SerializeField] private CharacterHealth _enemyHealth;
        [SerializeField] private CharacterHealth _playerHealth;
        [SerializeField] private Transform _modelRoot;
        [SerializeField] private float _attackDamage = 5f;
        [SerializeField] private float _shakeDuration = 0.6f;
        [SerializeField] private float _shakeAmplitude = 0.25f;

        private Coroutine _attackRoutine;
        private Vector3 _defaultLocalPos;

        private void Awake()
        {
            if (_modelRoot == null)
                _modelRoot = transform;

            _defaultLocalPos = _modelRoot.localPosition;
        }

        private void OnEnable()
        {
            ArrowShooter.onArrowFired += HandlePlayerShot;
            if (_enemyHealth != null)
                _enemyHealth.onDeath += StopAttack;

            onPlayerTurnBegan();
        }

        private void OnDisable()
        {
            ArrowShooter.onArrowFired -= HandlePlayerShot;
            if (_enemyHealth != null)
                _enemyHealth.onDeath -= StopAttack;
        }

        public void Configure(CharacterHealth enemy, CharacterHealth player, Transform modelRoot = null)
        {
            if (_enemyHealth != null)
                _enemyHealth.onDeath -= StopAttack;

            _enemyHealth = enemy;
            _playerHealth = player;

            if (_enemyHealth != null)
                _enemyHealth.onDeath += StopAttack;

            if (modelRoot != null)
            {
                _modelRoot = modelRoot;
                _defaultLocalPos = _modelRoot.localPosition;
            }
        }

        private void HandlePlayerShot()
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
            onPlayerTurnBegan();
        }

        private void StopAttack()
        {
            if (_attackRoutine != null)
            {
                StopCoroutine(_attackRoutine);
                _attackRoutine = null;
            }

            if (_modelRoot != null)
                _modelRoot.localPosition = _defaultLocalPos;

            onPlayerTurnBegan();
        }
    }
}
