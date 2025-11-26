using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Waving.Common
{
    public class SpriteGroup : MonoBehaviour
    {
        public float Alpha
        {
            get => alpha;
            set
            {
                alpha = value;
                ChangeSpriteRendererAlpha();
            }
        }

        [Range(0f, 1f)] [SerializeField] private float alpha = 1;

        [SerializeField] private List<SpriteRenderer> _spriteRenderers;

        public async UniTask DOFade(float value, float duration)
        {
            if (_spriteRenderers.Count == 0)
                _spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();

            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.DOFade(value, duration);
            }

            alpha = value;

            if (duration != 0)
                await UniTask.WaitForSeconds(duration);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ChangeSpriteRendererAlpha();
        }
#endif

        private void ChangeSpriteRendererAlpha()
        {
            if (_spriteRenderers.Count == 0)
                _spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();

            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                Color originColor = spriteRenderer.color;
                Color changeColor = new Color(originColor.r, originColor.g, originColor.b, alpha);
                spriteRenderer.color = changeColor;
            }
        }
    }
}