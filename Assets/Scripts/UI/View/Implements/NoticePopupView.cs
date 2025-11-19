using Aftertime.SecretSome;
using Aftertime.SecretSome.UI;
using TMPro;
using UnityEngine;

namespace Waving.MyTinyStreamer.UI
{
    public class NoticePopupView : View
    {
        public TextMeshProUGUI MessageText => _messageText;
        public ExtendedButton ConfirmButton => _confirmButton;
        public ExtendedButton CancelButton => _cancelButton;
        
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private ExtendedButton _confirmButton;
        [SerializeField] private ExtendedButton _cancelButton;
    }
   
}