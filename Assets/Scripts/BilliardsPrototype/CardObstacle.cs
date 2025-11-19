using System;
using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CardObstacle : MonoBehaviour
    {
        public static event Action<Sprite> onCardCollected = delegate { };

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider;

        public Sprite DisplaySprite => _spriteRenderer != null ? _spriteRenderer.sprite : null;

        private bool _isConsumed;

        private void Reset()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
        }

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_collider == null)
                _collider = GetComponent<Collider2D>();

            if (_collider != null)
                _collider.isTrigger = true;
        }

        public void HandleHit()
        {
            if (_isConsumed)
                return;

            _isConsumed = true;
            onCardCollected(DisplaySprite);

            if (_spriteRenderer != null)
                _spriteRenderer.enabled = false;

            if (_collider != null)
                _collider.enabled = false;

            gameObject.SetActive(false);
        }

        public void ResetCard()
        {
            _isConsumed = false;
            if (_spriteRenderer != null)
                _spriteRenderer.enabled = true;

            if (_collider != null)
            {
                _collider.enabled = true;
                _collider.isTrigger = true;
            }

            gameObject.SetActive(true);
        }
    }
}
