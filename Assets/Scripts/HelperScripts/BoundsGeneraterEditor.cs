using UnityEngine;
using UnityEditor;

namespace DungeonGenerator
{
    [CustomEditor(typeof(BoundsGenerater), true)]
    public class BoundsGeneraterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            BoundsGenerater area = target as BoundsGenerater;

            if (GUILayout.Button("Auto Generate Bounds"))
                area.CalculateBounds();

            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(area);
        }
    }
}
