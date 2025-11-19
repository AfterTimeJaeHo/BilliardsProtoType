using UnityEditor;
using UnityEditor.UI;

namespace Aftertime.SecretSome.UI.Editor
{
    [CustomEditor(typeof(DropdownToggle))]
    public class DropdownToggleEditor : ToggleEditor
    {
        private SerializedProperty _text;
        private SerializedProperty _image;
        private SerializedProperty _offImage;
        private SerializedProperty _onTextColor;
        private SerializedProperty _onImageColor;
        private SerializedProperty _offTextColor;
        private SerializedProperty _offImageColor;
        private SerializedProperty _fadeDuration;

        protected override void OnEnable()
        {
            base.OnEnable();
            _text = serializedObject.FindProperty("_text");
            _image = serializedObject.FindProperty("_image");
            _offImage = serializedObject.FindProperty("_offImage");
            _onTextColor = serializedObject.FindProperty("_onTextColor");
            _onImageColor = serializedObject.FindProperty("_onImageColor");
            _offTextColor = serializedObject.FindProperty("_offTextColor");
            _offImageColor = serializedObject.FindProperty("_offImageColor");
            _fadeDuration = serializedObject.FindProperty("_fadeDuration");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_text);
            EditorGUILayout.PropertyField(_image);
            EditorGUILayout.PropertyField(_offImage);
            EditorGUILayout.PropertyField(_onTextColor);
            EditorGUILayout.PropertyField(_onImageColor);
            EditorGUILayout.PropertyField(_offTextColor);
            EditorGUILayout.PropertyField(_offImageColor);
            EditorGUILayout.PropertyField(_fadeDuration);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
   
}
