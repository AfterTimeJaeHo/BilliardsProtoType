using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;

namespace Waving.Battle
{
    public class AssaultSlider : MonoBehaviour, IPointerDownHandler
    {
        [Header("References")] public Slider slider;
        public Image fillImage;
        public RectTransform marker; // 성공 구간 마커
        public RectTransform handle; // 핸들(없으면 slider.handleRect)
        public RectTransform trackArea; // 판정 기준 Rect(없으면 slider의 Rect)
        public TextMeshProUGUI _resultText;
        public CanvasGroup _canvasGroup;

        [Header("Settings")] public float moveDuration = 1f; // 왕복 한 번당 시간/2 (DOValue가 두 번이라 전체 왕복은 2*duration)

        private Sequence sliderSeq;
        private bool isActive = false;
        private bool hasInput = false;
        private bool _success = false;

        public async UniTask<bool> StartAssaultQTE()
        {
            _resultText.text = null;
            _canvasGroup.DOFade(1, 0.2f);
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            isActive = true;
            hasInput = false;
            _success = false;

            if (slider != null)
            {
                sliderSeq = DOTween.Sequence();
                sliderSeq.Append(slider.DOValue(1f, moveDuration).SetEase(Ease.InOutSine));
                sliderSeq.Append(slider.DOValue(0f, moveDuration).SetEase(Ease.InOutSine));
                sliderSeq.SetLoops(-1, LoopType.Yoyo);
            }

            await UniTask.WaitUntil(() => !isActive);
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            await _canvasGroup.DOFade(0, 0.2f).ToUniTask();
            return _success;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isActive || hasInput) return;

            hasInput = true;
            isActive = false;
            sliderSeq?.Kill();

            CheckResult().Forget();
        }

        private async UniTask<bool> CheckResult()
        {
            var canvas = GetComponentInParent<Canvas>();
            var area = trackArea != null
                ? trackArea
                : (slider != null ? slider.transform as RectTransform : transform as RectTransform);
            var handleRect = handle != null ? handle : (slider != null ? slider.handleRect : null);
            if (area == null || handleRect == null || marker == null)
            {
                Debug.LogWarning("AssaultSlider: Required references are not set.");
                return false;
            }

            Vector2 ToAreaLocal(Vector3 world)
            {
                var cam = canvas != null ? canvas.worldCamera : null;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    area,
                    RectTransformUtility.WorldToScreenPoint(cam, world),
                    cam,
                    out var local
                );
                return local;
            }

            Rect LocalRect(RectTransform rt)
            {
                var corners = new Vector3[4];
                rt.GetWorldCorners(corners);
                var l0 = ToAreaLocal(corners[0]);
                var l1 = ToAreaLocal(corners[1]);
                var l2 = ToAreaLocal(corners[2]);
                var l3 = ToAreaLocal(corners[3]);
                float minX = Mathf.Min(l0.x, Mathf.Min(l1.x, Mathf.Min(l2.x, l3.x)));
                float maxX = Mathf.Max(l0.x, Mathf.Max(l1.x, Mathf.Max(l2.x, l3.x)));
                float minY = Mathf.Min(l0.y, Mathf.Min(l1.y, Mathf.Min(l2.y, l3.y)));
                float maxY = Mathf.Max(l0.y, Mathf.Max(l1.y, Mathf.Max(l2.y, l3.y)));
                return Rect.MinMaxRect(minX, minY, maxX, maxY);
            }

            // Overlap 판정: 핸들 사각형과 마커 사각형이 겹치면 성공
            var handleLocalRect = LocalRect(handleRect);
            var markerLocalRect = LocalRect(marker);
            _success = handleLocalRect.Overlaps(markerLocalRect, true);

            _resultText.text = _success ? "기습 성공" : "기습 실패";
            sliderSeq?.Kill();
            return _success;
        }
    }
}