using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Bounds Config")]
    [Tooltip("The Bounds of the room, the corridors will scale to the bounds")]
    public Vector3 boundsOffset = Vector3.zero;
    public Vector3 boundsSize = Vector3.one;


    [Header("Enemys Config - Red")]
    [Tooltip("Create a list of possible enemy spawns")]
    public List<Vector3> possibleEnemySpawns = new List<Vector3>();

    [Tooltip("Max number of enemies per room that can spawn")]
    public int maxEnemyCount = 3;

    [Tooltip("Min number of enemies per room that can spawn")]
    public int minEnemyCount = 0;

    [Header("Item Config - Cyan")]
    [Tooltip("List of item Spawn Locations")]
    public List<Vector3> itemLocations = new List<Vector3>();

    List<Vector3> posList = new List<Vector3>();

    public void SpawnEnemyPrefabs(EnemyType[] enemies, Transform parent = null)
    {
        posList = possibleEnemySpawns;

        for (int i = 0; i < Random.Range(minEnemyCount, maxEnemyCount); i++)
        {
            if (posList.Count == 0 || enemies.Length == 0)
                break;

            //spawn random enemy on a random position, shrink list
            int randIndex = Random.Range(0, posList.Count);
            EnemyType randEnemy = enemies[Random.Range(0, enemies.Length)];
            Vector3 randPos = posList[randIndex];

            posList.RemoveAt(randIndex);

            EnemyType enemyPrefab = Instantiate(randEnemy, parent);
            enemyPrefab.transform.position = randPos + transform.position;

        }
    }

    public void CalculateBounds()
    {
        Bounds bounds = Encap(transform, new Bounds());
        boundsOffset = bounds.center;
        boundsSize = bounds.size;
    }

    Bounds Encap(Transform parent, Bounds blocker)
    {
        if (parent.childCount == 0)
        {
            Renderer rend = parent.GetComponent<Renderer>();

            if (rend != null)
                blocker.Encapsulate(rend.bounds);

            return blocker;
        }

        foreach (Transform child in parent)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            
            if (renderer != null)
                blocker.Encapsulate(renderer.bounds);

            blocker = Encap(child, blocker);
        }
        return blocker;
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