using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDatabase
{
    public Room[] allRooms;
    public EnemyType[] allEnemies;
    //items

    public void LoadPrefabs()
    {
        allRooms = Resources.LoadAll<Room>("Rooms");
        allEnemies = Resources.LoadAll<EnemyType>("Enemies");

        if (allRooms.Length == 0)
            Debug.Log("PrefabDatabase - Could not find room Prefabs with 'Room' script attatched");

        if (allEnemies.Length == 0)
            Debug.Log("PrefabDatabase - Could not find enemy prefabs with 'EnemyType' script attacthed");
    }
}
