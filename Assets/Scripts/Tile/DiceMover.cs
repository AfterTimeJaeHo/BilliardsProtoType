using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Aftertime.MyTinyStreamer.Tile
{
    /// <summary>
    /// 주사위를 굴려 나온 숫자만큼 타일을 이동합니다.
    /// - 갈림길에서 멈추고 후보 타일들을 노란색으로 하이라이트한 뒤, 클릭 선택 방향으로 계속 진행합니다.
    /// - GridMap의 IsWalkable을 사용하며, 4방향(상하좌우)만 이동합니다.
    /// </summary>
    public class DiceMover : MonoBehaviour
    {
        public event Action onMoveComplete;
            
        [SerializeField] private Waving.Tile.GridMap _gridMap;
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private float _moveDurationPerTile = 0.15f;
        [SerializeField] private Color _forkHighlightColor = new Color(1f, 1f, 0f, 0.9f);
        [SerializeField] private bool _useYBasedSorting = true;
        [SerializeField] private int _randomSeed = 0;
        [SerializeField] private Button _diceButton;
        [SerializeField] private List<Sprite> _diceSprites;

        private readonly Vector3Int[] _neighborDirs = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0)
        };

        private bool _isMoving;
        private Vector3Int? _lastCell; // 직전 셀(되돌림 방지)
        private List<Vector3Int> _highlighted = new List<Vector3Int>();
        private readonly Dictionary<Vector3Int, Color> _savedColors = new Dictionary<Vector3Int, Color>();
        private CancellationTokenSource _cts;

        private System.Random _rng;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
            _rng = _randomSeed == 0 ? new System.Random() : new System.Random(_randomSeed);
            RegisterDiceEvent();
        }

        private void OnDisable()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            HideForkHighlights();
        }

        private void RegisterDiceEvent()
        {
            _diceButton.onClick.AddListener(RollAndMove);
        }

        /// <summary>
        /// 주사위를 1회 굴려(1~6) 이동을 시작합니다.
        /// </summary>
        public async void RollAndMove()
        {
            if (_isMoving)
                return;

            int steps = _rng.Next(1, 7);
            int diceSpriteIndex = steps - 1;

            Sprite originDiceSprite = _diceButton.image.sprite;
            _diceButton.image.sprite = _diceSprites[diceSpriteIndex];
            MoveByStepsAsync(steps);
            _diceButton.image.sprite = originDiceSprite;
        }

        /// <summary>
        /// 지정한 스텝 수만큼 이동. 갈림길에서는 멈추고 방향 선택을 기다립니다.
        /// </summary>
        private async void MoveByStepsAsync(int steps)
        {
            if (_gridMap == null || _groundTilemap == null)
                return;

            _isMoving = true;

            try
            {
                Vector3Int current = _gridMap.WorldToCell(transform.position);
                current.z = 0;

                for (int i = 0; i < steps; i++)
                {
                    if (_cts == null || _cts.IsCancellationRequested)
                        break;

                    List<Vector3Int> candidates = GetMoveCandidates(current, _lastCell);

                    // 갈림길: 후보 2개 이상이면 사용자 선택 대기
                    if (candidates.Count >= 2)
                    {
                        Vector3Int? selected = await SelectForkAsync(candidates, _cts.Token);
                        if (selected.HasValue == false)
                            break;

                        Vector3Int next = selected.Value;
                        await MoveOneStepAsync(next, _cts.Token);
                        _lastCell = current;
                        current = next;
                        continue;
                    }

                    // 후보 1개: 자동 진행
                    if (candidates.Count == 1)
                    {
                        Vector3Int next = candidates[0];
                        await MoveOneStepAsync(next, _cts.Token);
                        _lastCell = current;
                        current = next;
                        continue;
                    }

                    // 막다른 길: 진행 종료 (되돌아갈 수밖에 없으면 허용)
                    if (_lastCell.HasValue)
                    {
                        List<Vector3Int> fallback = GetMoveCandidatesAllowBack(current, _lastCell);
                        if (fallback.Count > 0)
                        {
                            Vector3Int next = fallback[0];
                            await MoveOneStepAsync(next, _cts.Token);
                            _lastCell = current;
                            current = next;
                            continue;
                        }
                    }

                    // 더 이상 진행 불가
                    break;
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                HideForkHighlights();
                _isMoving = false;
                onMoveComplete?.Invoke();
            }
        }

        /// <summary>
        /// 현재 셀에서 이전 셀 방향을 제외한 이동 후보를 반환합니다.
        /// </summary>
        private List<Vector3Int> GetMoveCandidates(Vector3Int current, Vector3Int? last)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            for (int i = 0; i < _neighborDirs.Length; i++)
            {
                Vector3Int n = current + _neighborDirs[i];
                if (last.HasValue && n == last.Value)
                    continue;
                if (_gridMap.IsWalkable(n))
                {
                    result.Add(n);
                }
            }
            return result;
        }

        /// <summary>
        /// 되돌림도 허용하는 후보 (막다른 길에서만 사용)
        /// </summary>
        private List<Vector3Int> GetMoveCandidatesAllowBack(Vector3Int current, Vector3Int? last)
        {
            List<Vector3Int> result = new List<Vector3Int>();
            for (int i = 0; i < _neighborDirs.Length; i++)
            {
                Vector3Int n = current + _neighborDirs[i];
                if (_gridMap.IsWalkable(n))
                {
                    result.Add(n);
                }
            }

            // 이전 방향만 남으면 1개로 축소
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

        /// <summary>
        /// 한 칸 이동 애니메이션
        /// </summary>
        private async UniTask MoveOneStepAsync(Vector3Int targetCell, CancellationToken token)
        {
            Vector3 targetWorld = _gridMap.CellToWorldCenter(targetCell);
            await transform.DOMove(targetWorld, _moveDurationPerTile)
                .SetEase(Ease.Linear)
                .ToUniTask(cancellationToken: token);
        }

        /// <summary>
        /// 갈림길 후보를 하이라이트하고 사용자 클릭으로 한 칸을 선택받습니다.
        /// </summary>
        private async UniTask<Vector3Int?> SelectForkAsync(List<Vector3Int> candidates, CancellationToken token)
        {
            ShowForkHighlights(candidates);

            Vector3Int? selected = null;
            bool waiting = true;

            void OnClick(InputAction.CallbackContext ctx)
            {
                if (waiting == false) return;

                Vector2 screenPos = UIInputAction.Instance.UI.Point.ReadValue<Vector2>();
                float depth = -Camera.main.transform.position.z;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, depth));
                Vector3Int cell = _gridMap.WorldToCell(worldPos);
                cell.z = 0;

                for (int i = 0; i < candidates.Count; i++)
                {
                    if (candidates[i] == cell)
                    {
                        selected = cell;
                        waiting = false;
                        break;
                    }
                }
            }

            try
            {
                UIInputAction.Instance.Global.LeftClick.performed += OnClick;
                await UniTask.WaitUntil(() => waiting == false, cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                selected = null;
            }
            finally
            {
                UIInputAction.Instance.Global.LeftClick.performed -= OnClick;
                HideForkHighlights();
            }

            return selected;
        }

        /// <summary>
        /// 후보 타일들의 색을 노란색으로 바꿉니다.
        /// </summary>
        private void ShowForkHighlights(List<Vector3Int> candidates)
        {
            HideForkHighlights();

            for (int i = 0; i < candidates.Count; i++)
            {
                Vector3Int cell = candidates[i];
                Color origin = _groundTilemap.GetColor(cell);
                if (_savedColors.ContainsKey(cell) == false)
                {
                    _savedColors[cell] = origin;
                }

                Color tint = _forkHighlightColor;
                if (_useYBasedSorting)
                {
                    float factor = 1f + (float)(-cell.y) * 0.02f;
                    tint *= factor;
                    tint.a = _forkHighlightColor.a; // 투명도는 유지
                }

                _groundTilemap.SetColor(cell, tint);
                _groundTilemap.RefreshTile(cell);
                _highlighted.Add(cell);
            }
        }

        /// <summary>
        /// 하이라이트를 원래 색으로 되돌립니다.
        /// </summary>
        private void HideForkHighlights()
        {
            if (_highlighted.Count == 0)
                return;

            for (int i = 0; i < _highlighted.Count; i++)
            {
                Vector3Int cell = _highlighted[i];
                Color origin;
                if (_savedColors.TryGetValue(cell, out origin))
                {
                    _groundTilemap.SetColor(cell, origin);
                }
                else
                {
                    _groundTilemap.SetColor(cell, Color.white);
                }
            }

            _highlighted.Clear();
            _savedColors.Clear();
        }
    }
}
