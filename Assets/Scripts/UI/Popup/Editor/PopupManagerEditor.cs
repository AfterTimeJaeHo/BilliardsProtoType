using UnityEditor;
using UnityEngine;

namespace Aftertime.SecretSome.UI.Popup
{
    [CustomEditor(typeof(PopupManager))]
    public class PopupManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty _uiList;

        private void OnEnable()
        {
            _uiList = serializedObject.FindProperty("_popups");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add All Popup"))
            {

                _uiList.ClearArray();
                
                PopupBase[] popupArray = FindObjectsByType<PopupBase>(FindObjectsSortMode.None);
                foreach (PopupBase popup in popupArray)
                {
                    _uiList.arraySize++;
                    _uiList.GetArrayElementAtIndex(_uiList.arraySize - 1).boxedValue = popup;    
                }
                
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
