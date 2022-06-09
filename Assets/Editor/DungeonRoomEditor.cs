using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonGenerator
{


    [CustomEditor(typeof(DungeonRoom))]
    public class DungeonRoomEditor : Editor
    {
        public enum DisplayCategory
        {
            Enemies, Items, DoorsAndWalls
        }

        public DisplayCategory categoryToDisplay;

        public override void OnInspectorGUI()
        {
            //calculate bounds
            BoundsGenerater bounds = target as BoundsGenerater;

            if (GUILayout.Button("Auto Generate Bounds"))
                bounds.CalculateBounds();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(bounds);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("boundsOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("boundsSize"));

            EditorGUILayout.Space();

            //add a dropdown tap to edit inspector
            categoryToDisplay = (DisplayCategory)EditorGUILayout.EnumPopup("Display", categoryToDisplay);

            DungeonRoom DungeonGen = target as DungeonRoom;

            EditorGUILayout.Space();

            switch (categoryToDisplay)
            {
                case DisplayCategory.Enemies:
                    DisplayEnemyInfo();
                    break;

                case DisplayCategory.Items:
                    DisplayItemInfo();
                    break;

                case DisplayCategory.DoorsAndWalls:
                    DisplayDoorInfo();
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }

        void DisplayEnemyInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gizmoSize"));
            SerializedProperty allowEnemySpawn = serializedObject.FindProperty("allowEnemiesToSpawn");
            EditorGUILayout.PropertyField(allowEnemySpawn);

            SerializedProperty allowBossSpawn = serializedObject.FindProperty("allowBossSpawns");
            EditorGUILayout.PropertyField(allowBossSpawn);

            if (allowEnemySpawn.boolValue || allowBossSpawn.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("possibleEnemySpawns"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxEnemyCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minEnemyCount"));
            }
        }

        void DisplayItemInfo()
        {
            SerializedProperty allowItems = serializedObject.FindProperty("allowItemsToSpawn");
            EditorGUILayout.PropertyField(allowItems);

            if (allowItems.boolValue)
            {           
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxItemCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minItemCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("itemLocations"));
            }
        }

        void DisplayDoorInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doorPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doorOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doorRotationOffset"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("wallPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wallPosOffset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wallRotationOffset"));
        }

    }
}
