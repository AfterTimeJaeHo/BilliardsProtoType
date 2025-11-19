using UnityEngine;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpritePainter : MonoBehaviour
    {
        [SerializeField] private string _resourcesPath = "Textures/White";
        [SerializeField] private Color _tint = Color.white;

        private void Awake()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Sprite sprite = Resources.Load<Sprite>(_resourcesPath);
            if (sprite != null)
                spriteRenderer.sprite = sprite;

            spriteRenderer.color = _tint;
        }
    }
}
