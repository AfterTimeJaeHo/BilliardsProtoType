using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Waving.MyTinyStreamer.Common;

namespace Aftertime.MyTinyStreamer.Tile
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private Tile _startTile;
        [SerializeField] private float _moveDurationPerTile = 0.15f;
        private bool _isMoving;
        private Tile _currentTile;

        private void Awake()
        {
            _currentTile = _startTile;
        }

        private void OnEnable()
        {
            UIInputAction.Instance.Global.LeftClick.performed += OnClick;
        }

        private void OnDisable()
        {
            UIInputAction.Instance.Global.LeftClick.performed -= OnClick;
        }

        private void OnClick(InputAction.CallbackContext ctx)
        {
            bool isOverlapUI = EventSystem.current.IsPointerOverGameObject();
            if (_isMoving || isOverlapUI)
                return;

            Move().Forget();
        }
        
        private async UniTaskVoid Move()
        {
            Tile target = GetTileFromCursor();
            Tile origin = _currentTile ?? _startTile;

            if (target == null || origin == null)
                return;

            if (target.BlocksMovement)
                return;

            List<Tile> path = PathFinder.FindPath(origin, target);
            if (path == null || path.Count == 0)
                return;

            _isMoving = true;
            await DoMove(path);
            _isMoving = false;
            _currentTile = target;
        }
        
        private async UniTask DoMove(List<Tile> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Tile tile = path[i];
                if (tile == null)
                {
                    continue;
                }

                Vector3 targetWorld = tile.transform.position;
                await transform.DOMove(targetWorld, _moveDurationPerTile)
                    .SetEase(Ease.Linear)
                    .ToUniTask();
            }
        }

        private Tile GetTileFromCursor()
        {
            Vector2 screenPos = UIInputAction.Instance.UI.Point.ReadValue<Vector2>();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            int layerMask = 1 << LayerMask.NameToLayer(Define.TileLayerName);

            RaycastHit2D hit = Physics2D.Raycast(
                worldPos,
                Vector2.zero,
                Mathf.Infinity,
                layerMask
            );

            if (hit.collider != null)
                return hit.collider.GetComponent<Tile>();

            return null;
        }
    }
}
