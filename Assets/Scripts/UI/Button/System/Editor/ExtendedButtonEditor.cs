using UnityEditor;
using UnityEditor.UI;

namespace Aftertime.SecretSome.UI.Editor
{
    [CustomEditor(typeof(ExtendedButton))]
    [CanEditMultipleObjects]
    public class ExtendedButtonEditor : ButtonEditor
    {
        private SerializedProperty _textMeshPro;

        // Option
        private SerializedProperty _useUpdateGraphic;
        private SerializedProperty _useObject;
        private SerializedProperty _useSpriteColor;
        private SerializedProperty _useText;
        private SerializedProperty _useSelectState;
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

        // Select
        protected SerializedProperty _selectedObject;
        protected SerializedProperty _selectedSprite;
        protected SerializedProperty _selectedSpriteColor;
        protected SerializedProperty _selectedTextColor;

        // Press
        private SerializedProperty _pressedObject;
        private SerializedProperty _pressedSprite;
        private SerializedProperty _pressedSpriteColor;
        private SerializedProperty _pressedTextColor;
        
        // Focus
        private SerializedProperty _focusBGObject;
        
        // Sound
        private SerializedProperty _clickClip;

        protected override void OnEnable()
        {
            base.OnEnable();

            _useUpdateGraphic = serializedObject.FindProperty("_useUpdateGraphic"); 
            _useObject = serializedObject.FindProperty("_useObject");
            _useSpriteColor = serializedObject.FindProperty("_useSpriteColor");
            _useText = serializedObject.FindProperty("_useText");
            _useSelectState = serializedObject.FindProperty("_useSelectState");
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

            _selectedObject = serializedObject.FindProperty("_selectedObject");
            _selectedSprite = serializedObject.FindProperty("_selectedSprite");
            _selectedSpriteColor = serializedObject.FindProperty("_selectedSpriteColor");
            _selectedTextColor = serializedObject.FindProperty("_selectedTextColor");

            _pressedObject = serializedObject.FindProperty("_pressedObject");
            _pressedSprite = serializedObject.FindProperty("_pressedSprite");
            _pressedSpriteColor = serializedObject.FindProperty("_pressedSpriteColor");
            _pressedTextColor = serializedObject.FindProperty("_pressedTextColor");
            
            _focusBGObject = serializedObject.FindProperty("_focusBGObject");
            
            _clickClip = serializedObject.FindProperty("_clickClip");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(_useSelectState);
            EditorGUILayout.PropertyField(_useUpdateGraphic);

            bool isOnOptionSession = OptionSession();

            if (isOnOptionSession)
            {
                ObjectSession();
                SpriteSession();
                SpriteColorSession();   
            }
            
            EditorGUILayout.PropertyField(_focusBGObject);
            EditorGUILayout.PropertyField(_clickClip);

            serializedObject.ApplyModifiedProperties();
        }

        private bool OptionSession()
        {
            if (_useUpdateGraphic.boolValue == false)
                return false;
            
            EditorGUILayout.PropertyField(_useObject);
            EditorGUILayout.PropertyField(_useText);
            EditorGUILayout.PropertyField(_useSpriteColor);

            return true;
        }

        private bool ObjectSession()
        {
            if (_useObject.boolValue == false)
                return false;

            TextSession();
            NonTextSession();
            
            return true;
            
            void TextSession()
            {
                if (_useText.boolValue == false)
                    return;

                EditorGUILayout.PropertyField(_textMeshPro);

                EditorGUILayout.PropertyField(_defaultObject);
                EditorGUILayout.PropertyField(_defaultTextColor);
                EditorGUILayout.PropertyField(_pointerEnterObject);
                EditorGUILayout.PropertyField(_pointerEnterTextColor);

                EditorGUILayout.PropertyField(_selectedObject);
                EditorGUILayout.PropertyField(_selectedTextColor);

                EditorGUILayout.PropertyField(_pressedObject);
                EditorGUILayout.PropertyField(_pressedTextColor);
            }

            void NonTextSession()
            {
                if (_useText.boolValue == true)
                    return;

                EditorGUILayout.PropertyField(_defaultObject);
                EditorGUILayout.PropertyField(_pointerEnterObject);

                EditorGUILayout.PropertyField(_selectedObject);

                EditorGUILayout.PropertyField(_pressedObject);
            }
        }

        private bool SpriteSession()
        {
            if (_useObject.boolValue == true)
                return false;
            
            if (_useSpriteColor.boolValue == true)
                return false;


            TextSession();
            NonTextSession();
            
            return true;
            
            void TextSession()
            {
                if (_useText.boolValue == false)
                    return;

                EditorGUILayout.PropertyField(_textMeshPro);

                EditorGUILayout.PropertyField(_defaultSprite);
                EditorGUILayout.PropertyField(_defaultTextColor);
                EditorGUILayout.PropertyField(_pointerEnterSprite);
                EditorGUILayout.PropertyField(_pointerEnterTextColor);

                EditorGUILayout.PropertyField(_selectedSprite);
                EditorGUILayout.PropertyField(_selectedTextColor);

                EditorGUILayout.PropertyField(_pressedSprite);
                EditorGUILayout.PropertyField(_pressedTextColor);
            }

            void NonTextSession()
            {
                if (_useText.boolValue == true)
                    return;

                EditorGUILayout.PropertyField(_defaultSprite);
                EditorGUILayout.PropertyField(_pointerEnterSprite);

                EditorGUILayout.PropertyField(_selectedSprite);

                EditorGUILayout.PropertyField(_pressedSprite);
            }
        }

        private bool SpriteColorSession()
        {
            if (_useObject.boolValue == true)
                return false;
            
            if (_useSpriteColor.boolValue == false)
                return false;
            
            EditorGUILayout.PropertyField(_colorDuration);

            TextSession();
            NonTextSession();

            return true;

            void TextSession()
            {
                if (_useText.boolValue == false)
                    return;

                EditorGUILayout.PropertyField(_textMeshPro);

                EditorGUILayout.PropertyField(_defaultSpriteColor);
                EditorGUILayout.PropertyField(_defaultTextColor);
                EditorGUILayout.PropertyField(_pointerEnterSpriteColor);
                EditorGUILayout.PropertyField(_pointerEnterTextColor);

                EditorGUILayout.PropertyField(_selectedSpriteColor);
                EditorGUILayout.PropertyField(_selectedTextColor);

                EditorGUILayout.PropertyField(_pressedSpriteColor);
                EditorGUILayout.PropertyField(_pressedTextColor);
            }

            void NonTextSession()
            {
                if (_useText.boolValue == true)
                    return;

                EditorGUILayout.PropertyField(_defaultSpriteColor);
                EditorGUILayout.PropertyField(_pointerEnterSpriteColor);

                EditorGUILayout.PropertyField(_selectedSpriteColor);

                EditorGUILayout.PropertyField(_pressedSpriteColor);
            }
        }
    }
}
