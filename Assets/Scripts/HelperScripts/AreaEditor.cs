using UnityEngine;
using UnityEditor;

namespace DungeonGenerator
{
    [CustomEditor(typeof(DungeonArea), true)]
    public class AreaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DungeonArea area = target as DungeonArea;

            if (GUILayout.Button("Auto Generate Bounds"))
                area.CalculateBounds();

            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(area);
        }
    }
}
