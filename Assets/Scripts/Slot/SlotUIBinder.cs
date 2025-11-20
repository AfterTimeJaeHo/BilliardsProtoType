using UnityEngine;
using UnityEngine.UI;

namespace Aftertime.MyTinyStreamer.Slot
{
    // 씬 내 Pull/Stop 버튼을 찾아 SlotController에 바인딩
    public class SlotUIBinder : MonoBehaviour
    {
        [SerializeField] private SlotController _slotController;
        [SerializeField] private string _pullButtonName = "PullButton";
        [SerializeField] private string _stopButtonName = "StopButton";

        // 간단한 바인딩 수행
        protected void Awake()
        {
            if (_slotController == null)
            {
                _slotController = GetComponent<SlotController>();
            }

            Button pull = FindButtonByName(_pullButtonName);
            Button stop = FindButtonByName(_stopButtonName);

            if (pull != null)
            {
                pull.onClick.RemoveListener(OnPullClicked);
                pull.onClick.AddListener(OnPullClicked);
            }

            if (stop != null)
            {
                stop.onClick.RemoveListener(OnStopClicked);
                stop.onClick.AddListener(OnStopClicked);
            }

            // SpinStopButton 등은 더 이상 토글로 처리하지 않음 (요구사항 준수)
        }

        private Button FindButtonByName(string name)
        {
            GameObject go = GameObject.Find(name);
            if (go == null)
                return null;
            return go.GetComponent<Button>();
        }

        private void OnPullClicked()
        {
            if (_slotController != null)
                _slotController.OnPull();
        }

        private void OnStopClicked()
        {
            if (_slotController != null)
                _slotController.OnStopButton();
        }

    }
}
