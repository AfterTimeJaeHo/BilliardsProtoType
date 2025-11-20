using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Aftertime.MyTinyStreamer.Combat
{
    // 피해 텍스트를 생성·재생하는 유틸리티
    public static class DamageTextController
    {
        // 화면 오버레이 캔버스 찾거나 생성
        private static Canvas GetOrCreateOverlayCanvas()
        {
            Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return canvas;
            }

            Canvas[] cards = UnityEngine.Object.FindObjectsOfType<Canvas>();
            if (cards != null)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    Canvas c = cards[i];
                    if (c != null && c.renderMode == RenderMode.ScreenSpaceOverlay)
                        return c;
                }
            }

            GameObject go = new GameObject("DamageOverlayCanvas");
            Canvas newCanvas = go.AddComponent<Canvas>();
            newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            return newCanvas;
        }

        // Enemy 위 위치에 화면 좌표 기반 텍스트를 잠시 띄우고 페이드 처리
        public static async UniTask ShowAsync(int amount, Transform worldAnchor, Camera camera, Color color, CancellationToken cancellationToken)
        {
            Canvas canvas = GetOrCreateOverlayCanvas();
            RectTransform canvasRect = canvas.transform as RectTransform;

            Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            GameObject go = new GameObject("DamageText");
            go.transform.SetParent(canvas.transform, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            Text text = go.AddComponent<Text>();
            CanvasGroup group = go.AddComponent<CanvasGroup>();
            Outline outline = go.AddComponent<Outline>();

            text.font = font;
            text.text = "-" + amount.ToString();
            Color textColor = color;
            textColor.a = 1f;
            text.color = textColor;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;

            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            group.alpha = 0f;

            Vector3 worldPos = worldAnchor.position + new Vector3(0f, 1.2f, 0f);
            Camera cam = camera != null ? camera : Camera.main;
            if (cam == null)
            {
                Camera[] camsRaw = UnityEngine.Object.FindObjectsOfType<Camera>();
                if (camsRaw != null && camsRaw.Length > 0)
                {
                    cam = camsRaw[0];
                }
            }
            Vector3 screenPos = cam != null ? cam.WorldToScreenPoint(worldPos) : new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

            Vector2 anchoredFromCanvas;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out anchoredFromCanvas))
            {
                rect.anchoredPosition = anchoredFromCanvas;
            }

            float moveUp = 28f;
            float fadeIn = 0.12f;
            float hold = 0.18f;
            float fadeOut = 0.22f;

            float elapsed = 0f;
            try
            {
                // Fade In + slight move up
                while (elapsed < fadeIn)
                {
                    float t = elapsed / fadeIn;
                    group.alpha = t;
                    rect.anchoredPosition = anchoredFromCanvas + new Vector2(0f, t * moveUp * 0.4f);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    elapsed += Time.deltaTime;
                }
                group.alpha = 1f;

                // Hold
                elapsed = 0f;
                while (elapsed < hold)
                {
                    rect.anchoredPosition = anchoredFromCanvas + new Vector2(0f, moveUp * 0.4f + elapsed / hold * moveUp * 0.3f);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    elapsed += Time.deltaTime;
                }

                // Fade Out + move up
                elapsed = 0f;
                while (elapsed < fadeOut)
                {
                    float t2 = elapsed / fadeOut;
                    group.alpha = 1f - t2;
                    rect.anchoredPosition = anchoredFromCanvas + new Vector2(0f, moveUp * 0.7f + t2 * moveUp * 0.3f);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    elapsed += Time.deltaTime;
                }
            }
            catch (OperationCanceledException)
            {
            }

            UnityEngine.Object.Destroy(go);
        }
    }
}

