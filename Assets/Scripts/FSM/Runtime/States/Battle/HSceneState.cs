using Aftertime.SecretSome.Content;
using Aftertime.SecretSome.UI.Popup;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SRPG;
using UnityEngine;
using UnityEngine.UI;
using Waving.Content;
using Waving.Di;
using Waving.Scene;
using Waving.UI;

namespace Waving.Battle
{
    public class HSceneState : DIClass, IState
    {
        public OnEnter onEnter { get; set; }
        public OnExecute onExecute { get; set; }
        public OnExit onExit { get; set; }

        private const float CanvasGroupDuration = 0.5f;
        private const float HSceneImageDuration = 0.5f;
        private const float Power = 0.9f;
        private const float ScanDelay = 2.7f;
        private const float HSceneParentScanDuration = 5f;
        private const float ShotDuration = 1f;
        private static Vector2 HSceneImageScale = new Vector2(1.5f, 1.5f);
        private static Vector2 HSceneImageStartPos = new Vector2(0, 150);
        private static Vector2 HSceneParentScanPos = new Vector2(0, -430);

        [Inject] private HSceneContainer _container;

        private Sequence _pistonSequence;

        public async void Enter()
        {
            await PlayDirecting();
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }

        private async UniTask PlayDirecting()
        {
            await OffBattleView();
            await PlayPiston();
            await PlayShot();
            ContentRunner.StopContent<BattleContent>();
        }

        private async UniTask OffBattleView()
        {
            CanvasGroup stageCanvasGroup = _container.StageCanvasGroup;
            CanvasGroup uiCanvasCanvasGroup = _container.UICanvasGroup;
            stageCanvasGroup.DOFade(0, CanvasGroupDuration);
            stageCanvasGroup.blocksRaycasts = false;
            stageCanvasGroup.interactable = false;
            uiCanvasCanvasGroup.DOFade(0, CanvasGroupDuration);
            uiCanvasCanvasGroup.blocksRaycasts = false;
            uiCanvasCanvasGroup.interactable = false;

            await UniTask.WaitForSeconds(CanvasGroupDuration);
        }

        private async UniTask PlayPiston()
        {
            Image hSceneImage = _container.HSceneImage;
            Image shotImage = _container.ShotImage;
            RectTransform rt = hSceneImage.rectTransform;

            // 초기화
            rt.localScale = HSceneImageScale; // Vector3(1.5f, 1.5f, 1)
            rt.anchoredPosition = HSceneImageStartPos; // y = 270 등
            hSceneImage.color = new Color(1, 1, 1, 0);
            shotImage.color = new Color(1, 1, 1, 0);
            hSceneImage.DOFade(1, HSceneImageDuration);

            // ──────────────────────────────
            // 스케일 1.5배 기준 최적화된 수치 (2025년 최고 퀄)
            // ──────────────────────────────
            float dur = 0.36f / Power; // 기본 속도
            float baseAmp = 68f; // ← 45f × 1.5배 + 미세 조정으로 68~70이 제일 자연스러움
            float amp = baseAmp * Power; // 깊이 들어가는 양
            float overshoot = amp + 17f * Power; // 튕기는 오버슈트 (11f × 1.5 + α)
            float shallow = 14f * Power; // 뺐을 때 최종 위치 (8f × 1.5 + α)

            _pistonSequence = DOTween.Sequence();

            // 1. 빠르게 찌르기
            _pistonSequence.Append(rt.DOLocalMoveY(rt.anchoredPosition.y + amp, dur * 0.38f)
                .SetEase(Ease.OutExpo));

            // 2. 살짝 더 들어갔다 튕기는 결정적 한 방
            _pistonSequence.Append(rt.DOLocalMoveY(rt.anchoredPosition.y + overshoot, dur * 0.09f)
                .SetEase(Ease.OutBounce));

            // 3. 천천히 빼기 (제일 길게!)
            _pistonSequence.Append(rt.DOLocalMoveY(rt.anchoredPosition.y + shallow, dur * 0.53f)
                .SetEase(Ease.InCubic));

            // 4. Squash & Stretch (스케일 1.5배라서 더 강하게 줘야 보인다!)
            _pistonSequence.Join(rt.DOScaleY(1.5f + 0.075f * Power, dur * 0.38f)); // 찌를 때 살짝 늘어남
            _pistonSequence.Join(rt.DOScaleY(1.5f * 0.955f, dur * 0.09f)); // 충격 순간 눌림
            _pistonSequence.Join(rt.DOScaleY(1.5f, dur * 0.53f).SetEase(Ease.OutElastic));

            // 5. 미세 회전 (1.5배일 때도 이 정도가 딱!)
            _pistonSequence.Join(rt.DOLocalRotate(new Vector3(0, 0, -2.1f * Power), dur * 0.38f));
            _pistonSequence.Join(rt.DOLocalRotate(Vector3.zero, dur * 0.62f).SetEase(Ease.OutCubic));

            _pistonSequence.SetLoops(-1, LoopType.Restart);
            _pistonSequence.Play();

            await UniTask.WaitForSeconds(ScanDelay); 
            
            RectTransform imageParent = _container.HSceneImage.transform.parent.GetComponent<RectTransform>();
            await imageParent.DOAnchorPos(HSceneParentScanPos, HSceneParentScanDuration).ToUniTask();
            
            _pistonSequence.Kill();
        }

        private async UniTask PlayShot()
        {
            PopupManager.Instance.Push<FadePopup>();
            float SceneFadeDuration = PopupManager.Instance.GetPopup<FadePopup>().FadeDuration;
            
            RectTransform hSceneImageRt = _container.HSceneImage.rectTransform;
            hSceneImageRt.DOScale(1.7f, SceneFadeDuration);
            
            await UniTask.WaitForSeconds(SceneFadeDuration);
        }
    }
}