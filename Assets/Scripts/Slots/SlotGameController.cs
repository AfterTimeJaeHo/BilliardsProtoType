using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Aftertime.MyTinyStreamer.Slots
{
    // 슬롯 전투의 핵심 컨트롤러. UI와 전투 규칙을 관리
    public class SlotGameController : MonoBehaviour
    {
        [SerializeField] private List<SlotCellView> _slotCells = new List<SlotCellView>();
        [SerializeField] private Button _spinStopButton;
        [SerializeField] private TextMeshProUGUI _spinButtonText;
        [SerializeField] private Button _pullButton;
        [SerializeField] private TextMeshProUGUI _pullButtonText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private Slider _playerHPSlider;
        [SerializeField] private TextMeshProUGUI _playerHPText;
        [SerializeField] private TextMeshProUGUI _playerShieldText;
        [SerializeField] private Slider _enemyHPSlider;
        [SerializeField] private TextMeshProUGUI _enemyHPText;
        [SerializeField] private TextMeshProUGUI _rerollText;
        [SerializeField] private Slider _rerollGaugeSlider;
        [SerializeField] private TextMeshProUGUI _coinText;
        [SerializeField] private Image _enemyImage;
        [SerializeField] private Sprite _enemyNormalSprite;
        [SerializeField] private Sprite _enemyBrokenSprite;
        [SerializeField] private float _breakThresholdPercent = 0.5f; // 체력 50% 이하에서 파손 연출

        [Header("Fonts")]
        [SerializeField] private TMP_FontAsset _nanumSquareFont;
        [Header("Slot Symbols")]
        [SerializeField] private Sprite _shieldSprite;
        [SerializeField] private Sprite _swordSprite;
        [SerializeField] private Sprite _magicSprite;

        [Header("Stats")]
        [SerializeField] private int _playerMaxHP = 125;
        [SerializeField] private int _enemyMaxHP = 125;
        [SerializeField] private int _playerCurrentHP = 125;
        [SerializeField] private int _enemyCurrentHP = 125;
        [SerializeField] private int _playerShield = 0;
        [SerializeField] private int _coins = 0;

        [Header("Effects")]
        [SerializeField] private int _shieldPerSymbol = 5;
        [SerializeField] private int _swordDamagePerSymbol = 10;
        [SerializeField] private int _magicDamagePerSymbol = 8;

        [Header("Game")]
        [SerializeField] private int _activeSlotCount = 3;
        [SerializeField] private int _totalSlotCount = 5;
        [SerializeField] private int _rerollReady = 5; // 기본 리롤 횟수
        private int _rerollMax = 5;

        private bool _isSpinning = false;
        private CancellationTokenSource _spinCts;

        private System.Random _rng = new System.Random();
        private SlotSymbol[] _currentSymbols;

        // 씬 오브젝트에서 UI를 자동 바인딩하고, 시작 시 초기화합니다.
        private void Awake()
        {
            TryBindFromScene();
        }

        // 게임 시작 시 초기화 진입
        private void Start()
        {
            InitGame();
        }

        // 게임 시작 초기화 - 슬라이더, 텍스트 등
        public void InitGame()
        {
            if (_currentSymbols == null || _currentSymbols.Length != _totalSlotCount)
            {
                _currentSymbols = new SlotSymbol[_totalSlotCount];
            }
            UpdateSlotActives();
            UpdatePlayerUI();
            UpdateEnemyUI();
            _rerollMax = _rerollReady;
            UpdateGaugeUI();
            _statusText.text = "대기";
            if (_spinStopButton != null)
            {
                _spinStopButton.onClick.RemoveAllListeners();
                _spinStopButton.onClick.AddListener(OnPressSpinOrStop);
            }
            _spinButtonText.text = "SPIN";
            // Pull 버튼이 존재하면: 당기기/정지 분리 구성
            if (_pullButton != null)
            {
                _pullButton.onClick.RemoveAllListeners();
                _pullButton.onClick.AddListener(OnPressPull);
                if (_pullButtonText != null) _pullButtonText.text = "PULL";

                if (_spinStopButton != null)
                {
                    _spinStopButton.onClick.RemoveAllListeners();
                    _spinStopButton.onClick.AddListener(StopSpinning);
                    _spinStopButton.interactable = false; // 시작 시 Stop 비활성화
                }
                if (_pullButton != null) _pullButton.interactable = true;
                if (_spinButtonText != null) _spinButtonText.text = "STOP"; // Stop 전용 버튼 텍스트
            }
            if (_enemyImage != null && _enemyNormalSprite != null)
            {
                _enemyImage.sprite = _enemyNormalSprite;
            }
        }

        // 씬에 배치된 오브젝트로부터 필요한 컴포넌트를 찾아 바인딩합니다.
        private void TryBindFromScene()
        {
            GameObject canvasGo = GameObject.Find("Canvas");
            if (canvasGo == null)
            {
                return;
            }
            if (_nanumSquareFont == null)
            {
                _nanumSquareFont = Resources.Load<TMP_FontAsset>("Fonts/NanumSquareR SDF");
            }
            if (_nanumSquareFont != null)
            {
                TextMeshProUGUI[] tmps = canvasGo.GetComponentsInChildren<TextMeshProUGUI>(true);
                for (int i = 0; i < tmps.Length; i++)
                {
                    tmps[i].font = _nanumSquareFont;
                }
            }

            if (_spinStopButton == null)
            {
                GameObject btnGo = GameObject.Find("SpinStopButton");
                if (btnGo != null)
                {
                    _spinStopButton = btnGo.GetComponent<Button>();
                }
            }
            if (_pullButton == null)
            {
                GameObject pullGo = GameObject.Find("PullButton");
                if (pullGo != null)
                {
                    _pullButton = pullGo.GetComponent<Button>();
                }
            }
            if (_spinButtonText == null)
            {
                GameObject labelGo = GameObject.Find("Label");
                if (labelGo != null)
                {
                    _spinButtonText = labelGo.GetComponent<TextMeshProUGUI>();
                }
            }
            if (_pullButtonText == null)
            {
                GameObject pullLabelGo = GameObject.Find("PullLabel");
                if (pullLabelGo != null)
                {
                    _pullButtonText = pullLabelGo.GetComponent<TextMeshProUGUI>();
                }
            }
            if (_statusText == null)
            {
                GameObject go = GameObject.Find("StatusText");
                if (go != null)
                {
                    _statusText = go.GetComponent<TextMeshProUGUI>();
                }
            }
            if (_playerHPSlider == null)
            {
                GameObject go = GameObject.Find("PlayerHP");
                if (go != null)
                {
                    _playerHPSlider = go.GetComponent<Slider>();
                }
            }
            if (_playerHPText == null)
            {
                GameObject go = GameObject.Find("PlayerHPText");
                if (go != null)
                {
                    _playerHPText = go.GetComponent<TextMeshProUGUI>();
                }
            }
            if (_playerShieldText == null)
            {
                GameObject go = GameObject.Find("PlayerShieldText");
                if (go != null)
                {
                    _playerShieldText = go.GetComponent<TextMeshProUGUI>();
                }
            }
            if (_enemyHPSlider == null)
            {
                GameObject go = GameObject.Find("EnemyHP");
                if (go != null)
                {
                    _enemyHPSlider = go.GetComponent<Slider>();
                }
            }
            if (_enemyHPText == null)
            {
                GameObject go = GameObject.Find("EnemyHPText");
                if (go != null)
                {
                    _enemyHPText = go.GetComponent<TextMeshProUGUI>();
                }
            }
            if (_rerollText == null)
            {
                GameObject go = GameObject.Find("RerollText");
                if (go != null)
                {
                    _rerollText = go.GetComponent<TextMeshProUGUI>();
                }
            }
            if (_rerollGaugeSlider == null)
            {
                GameObject go = GameObject.Find("RerollGauge");
                if (go != null)
                {
                    _rerollGaugeSlider = go.GetComponent<Slider>();
                }
            }
            if (_coinText == null)
            {
                GameObject go = GameObject.Find("CoinText");
                if (go != null)
                {
                    _coinText = go.GetComponent<TextMeshProUGUI>();
                }
            }
            if (_enemyImage == null)
            {
                GameObject go = GameObject.Find("EnemyImage");
                if (go != null)
                {
                    _enemyImage = go.GetComponent<Image>();
                }
            }
            if (_slotCells == null || _slotCells.Count == 0)
            {
                _slotCells = new List<SlotCellView>();
                GameObject row = GameObject.Find("SlotRow");
                if (row != null)
                {
                    SlotCellView[] found = row.GetComponentsInChildren<SlotCellView>(true);
                    for (int i = 0; i < found.Length; i++)
                    {
                        _slotCells.Add(found[i]);
                    }
                }
            }

            // 심볼 스프라이트 자동 로드(Resources) 및 각 셀에 설정
            if ((_shieldSprite == null) || (_swordSprite == null) || (_magicSprite == null))
            {
                Sprite[] loaded = Resources.LoadAll<Sprite>("Symbols/EmojiOne");
                if (loaded != null && loaded.Length >= 3)
                {
                    _shieldSprite = loaded[0];
                    _swordSprite = loaded[1];
                    _magicSprite = loaded[2];
                }
            }
            if (_slotCells != null && _slotCells.Count > 0)
            {
                for (int i = 0; i < _slotCells.Count; i++)
                {
                    if (_slotCells[i] != null)
                        _slotCells[i].ConfigureSprites(_shieldSprite, _swordSprite, _magicSprite);
                }
            }
        }

        // SPIN/STOP 토글 버튼 이벤트
        public void OnPressSpinOrStop()
        {
            if (_isSpinning)
            {
                StopSpinning();
            }
            else
            {
                StartSpinned().Forget();
            }
        }

        // 슬롯 당기기 전용 버튼 (슬롯 UI 옆)
        public void OnPressPull()
        {
            if (_rerollReady <= 0)
            {
                if (_statusText != null) _statusText.text = "리롤 부족";
                return;
            }
            if (_isSpinning) return;
            StartSpinned().Forget();
        }

        // 리롤 소모 -> 스핀 시작
        private async UniTask StartSpinned()
        {
            if (_rerollReady <= 0)
            {
                _statusText.text = "리롤 없음";
                return;
            }

            _rerollReady--;
            _isSpinning = true;
            _spinButtonText.text = "STOP";
            // Pull 버튼이 존재하면 Stop만 가능하도록 토글
            if (_pullButton != null)
            {
                if (_pullButton != null) _pullButton.interactable = false;
                if (_spinStopButton != null) _spinStopButton.interactable = true;
            }
            _statusText.text = "SPINNING (" + _rerollReady + "/" + _rerollMax + ")";
            UpdateGaugeUI();
            _spinCts = new CancellationTokenSource();

            List<UniTask> tasks = new List<UniTask>();
            for (int I = 0; I < _slotCells.Count; I++)
            {
                SlotCellView cell = _slotCells[I];
                bool active = I != 0 && I != (_slotCells.Count - 1);
                cell.SetActive(active);
                if (active)
                {
                    tasks.Add(SpinCellAsync(cell, _spinCts.Token));
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

        // 스핀 중지 -> 결과 계산
        public void StopSpinning()
        {
            if (!_isSpinning)
            {
                return;
            }
            _isSpinning = false;
            if (_spinCts != null)
            {
                _spinCts.Cancel();
            }
            _spinButtonText.text = "SPIN";
            _statusText.text = "해결";
            CacheCurrentSymbols();
            EvaluateResults();
            UpdateGaugeUI();
            // STOP 이후 UI 상태 정리 (분리 모드일 경우 Pull/Stop 상호배타)
            if (_pullButton != null)
            {
                if (_spinStopButton != null) _spinStopButton.interactable = false;
                if (_pullButton != null) _pullButton.interactable = true;
                if (_spinButtonText != null) _spinButtonText.text = "STOP";
            }
            else
            {
                if (_spinButtonText != null) _spinButtonText.text = "SPIN";
            }
            if (_statusText != null) _statusText.text = "정지";
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
        private void EvaluateResults()
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

            int shieldAdd = shieldCount * _shieldPerSymbol;
            int damage = swordCount * _swordDamagePerSymbol + magicCount * _magicDamagePerSymbol;

            if (shieldCount == 3 || swordCount == 3 || magicCount == 3)
            {
                _rerollReady += 1;
                _rerollMax += 1;
                if (_statusText != null) _statusText.text = "리롤 +1";
            }

            _playerShield += shieldAdd;
            DealDamageToEnemy(damage);
            UpdatePlayerUI();
            UpdateEnemyUI();

            if (_enemyCurrentHP <= 0)
            {
                _enemyCurrentHP = 0;
                _statusText.text = "승리!";
            }
        }

        private void DealDamageToEnemy(int damage)
        {
            if (damage <= 0) return;
            _enemyCurrentHP -= damage;
            if (_enemyCurrentHP < 0) _enemyCurrentHP = 0;

            float thresholdHP = Mathf.Round(_enemyMaxHP * _breakThresholdPercent);
            if (_enemyImage != null && _enemyCurrentHP <= (int)thresholdHP)
            {
                if (_enemyBrokenSprite != null) _enemyImage.sprite = _enemyBrokenSprite;
                else _enemyImage.color = new Color(1f, 0.6f, 0.6f, 1f);
            }
        }

        private void UpdatePlayerUI()
        {
            _playerHPSlider.maxValue = _playerMaxHP;
            _playerHPSlider.value = _playerCurrentHP;
            _playerHPText.text = "Player: " + _playerCurrentHP + "/" + _playerMaxHP;
            _playerShieldText.text = "실드: " + _playerShield;
        }

        private void UpdateEnemyUI()
        {
            _enemyHPSlider.maxValue = _enemyMaxHP;
            _enemyHPSlider.value = _enemyCurrentHP;
            _enemyHPText.text = "Enemy: " + _enemyCurrentHP + "/" + _enemyMaxHP;
        }

        private void UpdateGaugeUI()
        {
            _rerollGaugeSlider.maxValue = _rerollMax;
            _rerollGaugeSlider.value = _rerollReady;
            _rerollText.text = "Reroll: " + _rerollReady.ToString();
        }

        // 외부에서 슬롯 목록 주입
        public void SetSlotCells(List<SlotCellView> cells)
        {
            _slotCells = cells;
            UpdateSlotActives();
        }

        private void UpdateSlotActives()
        {
            for (int i = 0; i < _slotCells.Count; i++)
            {
                bool active = i != 0 && i != (_slotCells.Count - 1);
                _slotCells[i].SetActive(active);
            }
        }

        // 라벨 의존 제거됨
    }
}

