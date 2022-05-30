using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Room room = target as Room;

        if (GUILayout.Button("Auto Generate Bounds"))
        {
            room.CalculateBounds();
        }


        DrawPropertiesExcluding(serializedObject, "m_Script");
    }

}
