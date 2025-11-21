using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Waving.Tile
{
    public class GridMap : MonoBehaviour
    {
        [SerializeField] private Tilemap _groundTilemap;

        /// 월드 좌표 → 셀 좌표
        public Vector3Int WorldToCell(Vector3 worldPos)
        {
            return _groundTilemap.WorldToCell(worldPos);
        }

        /// 셀 좌표 → 타일 중앙 월드 좌표
        public Vector3 CellToWorldCenter(Vector3Int cell)
        {
            // 아이소메트릭은 셀 좌표 기준으로 바로 쓰면 됨
            Vector3 world = _groundTilemap.GetCellCenterWorld(cell);
            return world;
        }

        /// 해당 셀이 이동 가능한지 여부
        public bool IsWalkable(Vector3Int cell)
        {
            // 바닥 타일이 없으면 못 감
            if (!_groundTilemap.HasTile(cell))
                return false;

            return true;
        }
    }
   
}