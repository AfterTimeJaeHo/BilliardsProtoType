using Aftertime.SecretSome.UI.Editor;
using UnityEditor;

namespace Aftertime.SecretSome.UI
{
    [CustomEditor(typeof(ToggleSwitch))]
    [CanEditMultipleObjects]
    public class SwitchToggleEditor : ExtendedToggleEditor
    {
        private SerializedProperty _switchImage;
        private SerializedProperty _onColor;
        private SerializedProperty _offColor;
        private SerializedProperty _onPosX;
        private SerializedProperty _offPosX;
        
         protected void OnEnable()
        {
            base.OnEnable();
            _switchImage = serializedObject.FindProperty("_switchImage");
            _onColor = serializedObject.FindProperty("_onColor");
            _offColor = serializedObject.FindProperty("_offColor");
            _onPosX = serializedObject.FindProperty("_onPosX");
            _offPosX = serializedObject.FindProperty("_offPosX");
        }
         
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(_switchImage);
            EditorGUILayout.PropertyField(_onColor);
            EditorGUILayout.PropertyField(_offColor);
            EditorGUILayout.PropertyField(_onPosX);
            EditorGUILayout.PropertyField(_offPosX);

            serializedObject.ApplyModifiedProperties();
        }
    }
   
}
