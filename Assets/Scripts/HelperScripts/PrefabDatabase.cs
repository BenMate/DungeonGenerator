using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    public class PrefabDatabase
    {
        public DungeonRoom[] allRooms;
        public GameObject[] allEnemies;
        public GameObject[] allBosses;
        public DungeonRoom[] bossRooms;
        public GameObject[] allItems;

        public DungeonRoom spawnRoom;
        public DungeonArea corridorIntersection;
        public GameObject corridorSegment;

        public void LoadPrefabs()
        {
            allRooms = Resources.LoadAll<DungeonRoom>("DungeonResources/RoomFolder/Rooms");
            allEnemies = Resources.LoadAll<GameObject>("DungeonResources/Enemies/Enemies");
            bossRooms = Resources.LoadAll<DungeonRoom>("DungeonResources/RoomFolder/BossRoom");
            allItems = Resources.LoadAll<GameObject>("DungeonResources/Items");
            allBosses = Resources.LoadAll<GameObject>("DungeonResources/Enemies/BossEnemy");

            spawnRoom = Resources.LoadAll<DungeonRoom>("DungeonResources/RoomFolder/SpawnRoom")[0];
            corridorIntersection = Resources.LoadAll<DungeonArea>("DungeonResources/RoomFolder/CorridorIntersection")[0];
            corridorSegment = Resources.LoadAll<GameObject>("DungeonResources/RoomFolder/CorridorSegments")[0];

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


            if (spawnRoom == null)
                Debug.Log("PrefabDatabase - Could not find any Room Prefabs with 'DungeonRoom' Scirpt Attaced - Inside 'SpawnRoom' Folder");
            if (corridorSegment == null)
                Debug.Log("PrefabDatabase - Could not Find a 'GameObject' - Inside 'CorridorSegments' Folder");
            if (corridorIntersection == null)
                Debug.Log("PrefabDatabase - Could not Find a 'GameObject' - Inside 'CorridorIntersection' Folder");
        }
    }
}