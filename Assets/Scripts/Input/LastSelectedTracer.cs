using System;
using Aftertime.SecretSome.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aftertime.TeachingAssistantLife
{
    public class LastSelectedTracer : MonoBehaviour
    {
        public static GameObject LastSelected { get; private set; }

        private static GameObject curSelected;
        
        private void Start()
        {
            curSelected = EventSystem.current.currentSelectedGameObject;
            UpdateExecutor.onUpdate += UpdateLastSelected;
        }

        private void UpdateLastSelected()
        {
            GameObject newSelected = EventSystem.current.currentSelectedGameObject;
            if (curSelected != null && curSelected != newSelected)
                LastSelected = curSelected;

            curSelected = newSelected;
        }

        public static void SetLastSelectedGameObject()
        {
            EventSystem.current.SetSelectedGameObject(LastSelected);
        }
    }
}
