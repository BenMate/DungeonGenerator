using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : DungeonArea
{
    [Header("Enemys Config - Red")]
    [Tooltip("Create a list of possible enemy spawns")]
    public List<Vector3> possibleEnemySpawns = new List<Vector3>();

    [Tooltip("Max number of enemies per room that can spawn")]
    public int maxEnemyCount = 3;

    [Tooltip("Min number of enemies per room that can spawn")]
    public int minEnemyCount = 0;

    [Header("Item Config - Cyan")]
    [Tooltip("List of Item Spawn Locations")]
    public List<Vector3> itemLocations = new List<Vector3>();

    [Header("Door Config")]
    [Tooltip("the door/wallfiller will spawn on the edge, the offset will move it closer or further")]
    public float doorOffset = 0.0f;

    [Tooltip("All Doors have to be the same Size")]
    public GameObject doorPrefab;

    List<Vector3> posList = new List<Vector3>();

    public void GenerateDoors()
    {
        if (forward)
            Instantiate(doorPrefab, transform.position + Vector3.forward * boundsSize.z / 2, Quaternion.Euler(0, 180, 0));

        if (back)
            Instantiate(doorPrefab, transform.position + Vector3.back * boundsSize.z / 2, Quaternion.Euler(0, 0, 0));

        if (left)
            Instantiate(doorPrefab, transform.position + Vector3.left * boundsSize.x / 2, Quaternion.Euler(0, 90, 0));

        if (right)
            Instantiate(doorPrefab, transform.position + Vector3.right * boundsSize.x / 2, Quaternion.Euler(0, 270, 0));
    }

    public void SpawnEnemyPrefabs(EnemyType[] enemies, Transform parent = null)
    {
        posList = possibleEnemySpawns;

        if (posList.Count == 0 || enemies.Length == 0)
            return;

        for (int i = 0; i < Random.Range(minEnemyCount, maxEnemyCount); i++)
        {
            //spawn random enemy on a random position, shrink list
            int randIndex = Random.Range(0, posList.Count);
            EnemyType randEnemy = enemies[Random.Range(0, enemies.Length)];
            Vector3 randPos = posList[randIndex];

            posList.RemoveAt(randIndex);

            EnemyType enemyPrefab = Instantiate(randEnemy, parent);
            enemyPrefab.transform.position = randPos + transform.position;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        //draw the bounds
        Gizmos.DrawWireCube(transform.position + boundsOffset, boundsSize);

        Gizmos.color = Color.red;

        //every spawn locations draw a cube at the 
        for (int i = 0; i < possibleEnemySpawns.Count; i++)
        {
            Vector3 enemyPos = new Vector3(possibleEnemySpawns[i].x, possibleEnemySpawns[i].y, possibleEnemySpawns[i].z);

            Gizmos.DrawSphere(transform.position + enemyPos, 0.2f);
        }

        Gizmos.color = Color.cyan;

        //every item locations
        for (int i = 0; i < itemLocations.Count; i++)
        {
            Vector3 itemPos = new Vector3(itemLocations[i].x, itemLocations[i].y, itemLocations[i].z);
            Gizmos.DrawWireCube(transform.position + itemPos, Vector3.one);
        }
    }
}