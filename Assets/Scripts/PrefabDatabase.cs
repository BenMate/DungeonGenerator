using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDatabase
{
    public DungeonRoom[] allRooms;
    public EnemyType[] allEnemies;
    public DungeonDoor[] allDoors;

    //items

    public void LoadPrefabs()
    {
        allRooms = Resources.LoadAll<DungeonRoom>("DungeonResources/Rooms");
        allEnemies = Resources.LoadAll<EnemyType>("DungeonResources/Enemies");
    
        if (allRooms.Length == 0)
            Debug.LogWarning("PrefabDatabase - Could not find any Room Prefabs with 'Room' script attatched");

        if (allEnemies.Length == 0)
            Debug.LogWarning("PrefabDatabase - Could not find any Enemy prefabs with 'EnemyType' script attacthed");
        
    }
}
