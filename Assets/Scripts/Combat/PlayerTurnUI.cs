using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Aftertime.MyTinyStreamer.Combat
{
    // 플레이어 턴 배너 UI 표시/숨김 관리
    public class PlayerTurnUI : MonoBehaviour
    {
        [SerializeField] private string _label = "플레이어 턴";
        [SerializeField] private Color _bgColor = new Color(0f, 0f, 0f, 0.65f);
        [SerializeField] private Color _textColor = Color.white;
        [SerializeField] private Vector2 _size = new Vector2(600f, 160f);
        [SerializeField] private float _fadeIn = 0.15f;
        [SerializeField] private float _showTime = 0.8f;
        [SerializeField] private float _fadeOut = 0.2f;

        private RectTransform _root;
        private CanvasGroup _group;
        private Text _text;
        private CancellationTokenSource _cts;

        // 초기화: 런타임 UI 준비
        protected void Awake()
        {
            _cts = new CancellationTokenSource();
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

        // 플레이어 턴 배너를 생성(한 번만)한다
        private void EnsureCreated()
        {
            if (_root != null)
                return;

            Canvas canvas = GetOverlayCanvas();
            GameObject go = new GameObject("PlayerTurnBanner");
            go.transform.SetParent(canvas.transform, false);

            _root = go.AddComponent<RectTransform>();
            _root.sizeDelta = _size;

            Image bg = go.AddComponent<Image>();
            bg.color = _bgColor;

            _group = go.AddComponent<CanvasGroup>();
            _group.alpha = 0f;

            GameObject textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            RectTransform textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 0f);
            textRect.anchorMax = new Vector2(1f, 1f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Font font = Resources.Load<Font>("NanumSquareR SDF");
            _text = textGo.AddComponent<Text>();
            _text.font = font;
            _text.text = _label;
            _text.color = _textColor;
            _text.alignment = TextAnchor.MiddleCenter;

            _root.anchorMin = new Vector2(0.5f, 0.5f);
            _root.anchorMax = new Vector2(0.5f, 0.5f);
            _root.anchoredPosition = Vector2.zero;
        }

        // 캔버스 획득(스크린 오버레이)
        private Canvas GetOverlayCanvas()
        {
            Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return canvas;

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

            GameObject go = new GameObject("PlayerTurnCanvas");
            Canvas newCanvas = go.AddComponent<Canvas>();
            newCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            return newCanvas;
        }

        // 플레이어 턴 배너 표시
        public async UniTask ShowAsync(string label, CancellationToken cancellationToken)
        {
            EnsureCreated();
            _text.text = label;

            float elapsed = 0f;
            while (elapsed < _fadeIn)
            {
                float t = elapsed / _fadeIn;
                _group.alpha = t;
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsed += Time.deltaTime;
            }
            _group.alpha = 1f;

            elapsed = 0f;
            while (elapsed < _showTime)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsed += Time.deltaTime;
            }

            elapsed = 0f;
            while (elapsed < _fadeOut)
            {
                float t2 = elapsed / _fadeOut;
                _group.alpha = 1f - t2;
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                elapsed += Time.deltaTime;
            }
            _group.alpha = 0f;
        }
    }
}

