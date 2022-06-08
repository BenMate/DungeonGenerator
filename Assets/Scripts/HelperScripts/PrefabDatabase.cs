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

        public string defaultRoomsPath = "DungeonResources/RoomFolder/Rooms";
        public string defaultEnemiesPath = "DungeonResources/Enemies/Enemies";
        public string defaultbossRoomPath = "DungeonResources/RoomFolder/BossRoom";
        public string defaultItemsPath = "DungeonResources/Items";
        public string defaultBossEnemiesPath = "DungeonResources/Enemies/BossEnemy";

        public string defaultSpawnRoomPath = "DungeonResources/RoomFolder/SpawnRoom";
        public string defaultCorridorIntersectionPath = "DungeonResources/RoomFolder/CorridorIntersection";
        public string defaultCorridorSegmentPath = "DungeonResources/RoomFolder/CorridorSegments";

        public void LoadPrefabs()
        {
            allRooms = Resources.LoadAll<DungeonRoom>(rooms);
            allEnemies = Resources.LoadAll<GameObject>(enemies);
            bossRooms = Resources.LoadAll<DungeonRoom>(bossRoom);
            allItems = Resources.LoadAll<GameObject>(items);
            allBosses = Resources.LoadAll<GameObject>(bossEnemies);

            //there has to be rooms, corridors and intsersections prefabs for the dungeon to work
            spawnDungeonRoom = Resources.LoadAll<DungeonRoom>(spawnRoom)[0];
            corridorDungeonIntersection = Resources.LoadAll<DungeonArea>(corridorIntersection)[0];
            CorridorDungeonSegments = Resources.LoadAll<BoundsGenerater>(corridorSegments)[0];

            //Objects in an array
            if (allRooms.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any Room Prefabs with 'DungeonRoom' Scirpt Attaced - Inside 'Rooms' folder ");
            if (allEnemies.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any Enemy prefabs with 'EnemyType' script attacthed - Inside 'Enemies' Folder");
            if (bossRooms.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any BossRoom Prefabs with 'DungeonRoom' Scirpt Attaced - Inside 'BossRoom' Folder");
            if (allItems.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any Item Prefab - Inside 'Items' Folder");
            if (allBosses.Length == 0)
                Debug.Log("PrefabDatabase - Could not find any Boss Enemy Prefabs - Inside 'BossEnemy' Folder");

            //single objects
            if (spawnDungeonRoom == null)
                Debug.Log("PrefabDatabase - Could not find any Room Prefabs with 'DungeonRoom' Scirpt Attaced - Inside 'SpawnRoom' Folder");
            if (CorridorDungeonSegments == null)
                Debug.Log("PrefabDatabase - Could not find any CorridorSegment Prefab with 'BoundsGenerator' Scirpt Attaced - Inside 'CorridorSegment' Folder");
            if (corridorDungeonIntersection == null)
                Debug.Log("PrefabDatabase - Could not Find a 'GameObject' - Inside 'CorridorIntersection' Folder");
        }
    }
}