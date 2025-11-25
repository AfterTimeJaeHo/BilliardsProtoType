using System;
using System.Collections.Generic;
using System.Threading;
using Aftertime.SecretSome.Content;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Waving.Content;
using Waving.Tile;
using Random = System.Random;

namespace Aftertime.MyTinyStreamer.Tile
{
    public sealed class DiceMoveController : MonoBehaviour
    {
        [SerializeField] private GridMap _gridMap;
        [SerializeField] private Transform _moverRoot;
        [SerializeField] private float _moveDurationPerTile = 0.2f;
        [SerializeField] private Color _forkHighlightColor = Color.cyan;
        [SerializeField] private Vector3Int[] _neighborDirs =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        [Header("Dice UI")]
        [SerializeField] private Button _diceButton;
        [SerializeField] private Sprite[] _diceSprites = Array.Empty<Sprite>();

        [Header("Tiles")]
        [SerializeField] private List<Tile> _tiles = new List<Tile>();

        [Header("Events")]
        [SerializeField] private UnityEvent _onMoveComplete = new UnityEvent();
        [SerializeField] private CellEvent _onArrivedCell = new CellEvent();

        public event Action<GameObject> onEnemyFaced;

        private readonly List<Vector3Int> _forkCandidates = new List<Vector3Int>();
        private readonly List<Vector3Int> _graphCandidates = new List<Vector3Int>();
        private readonly HashSet<Vector3Int> _visitedCells = new HashSet<Vector3Int>();
        private readonly Dictionary<Vector3Int, Tile> _tileLookup = new Dictionary<Vector3Int, Tile>();
        private readonly Dictionary<Tile, Color> _highlightedTiles = new Dictionary<Tile, Color>();
        private readonly Random _rng = new Random();

        private CancellationTokenSource _cts;
        private bool _isMoving;
        private bool _waitingFork;
        private Vector3Int? _selectedCell;
        private Vector3Int? _lastCell;

        private Transform Mover => _moverRoot == null ? transform : _moverRoot;

        private void Awake()
        {
            BuildTileLookup();
        }

        private void OnEnable()
        {
            BuildTileLookup();

            if (_diceButton != null)
            {
                _diceButton.onClick.AddListener(RollAndMove);
            }

            UIInputAction.Instance.Global.LeftClick.performed += OnLeftClick;
            onEnemyFaced += (_) => ContentRunner.StartContent<BattleContent>();
        }

        private void OnDisable()
        {
            if (_diceButton != null)
            {
                _diceButton.onClick.RemoveListener(RollAndMove);
            }

            UIInputAction.Instance.Global.LeftClick.performed -= OnLeftClick;
            CancelMovement();
            HideForkHighlights();
        }

        private void OnDestroy()
        {
            CancelMovement();
        }

        public void RollAndMove()
        {
            if (_isMoving)
            {
                return;
            }

            RollAndMoveAsync().Forget();
        }

        private async UniTask RollAndMoveAsync()
        {
            if (_isMoving)
            {
                return;
            }

            int steps = _rng.Next(1, 7);
            int diceSpriteIndex = steps - 1;
            Sprite originDiceSprite = null;

            if (_diceButton != null && _diceSprites.Length > 0)
            {
                diceSpriteIndex = Mathf.Clamp(diceSpriteIndex, 0, _diceSprites.Length - 1);
                originDiceSprite = _diceButton.image.sprite;
                _diceButton.image.sprite = _diceSprites[diceSpriteIndex];
            }

            await MoveByStepsAsync(steps);

            if (originDiceSprite != null && _diceButton != null)
            {
                _diceButton.image.sprite = originDiceSprite;
            }
        }

