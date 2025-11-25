using Aftertime.SecretSome;
using TMPro;
using UnityEngine;

namespace Waving.UI
{
    public class TurnChangePopupView : View
    {
        public TextMeshProUGUI TitleText => _titleText;
        public CanvasGroup CanvasGroup => _canvasGroup;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private CanvasGroup _canvasGroup;
    }
   
}