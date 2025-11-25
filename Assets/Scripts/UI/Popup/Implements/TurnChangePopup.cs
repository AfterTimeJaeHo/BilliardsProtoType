using Aftertime.SecretSome.UI.Popup;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Waving.UI
{
    public class TurnChangePopup : PopupBase
    {
        private TurnChangePopupView _turnChangePopupView;
        private const float Delay = 0.5f;
        private const float Interval = 0.2f;
        
        protected override void Awake()
        {
            base.Awake();
            _turnChangePopupView = _view as TurnChangePopupView;
        }

        public async UniTask UpdatePlayerTurnView()
        {
            TextMeshProUGUI titleText = _turnChangePopupView.TitleText;
            titleText.text = "플레이어 턴";
            await PlayCommonDirecting();
        }

        public async UniTask UpdateEnemyTurnView()
        {
            TextMeshProUGUI titleText = _turnChangePopupView.TitleText;
            titleText.text = "적 턴";
            await PlayCommonDirecting();
        }

        private async UniTask PlayCommonDirecting()
        {
            CanvasGroup canvasGroup = _turnChangePopupView.CanvasGroup;
            canvasGroup.DOKill();
            canvasGroup.alpha = 0;
            
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(Delay);
            sequence.Append(canvasGroup.DOFade(1, fadeDuration));
            sequence.AppendInterval(Interval);
            sequence.Append(canvasGroup.DOFade(0, fadeDuration));
            sequence.onComplete += () => Hide();

            await UniTask.WaitUntil(() => _view.IsVisible == false);
        }
    }
   
}