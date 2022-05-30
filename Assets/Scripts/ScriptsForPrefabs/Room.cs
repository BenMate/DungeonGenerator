using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Bounds Config")]
    [Tooltip("The Bounds of the room, the corridors will scale to the bounds")]
    public Vector3 boundsOffset = Vector3.zero;
    public Vector3 boundsSize = Vector3.one;

    [Header("Doors Config")]
    [Tooltip("Four doors are requiered, they are toggled off when needed to")]
    public List<GameObject> doorList = new List<GameObject>(4);

    [Header("Enemys Config - Red")]
    [Tooltip("Create a list of possible enemy spawns")]
    public List<Vector3> possibleEnemySpawns = new List<Vector3>();

    [Tooltip("Max number of enemies per room that can spawn")]
    public int maxEnemyCount = 1;

    [Header("Item Config - Cyan")]
    [Tooltip("List of item Spawn Locations")]
    public List<Vector3> itemLocations = new List<Vector3>();

    public void SpawnEnemyPrefabs(EnemyType[] enemies)
    {
        //temp list
        List<Vector3> tempList = possibleEnemySpawns;

        for (int i = 0; i < maxEnemyCount; i++)
        {
            EnemyType randEnemy = enemies[Random.Range(0, enemies.Length)];
            //random pos
            Vector3 randPos = tempList[Random.Range(0, tempList.Count)];
            //instantiante

            //remove item from list...
        }
    }

    void RestrictDoorCount()
    {
        if (doorList.Count > 4)
            Debug.LogError("cannot have more then 4 doors in the list, trimmed down to 4.");

        //trim the list down to 4
        for (int i = 0; i < doorList.Count; i++)
        {
            if (i > 4)
            {
                doorList.RemoveAt(i);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(transform.position + boundsOffset, boundsSize);

        //every spawn locations draw a cube at the 
        for (int i = 0; i < possibleEnemySpawns.Count; i++)
        {
            Gizmos.color = Color.red;
            Vector3 enemyPos = new Vector3(possibleEnemySpawns[i].x, possibleEnemySpawns[i].y, possibleEnemySpawns[i].z);

            Gizmos.DrawWireCube(transform.position + enemyPos, Vector3.one);
        }

        //every item locations
        for (int i = 0; i < itemLocations.Count; i++)
        {
            Gizmos.color = Color.cyan;
            Vector3 itemPos = new Vector3(itemLocations[i].x, itemLocations[i].y, itemLocations[i].z);
            Gizmos.DrawWireCube(transform.position + itemPos, Vector3.one);
        }

    }


}