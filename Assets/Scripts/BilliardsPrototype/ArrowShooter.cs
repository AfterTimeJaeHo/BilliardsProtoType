using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public class ArrowShooter : MonoBehaviour
    {
        public static event Action onArrowFired = delegate { };
        private static readonly Vector3 LineOffset = new Vector3(0f, 0f, -0.1f);

        [Header("References")]
        [SerializeField] private Camera _aimCamera;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private ArrowProjectile _arrowPrefab;
        [SerializeField] private string _arrowPrefabResource = "Prefabs/ArrowProjectile";
        [SerializeField] private LineRenderer _trajectoryLine;

        [Header("Charge Settings")]
        [SerializeField] private KeyCode _chargeKey = KeyCode.Mouse0;
        [SerializeField] private float _minLaunchSpeed = 8f;
        [SerializeField] private float _maxLaunchSpeed = 20f;
        [SerializeField] private float _maxChargeTime = 1.2f;
        [SerializeField] private AnimationCurve _chargeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Trajectory")]
        [SerializeField] private LayerMask _collisionMask = ~0;
        [SerializeField] private LayerMask _trajectoryReflectionMask = ~0;
        [SerializeField] private float _previewTravelTime = 1.8f;
        [SerializeField] private int _maxBounceCount = 5;
        [SerializeField] private float _timeBetweenShots = 0.25f;

        private readonly List<Vector3> _previewPoints = new List<Vector3>(8);

        private bool _isCharging;
        private float _currentChargeTime;
        private float _shotCooldown;

        private void Awake()
        {
            if (_aimCamera == null)
                _aimCamera = Camera.main;

            if (_firePoint == null)
            {
                Transform found = transform.Find("FirePoint");
                _firePoint = found != null ? found : transform;
            }

            if (_trajectoryLine == null)
                _trajectoryLine = GetComponent<LineRenderer>();

            if (_arrowPrefab == null && string.IsNullOrWhiteSpace(_arrowPrefabResource) == false)
                _arrowPrefab = Resources.Load<ArrowProjectile>(_arrowPrefabResource);

            ToggleTrajectory(false);
        }

        private void Update()
        {
            if (_shotCooldown > 0f)
                _shotCooldown -= Time.deltaTime;

            if (Input.GetKeyDown(_chargeKey) && _shotCooldown <= 0f)
                BeginCharge();

            if (_isCharging == false)
                return;

            _currentChargeTime = Mathf.Min(_currentChargeTime + Time.deltaTime, _maxChargeTime);
            Vector2 aimDirection = GetAimDirection();
            float launchSpeed = EvaluateLaunchSpeed();
            UpdateTrajectoryPreview(aimDirection, launchSpeed);

            if (Input.GetKeyUp(_chargeKey))
                LaunchArrow(aimDirection, launchSpeed);
        }

        private void BeginCharge()
        {
            _isCharging = true;
            _currentChargeTime = 0f;
            ToggleTrajectory(true);
        }

        private Vector2 GetAimDirection()
        {
            if (_aimCamera == null)
                return _firePoint.right;

            Vector3 pointerWorld = _aimCamera.ScreenToWorldPoint(Input.mousePosition);
            pointerWorld.z = _firePoint.position.z;
            Vector2 direction = pointerWorld - _firePoint.position;
            if (direction.sqrMagnitude < 0.0001f)
                direction = _firePoint.right;
            return direction.normalized;
        }

        private float EvaluateLaunchSpeed()
        {
            float chargePercent = Mathf.Approximately(_maxChargeTime, 0f) ? 1f : _currentChargeTime / _maxChargeTime;
            float curved = Mathf.Clamp01(_chargeCurve.Evaluate(chargePercent));
            return Mathf.Lerp(_minLaunchSpeed, _maxLaunchSpeed, curved);
        }

        private void LaunchArrow(Vector2 direction, float launchSpeed)
        {
            _isCharging = false;
            _shotCooldown = _timeBetweenShots;
            ToggleTrajectory(false);

            onArrowFired();

            if (_arrowPrefab == null)
            {
                Debug.LogWarning("ArrowShooter에 ArrowProjectile 프리팹이 지정되지 않았습니다.");
                return;
            }

            ArrowProjectile arrow = Instantiate(_arrowPrefab, _firePoint.position, Quaternion.identity);
            arrow.Initialize(direction, launchSpeed, _maxBounceCount, _collisionMask);
        }

private void UpdateTrajectoryPreview(Vector2 direction, float launchSpeed)
        {
            if (_trajectoryLine == null || _firePoint == null)
                return;

            _previewPoints.Clear();
            Vector3 start = _firePoint.position;
            _previewPoints.Add(start + LineOffset);

            Vector2 position = _firePoint.position;
            Vector2 normalizedDirection = direction.sqrMagnitude < Mathf.Epsilon ? (Vector2)_firePoint.right : direction.normalized;
            float speed = Mathf.Max(launchSpeed, 0f);
            if (speed <= 0.0001f)
            {
                _trajectoryLine.positionCount = 1;
                _trajectoryLine.SetPosition(0, start + LineOffset);
                return;
            }

            Vector2 velocity = normalizedDirection * speed;
            float remainingTime = Mathf.Max(0f, _previewTravelTime);
            float simStep = Mathf.Max(Time.fixedDeltaTime, 0.02f);
            int reflectionsRemaining = Mathf.Max(0, _maxBounceCount);
            int iterationGuard = Mathf.CeilToInt(remainingTime / simStep) + reflectionsRemaining + 8;

            while (remainingTime > Mathf.Epsilon && iterationGuard-- > 0)
            {
                float stepTime = Mathf.Min(simStep, remainingTime);
                float stepDistance = velocity.magnitude * stepTime;
                if (stepDistance <= 0.0001f)
                    break;

                Vector2 directionThisStep = velocity.normalized;
                RaycastHit2D hit = Physics2D.Raycast(position, directionThisStep, stepDistance, _collisionMask);
                if (hit.collider == null || ShouldIgnorePreviewCollider(hit.collider))
                {
                    position += directionThisStep * stepDistance;
                    _previewPoints.Add(new Vector3(position.x, position.y, start.z) + LineOffset);
                    remainingTime -= stepTime;
                    continue;
                }

                float timeToHit = hit.distance / Mathf.Max(velocity.magnitude, 0.0001f);
                position = hit.point;
                _previewPoints.Add(new Vector3(position.x, position.y, start.z) + LineOffset);
                remainingTime -= timeToHit;

                bool canReflect = reflectionsRemaining > 0 && ShouldReflect(hit.collider);
                if (canReflect)
                {
                    reflectionsRemaining--;
                    velocity = Vector2.Reflect(directionThisStep, hit.normal).normalized * velocity.magnitude;
                    position += hit.normal * 0.01f;
                    continue;
                }

                break;
            }

            _trajectoryLine.positionCount = _previewPoints.Count;
            _trajectoryLine.SetPositions(_previewPoints.ToArray());
        }

        private bool ShouldReflect(Collider2D collider)
        {
            if (collider == null)
                return false;

            if (collider.isTrigger)
                return false;

            if (collider.GetComponent<CardObstacle>() != null)
                return false;

            if (collider.GetComponent<CharacterHealth>() != null)
                return false;

            int layerMask = 1 << collider.gameObject.layer;
            return (_trajectoryReflectionMask.value & layerMask) != 0;
        }

        private void ToggleTrajectory(bool isVisible)
        {
            if (_trajectoryLine == null)
                return;

            _trajectoryLine.enabled = isVisible;
            if (isVisible == false)
                _trajectoryLine.positionCount = 0;
        }
    

private bool ShouldIgnorePreviewCollider(Collider2D collider)
        {
            if (collider == null)
                return true;

            if (collider.isTrigger)
                return true;

            if (collider.GetComponent<CardObstacle>() != null)
                return true;

            if (collider.GetComponent<CharacterHealth>() != null)
                return true;

            if (collider.CompareTag("Enemy"))
                return true;

            string objectName = collider.gameObject.name;
            return objectName.IndexOf("enemy", StringComparison.OrdinalIgnoreCase) >= 0;
        }
}
}

