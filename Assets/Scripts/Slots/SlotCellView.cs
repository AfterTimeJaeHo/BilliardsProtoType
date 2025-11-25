using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Aftertime.MyTinyStreamer.Slots
{
    // 슬롯 셀의 뷰. 텍스트/이미지 활성 상태를 관리
    public class SlotCellView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _symbolText; // 디버그 표시용(미사용 가능)
        [SerializeField] private Image _symbolImage; // 실제 심볼 표시 이미지
        [Header("Symbol Sprites")]
        [SerializeField] private Sprite _shieldSprite;
        [SerializeField] private Sprite _swordSprite;
        [SerializeField] private Sprite _magicSprite;
        [SerializeField] private bool _isActive = true;

        public bool isSpinning = false;
        private SlotSymbol _currentSymbol = SlotSymbol.None;
        public SlotSymbol CurrentSymbol { get { return _currentSymbol; } }

        // 자동 바인딩: 빠진 참조를 자식/자기에서 보완
        private void Awake()
        {
            if (_background == null)
            {
                _background = GetComponent<Image>();
            }
            if (_symbolImage == null)
            {
                Transform icon = transform.Find("Icon");
                if (icon != null)
                {
                    _symbolImage = icon.GetComponent<Image>();
                }
                if (_symbolImage == null)
                {
                    Image[] images = GetComponentsInChildren<Image>(true);
                    for (int i = 0; i < images.Length; i++)
                    {
                        if (images[i] != _background)
                        {
                            _symbolImage = images[i];
                            break;
                        }
                    }
                }
            }
            if (_symbolText == null)
            {
                _symbolText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        // 외부 호출 시 초기화
        public void Init(Image bgImage, TextMeshProUGUI text)
        {
            _background = bgImage;
            _symbolText = text;
        }

        // 심볼 변경: 텍스트는 보조, 실제 표시는 스프라이트로 변경
        public void SetSymbol(SlotSymbol symbol)
        {
            _currentSymbol = symbol;
            if (_symbolText != null)
            {
                _symbolText.text = SlotSymbolHelper.GetDisplayText(symbol);
            }
            if (_symbolImage != null)
            {
                Sprite s = null;
                if (symbol == SlotSymbol.Shield) s = _shieldSprite;
                else if (symbol == SlotSymbol.Sword) s = _swordSprite;
                else if (symbol == SlotSymbol.Magic) s = _magicSprite;
                _symbolImage.sprite = s;
                _symbolImage.preserveAspect = true;
            }
        }

        // 활성/비활성 시 시각 효과
        public void SetActive(bool active)
        {
            _isActive = active;
            if (_background != null)
            {
                _background.color = active ? Color.white : new Color(0.36f, 0.36f, 0.36f, 1.0f);
            }
            if (_symbolImage != null)
            {
                Color col = _symbolImage.color;
                col.a = active ? 1f : 0.5f;
                _symbolImage.color = col;
            }
        }
        

        // 슬롯머신 회전 연출(상하 이동 후 심볼 교체)
        public async UniTask SpinStepAsync(SlotSymbol nextSymbol, float travel, float duration, CancellationToken token)
        {
            if (_symbolImage == null)
            {
                SetSymbol(nextSymbol);
                await UniTask.Delay(System.TimeSpan.FromSeconds(duration), cancellationToken: token);
                return;
            }

            RectTransform rt = _symbolImage.rectTransform;
            float half = duration * 0.5f;
            try
            {
                Tween down = rt.DOAnchorPosY(-travel, half).SetEase(Ease.InQuad);
                await down.AsyncWaitForCompletion();
                SetSymbol(nextSymbol);
                Tween up = rt.DOAnchorPosY(0f, half).SetEase(Ease.OutQuad);
                await up.AsyncWaitForCompletion();
            }
            catch (System.Exception)
            {
            }
            finally
            {
                if (rt != null) rt.anchoredPosition = Vector2.zero;
            }
        }
    }
}
