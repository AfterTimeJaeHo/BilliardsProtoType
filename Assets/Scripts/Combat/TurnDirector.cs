using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Aftertime.MyTinyStreamer.Combat
{
    // 슬롯 정지 결과에 따라 데미지/흔들림/반격/턴전환을 오케스트레이션
    public class TurnDirector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EnemyReact _enemy;
        [SerializeField] private GameObject _enemyObject;
        [SerializeField] private CameraShaker _cameraShaker;
        [SerializeField] private PlayerTurnUI _playerTurnUI;

        [Header("Timings")]
        [SerializeField] private float _enemyShakeDuration = 0.3f;
        [SerializeField] private float _enemyShakeAmplitude = 0.25f;
        [SerializeField] private float _enemyShakeFrequency = 45f;
        [SerializeField] private float _enemyCounterDelay = 0.5f;
        [SerializeField] private float _cameraShakeDuration = 0.2f;
        [SerializeField] private float _cameraShakeAmplitude = 0.12f;
        [SerializeField] private float _turnBackDelay = 0.4f;

        [Header("Events")]
        [SerializeField] private UnityEvent _onResetReroll;

        private CancellationTokenSource _cts;

        // 초기화: 토큰 준비 및 카메라 셰이커 자동 할당
        protected void Awake()
        {
            _cts = new CancellationTokenSource();
            if (_enemy == null && _enemyObject != null)
            {
                _enemy = _enemyObject.GetComponent<EnemyReact>();
            }
            if (_enemy == null)
            {
                GameObject enemyGo = GameObject.Find("Enemy");
                if (enemyGo == null)
                    enemyGo = GameObject.Find("EnemyAnchor");
                if (enemyGo != null)
                    _enemy = enemyGo.GetComponent<EnemyReact>();
            }

            if (_cameraShaker == null)
            {
                Camera cam = GetMainCamera();
                if (cam != null)
                {
                    _cameraShaker = cam.GetComponent<CameraShaker>();
                    if (_cameraShaker == null)
                    {
                        _cameraShaker = cam.gameObject.AddComponent<CameraShaker>();
                    }
                }
            }
        }

        // 외부에서 적 객체를 갱신해 연결
        public void UpdateEnemyObject(GameObject enemyObject)
        {
            _enemyObject = enemyObject;
            if (_enemyObject != null)
            {
                _enemy = _enemyObject.GetComponent<EnemyReact>();
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

        // 슬롯이 멈췄을 때 호출 (지팡이/검 심볼인 경우 공격 처리)
        public void OnSlotStoppedWithSymbol(CombatSymbol symbol, int damage)
        {
            if (symbol == CombatSymbol.Staff || symbol == CombatSymbol.Sword)
            {
                CancellationToken token = _cts.Token;
                HandlePlayerAttackAsync(damage, token).Forget();
            }
        }

        // 플레이어 공격 → 적 흔들림+데미지 텍스트 → 반격(카메라 셰이크) → 플레이어 턴 UI → 리롤 리셋
        private async UniTask HandlePlayerAttackAsync(int damage, CancellationToken token)
        {
            Camera cam = GetMainCamera();

            if (_enemy != null)
            {
                UniTask shakeTask = _enemy.ShakeAsync(_enemyShakeDuration, _enemyShakeAmplitude, _enemyShakeFrequency, token);
                UniTask damageTask = _enemy.ShowDamageTextAsync(damage, cam, token);
                await UniTask.WhenAll(shakeTask, damageTask);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_enemyCounterDelay), cancellationToken: token);

            if (_cameraShaker != null)
            {
                await _cameraShaker.ShakeAsync(_cameraShakeDuration, _cameraShakeAmplitude, token);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_turnBackDelay), cancellationToken: token);

            if (_playerTurnUI == null)
            {
                _playerTurnUI = gameObject.GetComponent<PlayerTurnUI>();
                if (_playerTurnUI == null)
                {
                    _playerTurnUI = gameObject.AddComponent<PlayerTurnUI>();
                }
            }

            await _playerTurnUI.ShowAsync("플레이어 턴", token);

            if (_onResetReroll != null)
            {
                _onResetReroll.Invoke();
            }
        }

        // 메인 카메라를 찾는다
        private Camera GetMainCamera()
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                Camera[] cams = GameObject.FindObjectsOfType<Camera>();
                if (cams != null && cams.Length > 0)
                {
                    cam = cams[0];
                }
            }
            return cam;
        }
    }
}
