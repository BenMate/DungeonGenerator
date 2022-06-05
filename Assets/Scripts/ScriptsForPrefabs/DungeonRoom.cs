using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom : MonoBehaviour
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
    [Tooltip("List of Item Spawn Locations")]
    public List<Vector3> itemLocations = new List<Vector3>();

    [Header("Door Config")]

    [Tooltip("the door/wallfiller will spawn on the edge, the offset will move it closer or further")]
    public float doorOffset = 0.0f;

    [Tooltip("All Doors have to be the same Size")]
    public GameObject doorPrefab;

    [Tooltip("Replacement for the Door (Same Size)")]
    public GameObject wallFillerPrefab;

    List<Vector3> posList = new List<Vector3>();

    bool left, right, back, forward;

    public void SetCorridorDirection(Vector3 direction)
    {
        if (direction == Vector3.left)
            left = true;
        else if (direction == Vector3.right)
            right = true;
        else if(direction == Vector3.back)
            back = true;
        else if(direction == Vector3.forward)
            forward = true;
    }

    public void GenerateDoors()
    {

        GameObject doorOrWall = Instantiate(forward ? doorPrefab : wallFillerPrefab, transform.position + Vector3.forward * boundsSize.z / 2, Quaternion.Euler(0, 180, 0));
        doorOrWall.transform.position -= new Vector3(0, 0, doorOffset);

        GameObject doorOrWall2 = Instantiate(back ? doorPrefab : wallFillerPrefab, transform.position + Vector3.back * boundsSize.z / 2, Quaternion.Euler(0, 0, 0));
        doorOrWall2.transform.position -= new Vector3(0, 0, -doorOffset);

        GameObject doorOrWall3 = Instantiate(right ? doorPrefab : wallFillerPrefab, transform.position + Vector3.right * boundsSize.x / 2, Quaternion.Euler(0, 270, 0));
        doorOrWall3.transform.position -= new Vector3(doorOffset, 0, 0);

        GameObject doorOrWall4 = Instantiate(left ? doorPrefab : wallFillerPrefab, transform.position + Vector3.left * boundsSize.x / 2, Quaternion.Euler(0, 90, 0));
        doorOrWall4.transform.position -= new Vector3(-doorOffset, 0, 0);

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
        
        //draw the bounds
        Gizmos.DrawWireCube(transform.position + boundsOffset, boundsSize);

        //every spawn locations draw a cube at the 
        for (int i = 0; i < possibleEnemySpawns.Count; i++)
        {
            Gizmos.color = Color.red;
            Vector3 enemyPos = new Vector3(possibleEnemySpawns[i].x, possibleEnemySpawns[i].y, possibleEnemySpawns[i].z);

            Gizmos.DrawSphere(transform.position + enemyPos, 0.2f);
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