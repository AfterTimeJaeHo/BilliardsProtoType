using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aftertime.MyTinyStreamer.Combat;

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

        [Header("Fonts")] [SerializeField] private TMP_FontAsset _nanumSquareFont;

        [Header("Slot Symbols")] [SerializeField]
        private Sprite _shieldSprite;

        [SerializeField] private Sprite _swordSprite;
        [SerializeField] private Sprite _magicSprite;

        [Header("Stats")] [SerializeField] private int _playerMaxHP = 125;
        [SerializeField] private int _enemyMaxHP = 125;
        [SerializeField] private int _playerCurrentHP = 125;
        [SerializeField] private int _enemyCurrentHP = 125;
        [SerializeField] private int _playerShield = 0;
        [SerializeField] private int _coins = 0;

        [Header("Effects")] [SerializeField] private int _shieldPerSymbol = 5;
        [SerializeField] private int _swordDamagePerSymbol = 10;
        [SerializeField] private int _magicDamagePerSymbol = 8;

        [Header("Game")] [SerializeField] private int _activeSlotCount = 3;
        [SerializeField] private int _totalSlotCount = 5;
        [SerializeField] private int _rerollReady = 5; // 기본 리롤 횟수
        private int _rerollMax = 5;

        private bool _isSpinning = false;
        private CancellationTokenSource _spinCts;
        private bool _pendingStopped = false;

        [Header("Auto Stop")] [SerializeField] private float _autoStopDelay = 1.8f; // Pull 이후 자동 정지 대기

        [Header("Combat FX - Timings")] [SerializeField] private float _enemyShakeDuration = 0.3f;
        [SerializeField] private float _enemyShakeAmplitude = 0.25f;
        [SerializeField] private float _enemyShakeFrequency = 45f;
        [SerializeField] private float _enemyCounterDelay = 0.5f;
        [SerializeField] private float _cameraShakeDuration = 0.2f;
        [SerializeField] private float _cameraShakeAmplitude = 0.12f;
        [SerializeField] private float _turnBackDelay = 0.4f;
        [SerializeField] private Color _damageTextColor = Color.red;
        [SerializeField] private string _playerTurnLabel = "플레이어 턴";

        [Header("Combat FX - Refs")] [SerializeField] private EnemyReact _enemyReact;
        [SerializeField] private CameraShaker _cameraShaker;
        [SerializeField] private Transform _enemyAnchor;

        // PlayerTurn 배너 내부 캐시
        private RectTransform _turnBannerRoot;
        private CanvasGroup _turnBannerGroup;
        private TMPro.TextMeshProUGUI _turnBannerLabel;

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
            
            if ( _enemyCurrentHP <= 0 )
            {
                _enemyCurrentHP = 0;
                _statusText.text = "승리!";
            }
            
            if ( /* damage computed above */ true ) { }
            _rerollMax = _rerollReady;
            UpdateGaugeUI();
            _statusText.text = "대기";
            _spinStopButton.onClick.AddListener(OnPressSpinOrStop);
            _spinButtonText.text = "SPIN";
            // Pull 버튼이 존재하면: 당기기/정지 분리 구성
            if (_pullButton != null)
            {
                _pullButton.onClick.AddListener(OnPressPull);
                if (_pullButtonText != null) _pullButtonText.text = "PULL";

                if (_spinStopButton != null)
                {

                    _spinStopButton.interactable = false; // 시작 시 Stop 비활성화
                }

                if (_pullButton != null) _pullButton.interactable = true;
                if (_spinButtonText != null) _spinButtonText.text = "STOP"; // Stop 전용 버튼 텍스트
            }

            if (_enemyImage != null && _enemyNormalSprite != null)
            {
                _enemyImage.sprite = _enemyNormalSprite;
            }
            EnsureFxBindings();
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
        // SPIN/STOP 버튼: Pull 버튼이 있을 땐 STOP(효과 적용)만 수행
        public async void OnPressSpinOrStop()
        {
            if (_pullButton != null)
            {
                if (_isSpinning)
                {
                    StopReelsOnly();
                    _pendingStopped = true;
                }
                if (_pendingStopped)
                {
                    await ApplyStopEffectsAsync();
                    _pendingStopped = false;
                }
                return;
            }

            if (_isSpinning)
                StopSpinning();
            else
                StartSpinned().Forget();
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
            AutoStopAfterDelayAsync(_autoStopDelay).Forget();
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
                if (_pendingStopped)
                {
                    ApplyStopEffectsAsync().Forget();
                    _pendingStopped = false;
                }
                return;
            }

            StopReelsOnly();
            _pendingStopped = true;
            ApplyStopEffectsAsync().Forget();
            _pendingStopped = false;
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
                if (_statusText != null) _statusText.text = "콤보 +1";
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

            if (damage > 0)
            {
                PlayCombatEffectsAsync(damage).Forget();
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
        // 스핀만 중지(효과 적용 없음)
        private void StopReelsOnly()
        {
            _isSpinning = false;
            if (_spinCts != null) _spinCts.Cancel();
        }

        // Pull 후 일정 시간 경과 시 자동 정지(효과 적용 없음)
        private async UniTask AutoStopAfterDelayAsync(float delay)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            catch (OperationCanceledException)
            {
                return;
            }
            if (_isSpinning)
            {
                StopReelsOnly();
                _pendingStopped = true;
                // 자동 정지 후 Pull 버튼 활성화, Stop 버튼도 유지
                if (_pullButton != null) _pullButton.interactable = true;
                if (_spinStopButton != null) _spinStopButton.interactable = true;
                if (_spinButtonText != null) _spinButtonText.text = "STOP";
            }
        }

        // STOP 시 효과 적용 전체 플로우
        private async UniTask ApplyStopEffectsAsync()
        {
            _spinButtonText.text = "SPIN";
            _statusText.text = "정지";
            CacheCurrentSymbols();
            EvaluateResults();
            UpdateGaugeUI();

            // STOP 후 UI 상태 복귀
            if (_pullButton != null)
            {
                if (_spinStopButton != null) _spinStopButton.interactable = false;
                if (_pullButton != null) _pullButton.interactable = true;
                if (_spinButtonText != null) _spinButtonText.text = "STOP";
            }
        }

        // Enemy 흔들림+데미지 텍스트 → 잠시 후 카메라 셰이크 → 플레이어 턴 배너 → 리롤 초기화
        private async UniTask PlayCombatEffectsAsync(int damage)
        {
            CancellationToken token = this.GetCancellationTokenOnDestroy();
            Camera cam = Camera.main != null ? Camera.main : (GameObject.FindObjectOfType<Camera>());

            if (_enemyReact != null)
            {
                UniTask shakeTask = _enemyReact.ShakeAsync(_enemyShakeDuration, _enemyShakeAmplitude, _enemyShakeFrequency, token);
                Transform anchor = _enemyAnchor != null ? _enemyAnchor : _enemyReact.transform;
                UniTask dmgTask = DamageTextController.ShowAsync(damage, anchor, cam, _nanumSquareFont, _damageTextColor, token);
                await UniTask.WhenAll(shakeTask, dmgTask);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_enemyCounterDelay), cancellationToken: token);

            if (_cameraShaker != null)
            {
                await _cameraShaker.ShakeAsync(_cameraShakeDuration, _cameraShakeAmplitude, token);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_turnBackDelay), cancellationToken: token);

            await ShowPlayerTurnBannerAsync(_playerTurnLabel, token);
            _rerollReady = _rerollMax;
            UpdateGaugeUI();
        }

        private void EnsureTurnBanner()
        {
            if (_turnBannerRoot != null) return;
            GameObject canvasGo = GameObject.Find("Canvas");
            if (canvasGo == null) return;

            GameObject root = new GameObject("PlayerTurnBanner");
            root.transform.SetParent(canvasGo.transform, false);
            _turnBannerRoot = root.AddComponent<RectTransform>();
            _turnBannerRoot.sizeDelta = new Vector2(600f, 160f);
            _turnBannerRoot.anchorMin = new Vector2(0.5f, 0.5f);
            _turnBannerRoot.anchorMax = new Vector2(0.5f, 0.5f);
            _turnBannerRoot.anchoredPosition = Vector2.zero;

            Image bg = root.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.65f);

            _turnBannerGroup = root.AddComponent<CanvasGroup>();
            _turnBannerGroup.alpha = 0f;

            GameObject labelGo = new GameObject("Label");
            labelGo.transform.SetParent(root.transform, false);
            RectTransform labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            _turnBannerLabel = labelGo.AddComponent<TextMeshProUGUI>();
            if (_nanumSquareFont != null) _turnBannerLabel.font = _nanumSquareFont;
            _turnBannerLabel.text = _playerTurnLabel;
            _turnBannerLabel.alignment = TextAlignmentOptions.Center;
            _turnBannerLabel.color = Color.white;
            _turnBannerLabel.fontSize = 52f;
        }

        private async UniTask ShowPlayerTurnBannerAsync(string label, CancellationToken token)
        {
            EnsureTurnBanner();
            if (_turnBannerLabel != null) _turnBannerLabel.text = label;

            float fadeIn = 0.15f;
            float show = 0.8f;
            float fadeOut = 0.2f;

            float t = 0f;
            while (t < fadeIn)
            {
                _turnBannerGroup.alpha = t / fadeIn;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                t += Time.deltaTime;
            }
            _turnBannerGroup.alpha = 1f;

            t = 0f;
            while (t < show)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                t += Time.deltaTime;
            }

            t = 0f;
            while (t < fadeOut)
            {
                _turnBannerGroup.alpha = 1f - (t / fadeOut);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                t += Time.deltaTime;
            }
            _turnBannerGroup.alpha = 0f;
        }

        private void EnsureFxBindings()
        {
            if (_enemyAnchor == null)
            {
                GameObject enemyGo = GameObject.Find("EnemyAnchor");
                if (enemyGo == null) enemyGo = GameObject.Find("Enemy");
                if (enemyGo != null) _enemyAnchor = enemyGo.transform;
            }
            if (_enemyReact == null && _enemyAnchor != null)
            {
                _enemyReact = _enemyAnchor.GetComponent<EnemyReact>();
                if (_enemyReact == null) _enemyReact = _enemyAnchor.gameObject.AddComponent<EnemyReact>();
            }
            if (_cameraShaker == null)
            {
                Camera cam = Camera.main;
                if (cam == null)
                {
                    Camera[] cams = GameObject.FindObjectsOfType<Camera>();
                    if (cams != null && cams.Length > 0) cam = cams[0];
                }
                if (cam != null)
                {
                    _cameraShaker = cam.GetComponent<CameraShaker>();
                    if (_cameraShaker == null) _cameraShaker = cam.gameObject.AddComponent<CameraShaker>();
                }
            }
        }
    }
}
