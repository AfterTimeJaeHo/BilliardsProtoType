using UnityEditor;
using UnityEngine;

namespace Aftertime.SecretSome.UI.Layout
{
    [CustomEditor(typeof(LayoutManager))]
    public class LayoutManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty layouts;

        private void OnEnable()
        {
            layouts = serializedObject.FindProperty("layouts");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add All Layout"))
            {

                layouts.ClearArray();
                
                LayoutBase[] layoutArray = FindObjectsByType<LayoutBase>(FindObjectsSortMode.None);
                foreach (LayoutBase layout in layoutArray)
                {
                    layouts.arraySize++;
                    layouts.GetArrayElementAtIndex(layouts.arraySize - 1).boxedValue = layout;    
                }
                
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
   
}
