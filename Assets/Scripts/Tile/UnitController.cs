using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Waving.Tile
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] private GridMap _gridMap;
        [SerializeField] private float _moveDurationPerTile = 0.15f;
        private bool _isMoving;

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
            List<Vector3Int> path = GetPath();

            if (path == null || path.Count == 0)
                return;

            _isMoving = true;
            await DoMove();
            _isMoving = false;

            List<Vector3Int> GetPath()
            {
                // 마우스 or 터치 좌표
                Vector2 screenPos = UIInputAction.Instance.UI.Point.ReadValue<Vector2>();
                float depthToPlane = 0f - Camera.main.transform.position.z; // 카메라 z축 거리 보완 
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, depthToPlane));

                Vector3Int targetCell = _gridMap.WorldToCell(worldPos);
                targetCell.z = 0;

                Vector3Int currentCell = _gridMap.WorldToCell(transform.position);
                currentCell.z = 0;

                if (!_gridMap.IsWalkable(targetCell))
                    return null;

                List<Vector3Int> path = PathFinder.FindPath(_gridMap, currentCell, targetCell);

                return path;
            }
            
            async UniTask DoMove()
            {
                foreach (var cell in path)
                {
                    Vector3 targetWorld = _gridMap.CellToWorldCenter(cell);

                    await transform.DOMove(targetWorld, _moveDurationPerTile)
                        .SetEase(Ease.Linear)
                        .ToUniTask();
                }

            }
        }
    }
   
}