        private async UniTask MoveByStepsAsync(int steps)
        {
            if (_gridMap == null || steps <= 0)
            {
                return;
            }

            CancelMovement();
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            _isMoving = true;
            SetDiceButtonState(false);

            Vector3Int current = _gridMap.WorldToCell(Mover.position);
            current.z = 0;

            _visitedCells.Clear();
            _visitedCells.Add(current);

            try
            {
                for (int i = 0; i < steps; i++)
                {
                    token.ThrowIfCancellationRequested();
                    List<Vector3Int> candidates = GetMoveCandidates(current, _lastCell);

                    if (candidates.Count >= 2)
                    {
                        IReadOnlyList<Vector3Int> directed = BuildGraphCandidates(current, _lastCell);
                        Vector3Int next;
                        if (directed.Count == 1)
                        {
                            next = directed[0];
                            await MoveOneStepAsync(next, token);
                            _lastCell = current;
                            current = next;
                            RaiseArrivalEvents(current);
                            _visitedCells.Add(current);
                            continue;
                        }

                        IReadOnlyList<Vector3Int> selectionPool = directed.Count >= 2 ? directed : candidates;

                        Vector3Int? selected = await SelectForkAsync(selectionPool, token);
                        if (selected.HasValue == false)
                        {
                            break;
                        }

                        next = selected.Value;
                        await MoveOneStepAsync(next, token);
                        _lastCell = current;
                        current = next;
                        RaiseArrivalEvents(current);
                        _visitedCells.Add(current);
                        continue;
                    }
                    if (candidates.Count == 1)
                    {
                        Vector3Int next = candidates[0];
                        await MoveOneStepAsync(next, token);
                        _lastCell = current;
                        current = next;
                        RaiseArrivalEvents(current);
                        _visitedCells.Add(current);
                        continue;
                    }

                    if (_lastCell.HasValue)
                    {
                        List<Vector3Int> fallback = GetMoveCandidatesAllowBack(current, _lastCell);
                        if (fallback.Count > 0)
                        {
                            Vector3Int next = fallback[0];
                            await MoveOneStepAsync(next, token);
                            _lastCell = current;
                            current = next;
                            RaiseArrivalEvents(current);
                            _visitedCells.Add(current);
                            continue;
                        }
                    }

                    break;
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                HideForkHighlights();
                _waitingFork = false;
                _selectedCell = null;
                _forkCandidates.Clear();
                RaiseEnemyEncounterIfNeeded(current);
                _isMoving = false;
                SetDiceButtonState(true);
                _cts?.Dispose();
                _cts = null;
                _onMoveComplete.Invoke();
            }
        }

        private List<Vector3Int> GetMoveCandidates(Vector3Int current, Vector3Int? last)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            for (int i = 0; i < _neighborDirs.Length; i++)
            {
                Vector3Int neighbor = current + _neighborDirs[i];

                if (last.HasValue && neighbor == last.Value)
                {
                    continue;
                }

                if (_gridMap.IsWalkable(neighbor) && !_visitedCells.Contains(neighbor))
                {
                    result.Add(neighbor);
                }
            }

            return result;
        }

        private void RaiseArrivalEvents(Vector3Int cell)
        {
            _onArrivedCell.Invoke(cell);
        }

        private void RaiseEnemyEncounterIfNeeded(Vector3Int cell)
        {
            Tile tile = FindTile(cell);
            if (tile != null && tile.TileType == TileType.Enemy)
            {
                GameObject enemy = tile.transform.GetChild(0).gameObject;
                onEnemyFaced?.Invoke(enemy);
            }
        }

        private List<Vector3Int> GetMoveCandidatesAllowBack(Vector3Int current, Vector3Int? last)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            for (int i = 0; i < _neighborDirs.Length; i++)
            {
                Vector3Int neighbor = current + _neighborDirs[i];
                if (_gridMap.IsWalkable(neighbor) && !_visitedCells.Contains(neighbor))
                {
                    result.Add(neighbor);
                }
            }

            if (result.Count > 1 && last.HasValue)
            {
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    if (result[i] != last.Value)
                    {
                        result.RemoveAt(i);
                    }
                }
            }

