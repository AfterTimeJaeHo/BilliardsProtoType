using UnityEditor;
using UnityEditor.UI;

namespace Aftertime.SecretSome.UI.Editor
{
    [CustomEditor(typeof(ExtendedToggle))]
    [CanEditMultipleObjects]
    public class ExtendedToggleEditor : SelectableEditor
    {
        private SerializedProperty _textMeshPro;

        // Option
        private SerializedProperty _useObject;
        private SerializedProperty _useSpriteColor;
        private SerializedProperty _useText;
        private SerializedProperty _colorDuration;

        // Default
        private SerializedProperty _defaultObject;
        private SerializedProperty _defaultSprite;
        private SerializedProperty _defaultSpriteColor;
        private SerializedProperty _defaultTextColor;

        // Enter
        private SerializedProperty _pointerEnterObject;
        private SerializedProperty _pointerEnterSprite;
        private SerializedProperty _pointerEnterSpriteColor;
        private SerializedProperty _pointerEnterTextColor;

        // On
        protected SerializedProperty _onObject;
        protected SerializedProperty _onSprite;
        protected SerializedProperty _onSpriteColor;
        protected SerializedProperty _onTextColor;
        
        // Off
        protected SerializedProperty _offObject;
        protected SerializedProperty _offSprite;
        protected SerializedProperty _offSpriteColor;
        protected SerializedProperty _offTextColor;
        
        // Press
        private SerializedProperty _pressedObject;
        private SerializedProperty _pressedSprite;
        private SerializedProperty _pressedSpriteColor;
        private SerializedProperty _pressedTextColor;
        
        // Focus
        private SerializedProperty _focusBGObject;
        
        // Sound
        private SerializedProperty _onClip;
        private SerializedProperty _offClip;

        protected void OnEnable()
        {
            base.OnEnable();
            _useObject = serializedObject.FindProperty("_useObject");
            _useSpriteColor = serializedObject.FindProperty("_useSpriteColor");
            _useText = serializedObject.FindProperty("_useText");
            _textMeshPro = serializedObject.FindProperty("_textMeshPro");
            _colorDuration = serializedObject.FindProperty("_colorDuration");

            _defaultObject = serializedObject.FindProperty("_defaultObject");
            _defaultSprite = serializedObject.FindProperty("_defaultSprite");
            _defaultSpriteColor = serializedObject.FindProperty("_defaultSpriteColor");
            _defaultTextColor = serializedObject.FindProperty("_defaultTextColor");

            _pointerEnterObject = serializedObject.FindProperty("_pointerEnterObject");
            _pointerEnterSprite = serializedObject.FindProperty("_pointerEnterSprite");
            _pointerEnterSpriteColor = serializedObject.FindProperty("_pointerEnterSpriteColor");
            _pointerEnterTextColor = serializedObject.FindProperty("_pointerEnterTextColor");

            _onObject = serializedObject.FindProperty("_onObject");
            _onSprite = serializedObject.FindProperty("_onSprite");
            _onSpriteColor = serializedObject.FindProperty("_onSpriteColor");
            _onTextColor = serializedObject.FindProperty("_onTextColor");
            
            _offObject = serializedObject.FindProperty("_offObject");
            _offSprite = serializedObject.FindProperty("_offSprite");
            _offSpriteColor = serializedObject.FindProperty("_offSpriteColor");
            _offTextColor = serializedObject.FindProperty("_offTextColor");

            _pressedObject = serializedObject.FindProperty("_pressedObject");
            _pressedSprite = serializedObject.FindProperty("_pressedSprite");
            _pressedSpriteColor = serializedObject.FindProperty("_pressedSpriteColor");
            _pressedTextColor = serializedObject.FindProperty("_pressedTextColor");
            
            _focusBGObject = serializedObject.FindProperty("_focusBGObject");
            
            _onClip = serializedObject.FindProperty("_onClip");
            _offClip = serializedObject.FindProperty("_offClip");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(_useObject);
            EditorGUILayout.PropertyField(_useText);
            EditorGUILayout.PropertyField(_useSpriteColor);

            ObjectSession();
            SpriteSession();
            SpriteColorSession();
            
            EditorGUILayout.PropertyField(_focusBGObject);
            
            EditorGUILayout.PropertyField(_onClip);
            EditorGUILayout.PropertyField(_offClip);

            serializedObject.ApplyModifiedProperties();
        }

