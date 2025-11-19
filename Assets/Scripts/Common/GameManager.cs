using System;
using UnityEngine;

namespace Aftertime.SecretSome.Common
{
    public class GameManager : MonoBehaviour
    {
        public static event Action onApplicationPause = delegate { };
        public static event Action onApplicationQuit = delegate { };
        
        [SerializeField] private GameObject border;

        private void Awake()
        {
            if (border != null)
                border.SetActive(true);

#if UNITY_ANDROID
            Application.targetFrameRate = 60;

#endif
        }


        public static float GameSpeed
        {
            get => Time.timeScale;
            set => Time.timeScale = value;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            onApplicationPause();
        }

        private void OnApplicationQuit()
        {
            onApplicationQuit();
        }
    }
}
