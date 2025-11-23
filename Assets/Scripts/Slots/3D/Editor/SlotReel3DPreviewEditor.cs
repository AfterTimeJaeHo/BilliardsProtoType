using UnityEditor;
using UnityEngine;

namespace Waving.SlotGame.Slots3D.Editor
{
    [CustomEditor(typeof(SlotReel3DPreview))]
    public class SlotReel3DPreviewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SlotReel3DPreview reel = (SlotReel3DPreview)target;
            GUILayout.Space(8f);
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Pull"))
                {
                    reel.PlayPreview();
                }
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Play Mode에서만 Pull 미리보기가 실행됩니다.", MessageType.Info);
            }
        }
    }
}
