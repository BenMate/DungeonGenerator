using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(DungeonRoom))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DungeonRoom room = target as DungeonRoom;

        if (GUILayout.Button("Auto Generate Bounds"))
        {
            room.CalculateBounds();
        }

        DrawPropertiesExcluding(serializedObject, "m_Script");
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(room);
        }
    }

}
