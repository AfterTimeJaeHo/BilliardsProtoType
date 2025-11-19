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
        [SerializeField] private float _manualSimStep = 0.02f;
        [SerializeField] private float _damage = 5f;

        private int _remainingBounces;
        private float _currentSpeed;
        private bool _isActive;
        private float _lifeTimer;
        private Vector2 _travelDirection = Vector2.right;
        private Vector2 _currentPosition;

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

            ManualSimulate(Time.deltaTime);
        }

        public void Initialize(Vector2 direction, float launchSpeed, int maxBounceCount, LayerMask bounceLayer)
        {
            _isActive = true;
            _lifeTimer = 0f;
            _currentSpeed = Mathf.Max(launchSpeed, _minimumBounceSpeed);
            _remainingBounces = Mathf.Max(0, maxBounceCount);
            _bounceMask = bounceLayer;

            _travelDirection = direction.sqrMagnitude < Mathf.Epsilon ? Vector2.right : direction.normalized;
            _currentPosition = _rigidbody != null ? _rigidbody.position : (Vector2)transform.position;

            if (_rigidbody != null)
            {
                _rigidbody.simulated = false;
                _rigidbody.bodyType = RigidbodyType2D.Kinematic;
                _rigidbody.linearVelocity = Vector2.zero;
            }

            if (_collider != null)
                _collider.enabled = false;

            transform.position = new Vector3(_currentPosition.x, _currentPosition.y, transform.position.z);
            transform.right = _travelDirection;
        }

        private void ManualSimulate(float deltaTime)
        {
            float remainingTime = deltaTime;

            while (remainingTime > Mathf.Epsilon && _isActive)
            {
                float stepTime = Mathf.Min(_manualSimStep, remainingTime);
                float stepDistance = _currentSpeed * stepTime;
                if (stepDistance <= 0.0001f)
                    break;

                RaycastHit2D hit = Physics2D.Raycast(_currentPosition, _travelDirection, stepDistance, _bounceMask);
                if (hit.collider == null)
                {
                    _currentPosition += _travelDirection * stepDistance;
                    remainingTime -= stepTime;
                    continue;
                }

                Collider2D hitCollider = hit.collider;
                float travelledTime = stepTime * (hit.distance / Mathf.Max(stepDistance, 0.0001f));
                remainingTime -= Mathf.Max(travelledTime, 0f);
                _currentPosition = hit.point;

                if (TryDamageTarget(hitCollider))
                {
                    FinishProjectile();
                    break;
                }

                if (hitCollider.TryGetComponent(out CardObstacle card))
                    card.HandleHit();

                if (hitCollider.isTrigger)
                {
                    _currentPosition += _travelDirection * Mathf.Max(_surfaceOffset, 0.001f);
                    continue;
                }

                bool canBounce = _remainingBounces > 0 && ((_bounceMask.value & (1 << hitCollider.gameObject.layer)) != 0);
                if (canBounce)
                {
                    _remainingBounces--;
                    _travelDirection = Vector2.Reflect(_travelDirection, hit.normal).normalized;
                    _currentPosition = hit.point + hit.normal * _surfaceOffset;
                    continue;
                }

                FinishProjectile();
                break;
            }

            transform.position = new Vector3(_currentPosition.x, _currentPosition.y, transform.position.z);
            transform.right = _travelDirection;
        }

        private bool TryDamageTarget(Collider2D target)
        {
            if (target != null && target.TryGetComponent(out CharacterHealth health))
            {
                health.TakeDamage(_damage);
                return true;
            }

            return false;
        }

        private void FinishProjectile()
        {
            if (_isActive == false)
                return;

            _isActive = false;
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector2.zero;
                _rigidbody.bodyType = RigidbodyType2D.Kinematic;
                _rigidbody.simulated = false;
            }

            if (_collider != null)
                _collider.enabled = false;

            Destroy(gameObject, _destroyDelayAfterStop);
        }
    }
}
