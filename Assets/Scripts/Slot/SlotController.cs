using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using Aftertime.MyTinyStreamer.Combat;

namespace Aftertime.MyTinyStreamer.Slot
{
    // 슬롯 Pull/자동정지/STOP 트리거를 관리
    public class SlotController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TurnDirector _turnDirector;

        [Header("Slot Timing")]
        [SerializeField] private float _autoStopDelay = 1.8f; // Pull 이후 자동 정지 대기

        [Header("Symbol Result (External set or test)")]
        [SerializeField] private CombatSymbol _lastSymbol = CombatSymbol.Sword; // 외부 슬롯 시스템이 UpdateResult로 갱신
        [SerializeField] private int _lastDamage = 10;

        [Header("Events to integrate existing reel controller")]
        [SerializeField] private UnityEvent _onRequestStartSpin; // 외부 슬롯 시스템에 StartSpin 요청
        [SerializeField] private UnityEvent _onRequestStopSpin;  // 외부 슬롯 시스템에 StopSpin 요청

        private CancellationTokenSource _cts;
        private bool _isSpinning;
        private bool _hasStopped;

        // 초기화
        protected void Awake()
        {
            _cts = new CancellationTokenSource();
            if (_turnDirector == null)
            {
                GameObject go = GameObject.Find("CombatSystem");
                if (go != null)
                {
                    _turnDirector = go.GetComponent<TurnDirector>();
                }
            }
        }

        protected void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        // Pull 버튼에서 호출
        public void OnPull()
        {
            if (_isSpinning)
                return;

            _isSpinning = true;
            _hasStopped = false;
            if (_onRequestStartSpin != null)
                _onRequestStartSpin.Invoke();

            AutoStopAfterDelayAsync(_autoStopDelay, _cts.Token).Forget();
        }

        // STOP 버튼에서 호출: 실제 효과는 STOP 시점에만 발동
        public void OnStopButton()
        {
            if (_isSpinning == false && _hasStopped == false)
            {
                return;
            }

            if (_hasStopped == false)
            {
                if (_onRequestStopSpin != null)
                    _onRequestStopSpin.Invoke();
                _hasStopped = true;
                _isSpinning = false;
            }

            if (_turnDirector != null && _lastSymbol != CombatSymbol.None)
            {
                _turnDirector.OnSlotStoppedWithSymbol(_lastSymbol, _lastDamage);
            }
        }

        // 단일 스핀/스톱 버튼 토글 대응
        public void OnSpinStopToggle()
        {
            if (_isSpinning == false && _hasStopped == false)
            {
                OnPull();
                return;
            }
            OnStopButton();
        }

        // 외부 슬롯 시스템이 실제 결과를 알려줄 때 호출
        public void UpdateResult(CombatSymbol symbol, int damage)
        {
            _lastSymbol = symbol;
            _lastDamage = damage;
        }

        // 자동 정지 딜레이 처리(문양 효과는 여기서 발동하지 않음)
        private async UniTask AutoStopAfterDelayAsync(float delay, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (_isSpinning)
            {
                if (_onRequestStopSpin != null)
                    _onRequestStopSpin.Invoke();
                _hasStopped = true;
                _isSpinning = false;
            }
        }
    }
}

