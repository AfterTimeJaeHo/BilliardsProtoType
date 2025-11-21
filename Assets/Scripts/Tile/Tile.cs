using System.Collections.Generic;
using UnityEngine;

namespace Aftertime.MyTinyStreamer.Tile
{
    /// <summary>
    /// 타일 데이터와 이동 정보를 저장합니다.
    /// </summary>
    [DisallowMultipleComponent]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private bool _blocksMovement;
        [SerializeField] private TileType _tileType = TileType.Ground;
        [SerializeField] private List<Tile> _nextTiles = new List<Tile>();
        [SerializeField] private Vector3Int _tilemapCell;

        public SpriteRenderer Renderer => _renderer;
        public bool BlocksMovement => _blocksMovement;
        public TileType TileType => _tileType;
        public IReadOnlyList<Tile> NextTiles => _nextTiles;
        public Vector3Int Cell => _tilemapCell;

        private void Awake()
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<SpriteRenderer>();
            }
        }

        /// <summary>
        /// 타일 정보를 초기화합니다.
        /// </summary>
        public void InitTile(SpriteRenderer renderer, Vector3Int cell)
        {
            _renderer = renderer;
            _tilemapCell = cell;
        }

        /// <summary>
        /// 타일맵 좌표를 갱신합니다.
        /// </summary>
        public void UpdateCell(Vector3Int cell)
        {
            _tilemapCell = cell;
        }

        /// <summary>
        /// 다음 타일 목록을 설정합니다.
        /// </summary>
        public void SetNextTiles(List<Tile> nextTiles)
        {
            _nextTiles = nextTiles != null ? new List<Tile>(nextTiles) : new List<Tile>();
        }

        /// <summary>
        /// 다음 타일 목록을 비웁니다.
        /// </summary>
        public void ClearNextTiles()
        {
            _nextTiles.Clear();
        }
    }

    public enum TileType
    {
        Ground,
        Obstacle,
        Enemy
    }
}
