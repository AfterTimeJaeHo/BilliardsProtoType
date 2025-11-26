using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SRPG;
using UnityEngine;
using UnityEngine.UI;
using Waving.Common;
using Waving.Di;
using Waving.Directing;

namespace Waving.Battle
{
    public class AssaultState : DIClass, IState
    {
        public event Action<bool> onAssaultComplete = delegate { };
        
        public OnEnter onEnter { get; set; }
        public OnExecute onExecute { get; set; }
        public OnExit onExit { get; set; }

        [Inject] private AssaultContainer _container;
        private Sequence _walkSequence;

        public async void Enter()
        {
            _container.AssaultCanvasGroup.alpha = 1;
            await ShowSpriteGroup();
            StartWalking();
            await PlayRun();
            StopWalking();
            bool isAssaultSuccess = await _container.AssaultSlider.StartAssaultQTE();

            if (isAssaultSuccess)
            {
                await ShowSpanking();
            }
            else
            {
                await ShowFrontStanding();
            }
            
            onAssaultComplete.Invoke(isAssaultSuccess);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }

        private async UniTask ShowSpriteGroup()
        {
            const float FadeDuration = 0.2f;
            SpriteGroup spriteGroup = _container.SpriteGroup;
            await spriteGroup.DOFade(1, FadeDuration);
        }

        private async UniTask PlayRun()
        {
            Transform cam = _container.BattleCam.transform;
            Vector3 destPos = _container.DestinationTransform.position;
            Vector3 originalPos = cam.position;

            // 1. 오버슈트 위치 (1.2~1.4배가 제일 무섭다)
            Vector3 overshoot = Vector3.Lerp(originalPos, destPos, 1.35f); // 1.35배 추천
            overshoot.z = originalPos.z;

            Sequence seq = DOTween.Sequence();

            // 1단계: 초고속 돌진 + FOV 폭발 (숨 막히는 느낌)
            seq.Append(cam.DOMove(overshoot, 0.28f) // 0.28초가 제일 무섭다
                .SetEase(Ease.OutCubic));

            // FOV 줌인 (숨막히는 압박감)
            seq.Join(Camera.main.DOFieldOfView(45f, 0.25f)); // 기본 60 → 45로 급축소

            // 2단계: 급정지 + 살짝 뒤로 튕기기 (현실감 + 충격)
            seq.Append(cam.DOMove(destPos, 0.06f)
                .SetEase(Ease.InQuart));

            // // 3단계: 튕기고 다시 미세하게 앞으로 살짝 (심장 쫄깃)
            // seq.Append(cam.DOMove(destPos + (destPos - originalPos).normalized * 0.15f, 0.08f)
            //     .SetEase(Ease.OutQuad));
            // seq.Append(cam.DOMove(destPos, 0.04f)); // 딱 멈춤

            // 4단계: 충격 연출 3연타 (이게 진짜 살인적임)
            seq.AppendCallback(() =>
            {
                // 1차 강한 충격
                Camera.main.transform.DOShakePosition(0.15f, 0.35f, 30, 90);
                Camera.main.transform.DOShakeRotation(0.15f, 5f, 30, 90);

                // 0.07초 후 2차 잔여 떨림
                DOVirtual.DelayedCall(0.07f, () => { Camera.main.transform.DOShakePosition(0.25f, 0.12f, 15, 90); });
            });

            // 5단계: 화면 암전 or H신 직행 (0.3초 후)
            seq.AppendInterval(0.3f);
            await seq.Play().ToUniTask();
        }

        private void StartWalking()
        {
            StopWalking(); // 기존 Tween Kill

            Transform targetTransform = _container.TargetTransform;
            Vector3 originalLocalPos = targetTransform.localPosition;
            float walkBobAmplitude = 0.05f;  // 위아래 움직임 세기
            float walkBobDuration = 0.5f;    // 위아래 주기 (0.3~0.5f가 자연스러움)
            Vector3 walkScale = new Vector3(0.25f, 0.25f, 0.25f);
            Ease walkEase = Ease.InOutSine;
            _walkSequence = DOTween.Sequence();

            // === 위치 bobbing ===
            _walkSequence.Append(targetTransform.DOLocalMoveY(originalLocalPos.y + walkBobAmplitude, walkBobDuration / 2f));
            _walkSequence.Append(targetTransform.DOLocalMoveY(originalLocalPos.y - walkBobAmplitude * 0.7f, walkBobDuration / 2f));
            _walkSequence.Append(targetTransform.DOScale( walkScale, walkBobDuration / 2f));

            
            _walkSequence.SetLoops(-1, LoopType.Restart);
        }

        private void StopWalking()
        {
            Transform targetTransform = _container.TargetTransform;
            Vector3 originalLocalPos = targetTransform.localPosition;
            if (_walkSequence != null)
            {
                _walkSequence.Kill(); // 즉시 중지 + 원위치 복귀
                targetTransform.localPosition = originalLocalPos;
                _walkSequence = null;
            }
        }

        private async UniTask ShowSpanking()
        {
            await TimelineDirector.Instance.PlaySpanking();
        }

        private async UniTask ShowFrontStanding()
        {
            await TimelineDirector.Instance.PlayAssaultFail();
        }
    }
}