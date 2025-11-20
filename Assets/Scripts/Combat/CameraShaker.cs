using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Aftertime.MyTinyStreamer.Combat
{
    // 카메라를 짧게 흔드는 연출
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _defaultDuration = 0.2f;
        [SerializeField] private float _defaultAmplitude = 0.12f;

        private CancellationTokenSource _cts;

        // 초기화: 대상 기본값 설정
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

        // 지정 값으로 카메라 흔들기
        public async UniTask ShakeAsync(float duration, float amplitude, CancellationToken cancellationToken)
        {
            Transform t = _target != null ? _target : transform;
            Vector3 origin = t.localPosition;
            float elapsed = 0f;
            try
            {
                while (elapsed < duration)
                {
                    float damper = 1f - (elapsed / duration);
                    float offsetX = (UnityEngine.Random.value * 2f - 1f) * amplitude * damper;
                    float offsetY = (UnityEngine.Random.value * 2f - 1f) * amplitude * 0.5f * damper;
                    t.localPosition = new Vector3(origin.x + offsetX, origin.y + offsetY, origin.z);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    elapsed += Time.deltaTime;
                }
            }
            catch (OperationCanceledException)
            {
            }
            t.localPosition = origin;
        }
    }
}