            return result;
        }

        private IReadOnlyList<Vector3Int> BuildGraphCandidates(Vector3Int current, Vector3Int? last)
        {
            _graphCandidates.Clear();

            Tile tile = FindTile(current);
            if (tile == null)
            {
                return _graphCandidates;
            }

            IReadOnlyList<Tile> nextTiles = tile.NextTiles;
            if (nextTiles == null || nextTiles.Count == 0)
            {
                return _graphCandidates;
            }

            for (int i = 0; i < nextTiles.Count; i++)
            {
                Tile nextTile = nextTiles[i];
                if (nextTile == null || nextTile.BlocksMovement)
                {
                    continue;
                }

                Vector3Int cell = nextTile.Cell;
                if (last.HasValue && cell == last.Value)
                {
                    continue;
                }

                if ((_gridMap != null && _gridMap.IsWalkable(cell) == false) || _visitedCells.Contains(cell))
                {
                    continue;
                }

                if (_graphCandidates.Contains(cell) == false)
                {
                    _graphCandidates.Add(cell);
                }
            }

            return _graphCandidates;
        }

        private async UniTask<Vector3Int?> SelectForkAsync(IReadOnlyList<Vector3Int> candidates, CancellationToken token)
        {
            _forkCandidates.Clear();
            for (int i = 0; i < candidates.Count; i++)
            {
                Vector3Int candidate = candidates[i];
                if (_forkCandidates.Contains(candidate) == false)
                {
                    _forkCandidates.Add(candidate);
                }
            }

            if (_forkCandidates.Count == 0)
            {
                return null;
            }

            _waitingFork = true;
            _selectedCell = null;
            ShowForkHighlights();

            try
            {
                await UniTask.WaitUntil(() => _selectedCell.HasValue, cancellationToken: token);
                return _selectedCell;
            }
            finally
            {
                HideForkHighlights();
                _waitingFork = false;
                _forkCandidates.Clear();
                _selectedCell = null;
            }
        }

        private async UniTask MoveOneStepAsync(Vector3Int nextCell, CancellationToken token)
        {
            Transform mover = Mover;
            Vector3 targetPosition = _gridMap.CellToWorldCenter(nextCell);

            await mover
                .DOMove(targetPosition, _moveDurationPerTile)
                .SetEase(Ease.Linear)
                .ToUniTask(cancellationToken: token);
        }

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            if (!_waitingFork)
            {
                return;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (TryPickForkCellFromPointer(out Vector3Int cell))
            {
                _selectedCell = cell;
            }
        }

        private bool TryPickForkCellFromPointer(out Vector3Int cell)
        {
            cell = default;
            if (_forkCandidates.Count == 0)
            {
                return false;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return false;
            }

            Vector2 pointer = UIInputAction.Instance.UI.Point.ReadValue<Vector2>();
            Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(pointer.x, pointer.y, 0f));

            for (int i = 0; i < _forkCandidates.Count; i++)
            {
                Vector3Int candidate = _forkCandidates[i];
                Tile tile = FindTile(candidate);
                SpriteRenderer renderer = tile != null ? tile.Renderer : null;
                if (renderer == null)
                {
                    continue;
                }

                Vector3 adjusted = worldPoint;
                adjusted.z = renderer.bounds.center.z;

                if (renderer.bounds.Contains(adjusted))
                {
                    cell = candidate;
                    return true;
                }
            }

            return false;
        }

        private void ShowForkHighlights()
        {
            for (int i = 0; i < _forkCandidates.Count; i++)
            {
                Vector3Int cell = _forkCandidates[i];
                Tile tile = FindTile(cell);
                SpriteRenderer renderer = tile != null ? tile.Renderer : null;
                if (renderer == null)
                {
                    continue;
                }

                if (_highlightedTiles.ContainsKey(tile) == false)
                {
                    _highlightedTiles[tile] = renderer.color;
                }

                renderer.color = _forkHighlightColor;
            }
        }

        private void HideForkHighlights()
        {
            foreach (KeyValuePair<Tile, Color> pair in _highlightedTiles)
            {
                if (pair.Key != null && pair.Key.Renderer != null)
                {
                    pair.Key.Renderer.color = pair.Value;
                }
            }

            _highlightedTiles.Clear();
        }

        private void BuildTileLookup()
        {
            _tileLookup.Clear();

            if (_tiles != null)
            {
                for (int i = 0; i < _tiles.Count; i++)
                {
                    Tile tile = _tiles[i];
                    if (tile == null)
                    {
                        continue;
                    }

                    _tileLookup[tile.Cell] = tile;
                }
            }

            if (_tileLookup.Count > 0)
            {
                return;
            }

            Tile[] foundTiles = FindObjectsOfType<Tile>();
            for (int i = 0; i < foundTiles.Length; i++)
            {
                Tile tile = foundTiles[i];
                if (tile == null)
                {
                    continue;
                }

                _tileLookup[tile.Cell] = tile;
            }
        }

        private Tile FindTile(Vector3Int cell)
        {
            if (_tileLookup.Count == 0)
            {
                BuildTileLookup();
            }

            _tileLookup.TryGetValue(cell, out Tile tile);
            return tile;
        }

        private void SetDiceButtonState(bool interactable)
        {
            if (_diceButton != null)
            {
                _diceButton.interactable = interactable;
            }
        }

        private void CancelMovement()
        {
            if (_cts == null)
            {
                return;
            }

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        [Serializable]
        private sealed class CellEvent : UnityEvent<Vector3Int>
        {
        }
    }
}
