using System.Collections.Generic;
using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public class CardIconDisplay : MonoBehaviour
    {
        [SerializeField] private Transform _anchor;
        [SerializeField] private float _startHeight = 1.2f;
        [SerializeField] private float _spacing = 0.35f;
        [SerializeField] private Vector3 _iconScale = Vector3.one * 0.35f;
        [SerializeField] private int _sortingOrder = 25;

        private readonly List<SpriteRenderer> _icons = new List<SpriteRenderer>();

        private void Awake()
        {
            if (_anchor == null)
            {
                GameObject anchorObject = new GameObject("CardIconAnchor");
                anchorObject.transform.SetParent(transform, false);
                _anchor = anchorObject.transform;
            }
        }

        private void OnEnable()
        {
            CardObstacle.onCardCollected += HandleCollected;
            ArrowShooter.onArrowFired += ResetIcons;
        }

        private void OnDisable()
        {
            CardObstacle.onCardCollected -= HandleCollected;
            ArrowShooter.onArrowFired -= ResetIcons;
        }

        private void HandleCollected(Sprite sprite)
        {
            if (_anchor == null || sprite == null)
                return;

            GameObject iconObject = new GameObject($"CardIcon_{_icons.Count}");
            iconObject.transform.SetParent(_anchor, false);
            iconObject.transform.localPosition = Vector3.up * (_startHeight + _spacing * _icons.Count);
            iconObject.transform.localScale = _iconScale;

            SpriteRenderer renderer = iconObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = _sortingOrder;

            _icons.Add(renderer);
        }

        public void ResetIcons()
        {
            for (int i = 0; i < _icons.Count; i++)
            {
                if (_icons[i] != null)
                    Destroy(_icons[i].gameObject);
            }

            _icons.Clear();
        }
    }
}
