using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    public class PrefabDatabase
    {
        //Objects inside filePaths
        public DungeonRoom[] allRooms;
        public GameObject[] allEnemies;
        public GameObject[] allBosses;
        public DungeonRoom[] bossRooms;
        public GameObject[] allItems;

        public DungeonRoom spawnDungeonRoom;
        public DungeonArea corridorDungeonIntersection;
        public BoundsGenerater CorridorDungeonSegments;

        //custom filePaths

        public string rooms;
        public string enemies;
        public string bossRoom;
        public string items;
        public string bossEnemies;

        public string spawnRoom;
        public string corridorIntersection;
        public string corridorSegments;

        //default filePaths 

        public string defaultRoomsPath = "RoomFolder/Rooms";
        public string defaultEnemiesPath = "Enemies/Enemies";
        public string defaultbossRoomPath = "RoomFolder/BossRoom";
        public string defaultItemsPath = "Items";
        public string defaultBossEnemiesPath = "Enemies/BossEnemy";

        public string defaultSpawnRoomPath = "RoomFolder/SpawnRoom";
        public string defaultCorridorIntersectionPath = "RoomFolder/CorridorIntersection";
        public string defaultCorridorSegmentPath = "RoomFolder/CorridorSegments";

        public void LoadPrefabs()
        {
            //loads every Prefab with correct scirpts attached in the folder.
            allRooms = Resources.LoadAll<DungeonRoom>(rooms);
            bossRooms = Resources.LoadAll<DungeonRoom>(bossRoom);

            //loads every Prefab inside folder
            allEnemies = Resources.LoadAll<GameObject>(enemies);
            allItems = Resources.LoadAll<GameObject>(items);
            allBosses = Resources.LoadAll<GameObject>(bossEnemies);

            //load first Prefab inside folder
            DungeonRoom[] spawnTemp = Resources.LoadAll<DungeonRoom>(spawnRoom);
            if (spawnTemp.Length > 0)
                spawnDungeonRoom = spawnTemp[0];

            DungeonArea[] intersectionTemp = Resources.LoadAll<DungeonArea>(corridorIntersection);
            if (intersectionTemp.Length > 0)
                corridorDungeonIntersection = intersectionTemp[0];

            BoundsGenerater[] segmentTemp = Resources.LoadAll<BoundsGenerater>(corridorSegments);
            if (segmentTemp.Length > 0)
                CorridorDungeonSegments = segmentTemp[0];

            //Database couldnt find Objects in folders/Filepath was off
            if (allRooms.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any Room Prefabs with 'DungeonRoom' Scirpt Attached - Inside 'Rooms' folder ");
            if (allEnemies.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any Enemy prefabs with 'EnemyType' script Attached - Inside 'Enemies' Folder");
            if (bossRooms.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any BossRoom Prefabs with 'DungeonRoom' Scirpt Attached - Inside 'BossRoom' Folder");
            if (allItems.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any Item Prefab - Inside 'Items' Folder");
            if (allBosses.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any Boss Enemy Prefabs - Inside 'BossEnemy' Folder");
            if (spawnDungeonRoom == null)
                Debug.Log("PrefabDatabase - Could not find any Prefabs with 'DungeonRoom' Scirpt Attached - Inside 'SpawnRoom' Folder");
            if (CorridorDungeonSegments == null)
                Debug.Log("PrefabDatabase - Could not find any CorridorSegment Prefab with 'BoundsGenerator' Scirpt Attached - Inside 'CorridorSegment' Folder");
            if (corridorDungeonIntersection == null)
                Debug.Log("PrefabDatabase - Could not Find a 'GameObject' - Inside 'CorridorIntersection' Folder");
        }
    }
}