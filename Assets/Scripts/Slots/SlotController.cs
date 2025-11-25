using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Aftertime.MyTinyStreamer.Slots
{
    public class SlotEvaluateContainer
    {
        public int ShieldCount { get; private set; }
        public int SwordCount { get; private set; }
        public int MagicCount { get; private set; }

        public SlotEvaluateContainer(int swordCount, int shieldCount, int magicCount)
        {
            ShieldCount = shieldCount;
            SwordCount = swordCount;
            MagicCount = magicCount;
        }
    }

    // 슬롯 전투의 핵심 컨트롤러. UI와 전투 규칙을 관리
    public class SlotController : MonoBehaviour
    {
        public event Action<SlotEvaluateContainer> onSlotEvaluated = delegate { };

        [SerializeField] private List<SlotCellView> _slotCells = new List<SlotCellView>();
        [SerializeField] private Button _spinStopButton;
        [SerializeField] private TextMeshProUGUI _spinButtonText;
        [SerializeField] private Button _pullButton;
        [SerializeField] private TextMeshProUGUI _pullButtonText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _rerollText;

        [Header("Game")] [SerializeField] private int _totalSlotCount = 5;
        [SerializeField] private int _existRerollCount = 5;
        private const int MaxRerollCount = 5;

        private bool _isSpinning = false;
        private CancellationTokenSource _spinCts;
        private CancellationTokenSource[] _slotSpinCts;
        private int _spinningActiveSlots = 0;
        private System.Random _rng = new System.Random();
        private SlotSymbol[] _currentSymbols;

        // 게임 시작 초기화 - 슬라이더, 텍스트 등
        public void Init()
        {
            if (_currentSymbols == null || _currentSymbols.Length != _totalSlotCount)
            {
                _currentSymbols = new SlotSymbol[_totalSlotCount];
            }

            if ( /* damage computed above */ true)
            {
            }

            _existRerollCount = MaxRerollCount;
            UpdateGaugeUI();
            _statusText.text = "대기";
            RegisterUIEvents();
            _spinButtonText.text = "SPIN";
            // Pull 버튼이 존재하면: 당기기/정지 분리 구성
            _pullButtonText.text = "PULL";
            _spinStopButton.interactable = false; // 시작 시 Stop 비활성화
            _pullButton.interactable = true;
            _spinButtonText.text = "STOP"; // Stop 전용 버튼 텍스트
        }


        private void RegisterUIEvents()
        {
            if (_spinStopButton != null)
            {
                _spinStopButton.onClick.RemoveListener(OnPressSpinOrStop);
                _spinStopButton.onClick.AddListener(OnPressSpinOrStop);
            }

            if (_pullButton != null)
            {
                _pullButton.onClick.RemoveListener(OnPressPull);
                _pullButton.onClick.AddListener(OnPressPull);
            }
        }

        private bool TryStopNextSlot()
        {
            if (_slotCells == null || _slotCells.Count == 0) return false;

            for (int i = 0; i < _slotCells.Count; i++)
            {
                if (!IsActiveSlotIndex(i)) continue;

                if (_slotSpinCts == null || i >= _slotSpinCts.Length) continue;

                var slotToken = _slotSpinCts[i];
                if (slotToken == null || slotToken.IsCancellationRequested) continue;

                slotToken.Cancel();
                slotToken.Dispose();
                _slotSpinCts[i] = null;
                _spinningActiveSlots = Mathf.Max(0, _spinningActiveSlots - 1);

                if (_spinningActiveSlots == 0)
                {
                    _isSpinning = false;
                }

                return true;
            }

            return false;
        }

        private bool IsActiveSlotIndex(int index)
        {
            if (_slotCells == null || _slotCells.Count == 0) return false;
            return index > 0 && index < (_slotCells.Count - 1);
        }

        private void InitSlotSpinCts()
        {
            if (_slotCells == null) return;

            if (_slotSpinCts == null)
            {
                _slotSpinCts = new CancellationTokenSource[_slotCells.Count];
                return;
            }

            if (_slotSpinCts.Length != _slotCells.Count)
            {
                ClearAllSlotSpinTokens();
                _slotSpinCts = new CancellationTokenSource[_slotCells.Count];
            }
        }

        private void CancelSlotSpinToken(int index)
        {
            if (_slotSpinCts == null || index < 0 || index >= _slotSpinCts.Length) return;

            var slotToken = _slotSpinCts[index];
            if (slotToken == null) return;

            if (!slotToken.IsCancellationRequested)
            {
                slotToken.Cancel();
            }

            slotToken.Dispose();
            _slotSpinCts[index] = null;
        }

        private void ClearAllSlotSpinTokens()
        {
            if (_slotSpinCts == null) return;

            for (int i = 0; i < _slotSpinCts.Length; i++)
            {
                CancelSlotSpinToken(i);
            }
        }

        // SPIN/STOP 버튼: Pull 버튼이 있을 땐 STOP(효과 적용)만 수행
        private async void OnPressSpinOrStop()
        {
            if (_isSpinning)
            {
                bool stopped = TryStopNextSlot();
                if (!stopped)
                {
                    await ApplyStopEffectsAsync();
                }

                bool hasStoppableSlot = _spinningActiveSlots > 0;
                if (!hasStoppableSlot)
                {
                    _pullButton.interactable = true;
                }

                return;
            }

            await ApplyStopEffectsAsync();
        }

        private void OnPressPull()
        {
            if (_existRerollCount <= 0)
            {
                if (_statusText != null) _statusText.text = "리롤 부족";
                return;
            }

            if (_isSpinning) return;
            StartSpin().Forget();
        }

        // 리롤 소모 -> 스핀 시작
        private async UniTask StartSpin()
        {
            if (_existRerollCount <= 0)
                return;

            _existRerollCount--;
            _isSpinning = true;
            _spinningActiveSlots = 0;
            _spinButtonText.text = "STOP";
            _pullButton.interactable = false;
            _spinStopButton.interactable = true;

            _statusText.text = "SPINNING (" + _existRerollCount + "/" + MaxRerollCount + ")";
            UpdateGaugeUI();

            if (_spinCts != null)
            {
                _spinCts.Cancel();
                _spinCts.Dispose();
            }

            _spinCts = new CancellationTokenSource();

            InitSlotSpinCts();
            ClearAllSlotSpinTokens();

            List<UniTask> tasks = new List<UniTask>();
            for (int i = 0; i < _slotCells.Count; i++)
            {
                SlotCellView cell = _slotCells[i];
                bool active = IsActiveSlotIndex(i);
                cell.SetActive(active);
                if (active)
                {
                    var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(_spinCts.Token);
                    _slotSpinCts[i] = linkedToken;
                    tasks.Add(SpinCellAsync(cell, linkedToken.Token));
                    _spinningActiveSlots++;
                }
            }

            await UniTask.WhenAll(tasks);
        }


        private async UniTask SpinCellAsync(SlotCellView cell, CancellationToken token)
        {
            try
            {
                cell.isSpinning = true;
                while (!token.IsCancellationRequested)
                {
                    SlotSymbol rand = SlotSymbolHelper.NextRandom(_rng);
                    await cell.SpinStepAsync(rand, 80f, 0.12f, token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                cell.isSpinning = false;
            }
        }

        private void CacheCurrentSymbols()
        {
            for (int i = 0; i < _slotCells.Count; i++)
            {
                bool active = i != 0 && i != (_slotCells.Count - 1);
                if (!active) continue;
                _currentSymbols[i] = _slotCells[i].CurrentSymbol;
            }
        }

        // 효과 합산 및 적용
        private void EvaluateSlotResults()
        {
            int shieldCount = 0, swordCount = 0, magicCount = 0;

            for (int i = 0; i < _slotCells.Count; i++)
            {
                bool active = i != 0 && i != (_slotCells.Count - 1);
                if (!active) continue;
                SlotSymbol s = _currentSymbols[i];
                if (s == SlotSymbol.Shield) shieldCount++;
                if (s == SlotSymbol.Sword) swordCount++;
                if (s == SlotSymbol.Magic) magicCount++;
            }

            if (shieldCount == 3 || swordCount == 3 || magicCount == 3)
            {
                _existRerollCount += 1;
                if (_statusText != null) _statusText.text = "??? +1";
            }

            SlotEvaluateContainer resultContainer = new SlotEvaluateContainer(swordCount, shieldCount, magicCount);
            onSlotEvaluated.Invoke(resultContainer);
        }

        private void UpdateGaugeUI()
        {
            _rerollText.text = "Reroll: " + _existRerollCount.ToString();
        }

        // STOP 시 효과 적용 전체 플로우
        private async UniTask ApplyStopEffectsAsync()
        {
            _spinButtonText.text = "SPIN";
            _statusText.text = "정지";
            CacheCurrentSymbols();
            EvaluateSlotResults();
            UpdateGaugeUI();
            _existRerollCount = MaxRerollCount;

            // STOP 후 UI 상태 복귀
            _spinStopButton.interactable = false;
            _spinButtonText.text = "STOP";
        }
    }
}