using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Aftertime.MyTinyStreamer.Combat
{
    // 적 데미지 텍스트와 흔들림 연출을 담당
    public class EnemyReact : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _shakeDuration = 0.3f;
        [SerializeField] private float _shakeAmplitude = 0.25f;
        [SerializeField] private float _shakeFrequency = 45f;
        [SerializeField] private Color _damageTextColor = Color.red;

        private CancellationTokenSource _cts;

        // 초기화: 토큰 준비 및 대상 기본값 설정
        protected void Awake()
        {
            _cts = new CancellationTokenSource();
            if (_target == null)
                _target = transform;
        }

        // 파괴 시: 비동기 작업 취소
        protected void OnDestroy()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        // 적이 좌우로 빠르게 흔들리는 연출
        public async UniTask ShakeAsync(float duration, float amplitude, float frequency, CancellationToken cancellationToken)
        {
            Transform t = _target != null ? _target : transform;
            Vector3 origin = t.localPosition;
            float elapsed = 0f;
            try
            {
                while (elapsed < duration)
                {
                    float pct = elapsed / duration;
                    float damper = 1f - pct;
                    float offsetX = Mathf.Sin(elapsed * frequency * 2f * Mathf.PI) * amplitude * damper;
                    t.localPosition = new Vector3(origin.x + offsetX, origin.y, origin.z);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    elapsed += Time.deltaTime;
                }
            }
            catch (OperationCanceledException)
            {
            }
            t.localPosition = origin;
        }

        // Enemy 위로 데미지 텍스트를 페이드인/아웃으로 표시
        public UniTask ShowDamageTextAsync(int amount, Camera camera, CancellationToken cancellationToken)
        {
            Transform t = _target != null ? _target : transform;
            return DamageTextController.ShowAsync(amount, t, camera, _damageTextColor, cancellationToken);
        }
    }
}

