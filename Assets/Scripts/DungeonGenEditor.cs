using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DungeonGenerator dungeonGenerator = (DungeonGenerator)target;

        //cant generate because this isnt on MonoBehaviour
        //if (GUILayout.Button("CreateNewDungeon"))
        //{
        //   dungeonGenerator.GenerateDungeon();
        //}

    }


}
