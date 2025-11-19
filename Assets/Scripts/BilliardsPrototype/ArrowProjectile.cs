using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class ArrowProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private LayerMask _bounceMask = ~0;
        [SerializeField] private float _maxLifetime = 8f;
        [SerializeField] private float _destroyDelayAfterStop = 0.8f;
        [SerializeField] private float _surfaceOffset = 0.05f;
        [SerializeField] private float _minimumBounceSpeed = 4f;
        [SerializeField] private float _damage = 5f;

        private int _remainingBounces;
        private float _cachedSpeed;
        private bool _isActive;
        private float _lifeTimer;

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
        }

        private void Awake()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody2D>();

            if (_collider == null)
                _collider = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (_isActive == false)
                return;

            _lifeTimer += Time.deltaTime;
            if (_lifeTimer >= _maxLifetime)
            {
                FinishProjectile();
                return;
            }

            if (_rigidbody.linearVelocity.sqrMagnitude > 0.001f)
            {
                Vector2 direction = _rigidbody.linearVelocity.normalized;
                transform.right = direction;
                _cachedSpeed = _rigidbody.linearVelocity.magnitude;
            }
        }

        public void Initialize(Vector2 direction, float launchSpeed, int maxBounceCount, LayerMask bounceLayer)
        {
            _isActive = true;
            _lifeTimer = 0f;
            _cachedSpeed = launchSpeed;
            _remainingBounces = Mathf.Max(0, maxBounceCount);
            _bounceMask = bounceLayer;

            Vector2 normalizedDirection = direction.sqrMagnitude < Mathf.Epsilon ? Vector2.right : direction.normalized;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.linearVelocity = normalizedDirection * launchSpeed;
            transform.right = normalizedDirection;
            _collider.enabled = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_isActive == false)
                return;

            CharacterHealth targetHealth = collision.collider.GetComponent<CharacterHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(_damage);
                FinishProjectile();
                return;
            }

            if (collision.collider.TryGetComponent<CardObstacle>(out CardObstacle cardObstacle))
                cardObstacle.HandleHit();

            int otherLayer = collision.gameObject.layer;
            bool canBounce = (_bounceMask.value & (1 << otherLayer)) != 0;

            if (canBounce == false || _remainingBounces <= 0)
            {
                FinishProjectile();
                return;
            }

            Vector2 incoming = _rigidbody.linearVelocity.sqrMagnitude <= Mathf.Epsilon
                ? (collision.relativeVelocity.sqrMagnitude < Mathf.Epsilon ? Vector2.right : collision.relativeVelocity.normalized)
                : _rigidbody.linearVelocity.normalized;
            Vector2 normal = collision.GetContact(0).normal;
            Vector2 reflected = Vector2.Reflect(incoming, normal).normalized;

            ContactPoint2D contact = collision.GetContact(0);
            Vector2 contactPoint = contact.point;
            _rigidbody.position = contactPoint + normal * _surfaceOffset;
            _rigidbody.linearVelocity = reflected * Mathf.Max(_cachedSpeed, _minimumBounceSpeed);
            transform.right = reflected;
            _remainingBounces--;
        }

        private void FinishProjectile()
        {
            if (_isActive == false)
                return;

            _isActive = false;
            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            if (_collider != null)
                _collider.enabled = false;

            Destroy(gameObject, _destroyDelayAfterStop);
        }
    }
}
