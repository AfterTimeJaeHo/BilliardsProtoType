using UnityEngine;

namespace Aftertime.SecretSome.Common
{
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField] private Vector2 _ratio;
        private float _targetAspect => _ratio.x / _ratio.y;
        private int _lastWidth;
        private int _lastHeight;

        private void Start()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            AdjustScale();
            UpdateExecutor.onUpdate += CheckResolutionChange;
        }

        private void CheckResolutionChange()
        {
            if(_lastWidth != Screen.width || _lastHeight != Screen.height)
            {
                _lastWidth = Screen.width;
                _lastHeight = Screen.height;
                AdjustScale();
            }
        }

        private void AdjustScale()
        {
            Camera cam = Camera.main;

            float windowAspect = (float)Screen.width / (float)Screen.height;
            float scaleHeight = windowAspect / _targetAspect;

            if (scaleHeight < 1f)
            {
                Rect rect = cam.rect;

                rect.width = 1f;
                rect.height = scaleHeight;
                rect.x = 0f;
                rect.y = (1f - scaleHeight) / 2f;

                cam.rect = rect;
            }
            else
            {
                float scaleWidth = 1f / scaleHeight;

                Rect rect = cam.rect;

                rect.width = scaleWidth;
                rect.height = 1f;
                rect.x = (1f - scaleWidth) / 2f;
                rect.y = 0f;

                cam.rect = rect;
            }
        }
    }   
}
