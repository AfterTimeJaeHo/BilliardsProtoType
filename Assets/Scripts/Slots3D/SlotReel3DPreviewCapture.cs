using UnityEngine;
using UnityEngine.UI;

namespace Waving.SlotGame.Slots3D
{
    [DisallowMultipleComponent]
    public class SlotReel3DPreviewCapture : MonoBehaviour
    {
        [SerializeField] private SlotReel3DPreview _previewSource;
        [SerializeField] private Camera _captureCamera;
        [SerializeField] private RawImage _targetImage;
        [SerializeField] private Vector2Int _renderSize = new Vector2Int(512, 512);
        [SerializeField] private bool _applyPreviewLayer = true;
        [SerializeField] private int _previewLayer = 30;

        private RenderTexture _renderTexture;

        public RenderTexture RenderTexture => _renderTexture;

        private void Awake()
        {
            if (_previewSource == null)
            {
                _previewSource = GetComponent<SlotReel3DPreview>();
            }
        }

        private void OnEnable()
        {
            ApplyPreviewLayer();
            SetupRenderTarget();
        }

        private void OnDisable()
        {
            ReleaseRenderTarget();
            if (_captureCamera != null)
            {
                _captureCamera.targetTexture = null;
            }

            if (_targetImage != null)
            {
                _targetImage.texture = null;
            }
        }

        private void OnValidate()
        {
            _renderSize.x = Mathf.Max(1, _renderSize.x);
            _renderSize.y = Mathf.Max(1, _renderSize.y);
        }

        private void SetupRenderTarget()
        {
            if (_captureCamera == null || _targetImage == null)
            {
                return;
            }

            ReleaseRenderTarget();

            int width = Mathf.Max(1, _renderSize.x);
            int height = Mathf.Max(1, _renderSize.y);
            _renderTexture = new RenderTexture(width, height, 16)
            {
                name = $"{name}_SlotPreviewRT",
                antiAliasing = 2
            };
            _renderTexture.Create();

            _captureCamera.targetTexture = _renderTexture;
            _targetImage.texture = _renderTexture;
        }

        private void ReleaseRenderTarget()
        {
            if (_renderTexture == null)
            {
                return;
            }

            if (_captureCamera != null && _captureCamera.targetTexture == _renderTexture)
            {
                _captureCamera.targetTexture = null;
            }

            if (_targetImage != null && _targetImage.texture == _renderTexture)
            {
                _targetImage.texture = null;
            }

            _renderTexture.Release();
            if (Application.isPlaying)
            {
                Destroy(_renderTexture);
            }
            else
            {
                DestroyImmediate(_renderTexture);
            }

            _renderTexture = null;
        }

        private void ApplyPreviewLayer()
        {
            if (!_applyPreviewLayer || _previewSource == null)
            {
                return;
            }

            if (_previewLayer < 0 || _previewLayer > 31)
            {
                return;
            }

            SetLayerRecursively(_previewSource.transform, _previewLayer);
            if (_captureCamera != null)
            {
                _captureCamera.cullingMask = 1 << _previewLayer;
            }
        }

        private void SetLayerRecursively(Transform target, int layer)
        {
            target.gameObject.layer = layer;
            int childCount = target.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = target.GetChild(i);
                SetLayerRecursively(child, layer);
            }
        }
    }
}
