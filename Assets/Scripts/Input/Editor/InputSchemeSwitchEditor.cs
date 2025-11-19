using UnityEditor;

namespace Aftertime.TeachingAssistantLife.Editor
{
    [CustomEditor(typeof(InputSchemeSwitch))]
    public class InputSchemeSwitchEditor : UnityEditor.Editor
    {
        private SerializedProperty _playerInput;

        private void OnEnable()
        {
            _playerInput = serializedObject.FindProperty("_playerInput");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
   
}
