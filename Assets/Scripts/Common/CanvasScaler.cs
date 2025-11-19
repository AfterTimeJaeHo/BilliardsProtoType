using Unity.Mathematics.Geometry;
using UnityEngine;

namespace Aftertime.TeachingAssistantLife.UI
{
    public class CanvasScaler : UnityEngine.UI.CanvasScaler
    {
        private float _targetAspect => referenceResolution.x / referenceResolution.y;

        protected override void Start()
        {
            base.Start();
            AdjustScale();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            AdjustScale();
        }


        private void AdjustScale()
        {
            float windowAspect = (float)Screen.width / (float)Screen.height;
            if (windowAspect >= _targetAspect)
            {
                matchWidthOrHeight = 1;
            }
            else
            {
                matchWidthOrHeight = 0;
            }
        }
    }
}