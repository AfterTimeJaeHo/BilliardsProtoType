using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    [ExecuteAlways]
    public class CameraBoundsController : MonoBehaviour
    {
        [System.Serializable]
        private class WallElement
        {
            public string fallbackName;
            public Transform root;
            public BoxCollider2D collider;
        }

        [SerializeField] private Camera _targetCamera;
        [SerializeField] private string _cameraFallbackName = "Main Camera";
        [SerializeField] private Transform _background;
        [SerializeField] private string _backgroundFallbackName = "Background";
        [SerializeField] private Vector2 _backgroundPadding = new Vector2(1.5f, 1.5f);
        [SerializeField] private float _wallThickness = 0.6f;
        [Header("Wall Roots")]
        [SerializeField] private WallElement _leftWall = new WallElement { fallbackName = "Wall_Left" };
        [SerializeField] private WallElement _rightWall = new WallElement { fallbackName = "Wall_Right" };
        [SerializeField] private WallElement _topWall = new WallElement { fallbackName = "Wall_Top" };
        [SerializeField] private WallElement _bottomWall = new WallElement { fallbackName = "Wall_Bottom" };

        private void OnEnable()
        {
            ResolveSceneReferences();
        }

        private void Awake()
        {
            ResolveSceneReferences();
        }

        private void LateUpdate()
        {
            if (_targetCamera == null)
                ResolveCamera();

            if (_targetCamera == null)
                return;

            Vector3 cameraPosition = _targetCamera.transform.position;
            float verticalExtent = _targetCamera.orthographicSize;
            float horizontalExtent = verticalExtent * _targetCamera.aspect;

            UpdateBackground(cameraPosition, horizontalExtent, verticalExtent);
            UpdateVerticalWall(_leftWall, cameraPosition, -horizontalExtent);
            UpdateVerticalWall(_rightWall, cameraPosition, horizontalExtent);
            UpdateHorizontalWall(_topWall, cameraPosition, verticalExtent);
            UpdateHorizontalWall(_bottomWall, cameraPosition, -verticalExtent);
        }

        private void ResolveSceneReferences()
        {
            ResolveWall(_leftWall);
            ResolveWall(_rightWall);
            ResolveWall(_topWall);
            ResolveWall(_bottomWall);
            ResolveBackground();
            ResolveCamera();
        }

        private void ResolveBackground()
        {
            if (_background == null && string.IsNullOrWhiteSpace(_backgroundFallbackName) == false)
            {
                GameObject foundBackground = GameObject.Find(_backgroundFallbackName);
                if (foundBackground != null)
                    _background = foundBackground.transform;
            }
        }

        private void ResolveCamera()
        {
            if (_targetCamera != null)
                return;

            _targetCamera = Camera.main;
            if (_targetCamera == null && string.IsNullOrWhiteSpace(_cameraFallbackName) == false)
            {
                GameObject foundCamera = GameObject.Find(_cameraFallbackName);
                if (foundCamera != null)
                    _targetCamera = foundCamera.GetComponent<Camera>();
            }
        }

        private void UpdateBackground(Vector3 cameraPosition, float horizontalExtent, float verticalExtent)
        {
            if (_background == null)
                return;

            Vector3 backgroundScale = _background.localScale;
            backgroundScale.x = horizontalExtent * 2f + _backgroundPadding.x;
            backgroundScale.y = verticalExtent * 2f + _backgroundPadding.y;
            _background.localScale = backgroundScale;
            _background.position = new Vector3(cameraPosition.x, cameraPosition.y, _background.position.z);
        }

        private void UpdateVerticalWall(WallElement wall, Vector3 cameraPosition, float horizontalOffset)
        {
            if (wall?.root == null || _targetCamera == null)
                return;

            float height = _targetCamera.orthographicSize * 2f + _backgroundPadding.y;
            float direction = Mathf.Sign(Mathf.Approximately(horizontalOffset, 0f) ? 1f : horizontalOffset);
            float xPosition = cameraPosition.x + horizontalOffset - direction * (_wallThickness * 0.5f);
            wall.root.position = new Vector3(xPosition, cameraPosition.y, wall.root.position.z);
            wall.root.localScale = new Vector3(_wallThickness, height, 1f);

            if (wall.collider != null)
            {
                wall.collider.size = Vector2.one;
                wall.collider.offset = Vector2.zero;
            }
        }

        private void UpdateHorizontalWall(WallElement wall, Vector3 cameraPosition, float verticalOffset)
        {
            if (wall?.root == null || _targetCamera == null)
                return;

            float width = _targetCamera.orthographicSize * _targetCamera.aspect * 2f + _backgroundPadding.x;
            float direction = Mathf.Sign(Mathf.Approximately(verticalOffset, 0f) ? 1f : verticalOffset);
            float yPosition = cameraPosition.y + verticalOffset - direction * (_wallThickness * 0.5f);
            wall.root.position = new Vector3(cameraPosition.x, yPosition, wall.root.position.z);
            wall.root.localScale = new Vector3(width, _wallThickness, 1f);

            if (wall.collider != null)
            {
                wall.collider.size = Vector2.one;
                wall.collider.offset = Vector2.zero;
            }
        }

        private void ResolveWall(WallElement element)
        {
            if (element == null)
                return;

            if (element.root == null && string.IsNullOrWhiteSpace(element.fallbackName) == false)
            {
                GameObject found = GameObject.Find(element.fallbackName);
                if (found != null)
                    element.root = found.transform;
            }

            if (element.root != null && element.collider == null)
                element.collider = element.root.GetComponent<BoxCollider2D>();
        }
    }
}
