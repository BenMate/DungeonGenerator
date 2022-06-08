using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DungeonGenerator
{
    [CustomEditor(typeof(DungeonGenerator))]
    public class DungeonGeneratorEdtior : Editor
    {

        public enum DisplayCategory
        {
            Generation, SpecialRooms, FilePaths, Corridors, Debug
        }

        public DisplayCategory categoryToDisplay;

        public override void OnInspectorGUI()
        {
            categoryToDisplay = (DisplayCategory)EditorGUILayout.EnumPopup("Display", categoryToDisplay);

            DungeonGenerator DungeonGen = target as DungeonGenerator;

            EditorGUILayout.Space();


            switch (categoryToDisplay)
            {
                case DisplayCategory.Generation:
                    DisplayGenerationInfo();
                    break;

                case DisplayCategory.SpecialRooms:
                    DisplaySpecialRoomInfo();
                    break;

                case DisplayCategory.FilePaths:
                    DisplayFilePathInfo();
                    break;

                case DisplayCategory.Corridors:
                    DisplayCorridorInfo();
                    break;

                case DisplayCategory.Debug:
                    DisplayDebugInfo();
                    break;
            }


            serializedObject.ApplyModifiedProperties();
        }

        void DisplayGenerationInfo()
        {
            SerializedProperty useRandomSeed = serializedObject.FindProperty("randomSeed");
            EditorGUILayout.PropertyField(useRandomSeed);

            if (!useRandomSeed.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("seed"));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GenerateDeadEnds"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minRoomGap"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("roomCount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("roomChance"));
        }

        void DisplaySpecialRoomInfo()
        {
            SerializedProperty canBossRoomGenerate = serializedObject.FindProperty("canBossRoomGenerate");
            EditorGUILayout.PropertyField(canBossRoomGenerate);

            if (canBossRoomGenerate.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("multipleBossRooms"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useSpawnRoom"));
        }

        void DisplayFilePathInfo()
        {
            SerializedProperty useDefaultFilePath = serializedObject.FindProperty("useDefaultFilePaths");

            EditorGUILayout.PropertyField(useDefaultFilePath);

            if (!useDefaultFilePath.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("roomsFilepath"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enemiesFilepath"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bossRoomFilepath"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("itemsFilepath"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bossEnemiesFilepath"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnRoomFilepath"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("corridorIntersectionFilepath"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("corridorSegmentsFilepath"));
            }
        }

        void DisplayCorridorInfo()
        {

            SerializedProperty forceCorridorsMinLength = serializedObject.FindProperty("forceCorriderMin");
            EditorGUILayout.PropertyField(forceCorridorsMinLength);

            if (!forceCorridorsMinLength.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("corridorLength"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxCorridorIntersections"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CorridorRotationOffset"));
        }

        void DisplayDebugInfo()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("genSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waitForGizmosGen"));
        }


    }
}