        private void ObjectSession()
        {
            if (_useObject.boolValue == false)
                return;

            TextSession();
            NonTextSession();

            void TextSession()
            {
                if (_useText.boolValue == false)
                    return;

                EditorGUILayout.PropertyField(_textMeshPro);

                EditorGUILayout.PropertyField(_defaultObject);
                EditorGUILayout.PropertyField(_defaultTextColor);
                EditorGUILayout.PropertyField(_pointerEnterObject);
                EditorGUILayout.PropertyField(_pointerEnterTextColor);

                EditorGUILayout.PropertyField(_onObject);
                EditorGUILayout.PropertyField(_onTextColor);
                
                EditorGUILayout.PropertyField(_offObject);
                EditorGUILayout.PropertyField(_offTextColor);

                EditorGUILayout.PropertyField(_pressedObject);
                EditorGUILayout.PropertyField(_pressedTextColor);
            }

            void NonTextSession()
            {
                if (_useText.boolValue == true)
                    return;

                EditorGUILayout.PropertyField(_defaultObject);
                EditorGUILayout.PropertyField(_pointerEnterObject);

                EditorGUILayout.PropertyField(_onObject);
                EditorGUILayout.PropertyField(_offObject);

                EditorGUILayout.PropertyField(_pressedObject);
            }
        }

        private void SpriteSession()
        {
            if (_useObject.boolValue == true)
                return;
            
            if (_useSpriteColor.boolValue == true)
                return;


            TextSession();
            NonTextSession();
            
            void TextSession()
            {
                if (_useText.boolValue == false)
                    return;

                EditorGUILayout.PropertyField(_textMeshPro);

                EditorGUILayout.PropertyField(_defaultSprite);
                EditorGUILayout.PropertyField(_defaultTextColor);
                EditorGUILayout.PropertyField(_pointerEnterSprite);
                EditorGUILayout.PropertyField(_pointerEnterTextColor);

                EditorGUILayout.PropertyField(_onSprite);
                EditorGUILayout.PropertyField(_onTextColor);
                EditorGUILayout.PropertyField(_offSprite);
                EditorGUILayout.PropertyField(_offTextColor);

                EditorGUILayout.PropertyField(_pressedSprite);
                EditorGUILayout.PropertyField(_pressedTextColor);
            }

            void NonTextSession()
            {
                if (_useText.boolValue == true)
                    return;

                EditorGUILayout.PropertyField(_defaultSprite);
                EditorGUILayout.PropertyField(_pointerEnterSprite);

                EditorGUILayout.PropertyField(_onSprite);
                EditorGUILayout.PropertyField(_offSprite);

                EditorGUILayout.PropertyField(_pressedSprite);
            }
        }

        private void SpriteColorSession()
        {
            if (_useObject.boolValue == true)
                return;
            
            if (_useSpriteColor.boolValue == false)
                return;
            
            EditorGUILayout.PropertyField(_colorDuration);

            TextSession();
            NonTextSession();

            void TextSession()
            {
                if (_useText.boolValue == false)
                    return;

                EditorGUILayout.PropertyField(_textMeshPro);

                EditorGUILayout.PropertyField(_defaultSpriteColor);
                EditorGUILayout.PropertyField(_defaultTextColor);
                EditorGUILayout.PropertyField(_pointerEnterSpriteColor);
                EditorGUILayout.PropertyField(_pointerEnterTextColor);

                EditorGUILayout.PropertyField(_onSpriteColor);
                EditorGUILayout.PropertyField(_onTextColor);
                EditorGUILayout.PropertyField(_offSpriteColor);
                EditorGUILayout.PropertyField(_offTextColor);

                EditorGUILayout.PropertyField(_pressedSpriteColor);
                EditorGUILayout.PropertyField(_pressedTextColor);
            }

            void NonTextSession()
            {
                if (_useText.boolValue == true)
                    return;

                EditorGUILayout.PropertyField(_defaultSpriteColor);
                EditorGUILayout.PropertyField(_pointerEnterSpriteColor);

                EditorGUILayout.PropertyField(_onSpriteColor);
                EditorGUILayout.PropertyField(_offSpriteColor);

                EditorGUILayout.PropertyField(_pressedSpriteColor);
            }
        }
    }
}
