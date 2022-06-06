using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonArea : MonoBehaviour
{
    [Header("Bounds Config")]
    [Tooltip("The Bounds of the room, the corridors will scale to the bounds")]
    public Vector3 boundsOffset = Vector3.zero;
    public Vector3 boundsSize = Vector3.one;

    [Tooltip("Wall Prefab")]
    public GameObject wallPrefab;

    protected bool left, right, back, forward;

    public void SetCorridorDirection(Vector3 direction)
    {
        if (direction == Vector3.left)
            left = true;
        else if (direction == Vector3.right)
            right = true;
        else if (direction == Vector3.back)
            back = true;
        else if (direction == Vector3.forward)
            forward = true;
    }

    public void GenerateWalls()
    {
        if (!forward)
            Instantiate(wallPrefab, transform.position + Vector3.forward * boundsSize.z / 2, Quaternion.Euler(0, 180, 0));

        if (!back)        
            Instantiate(wallPrefab, transform.position + Vector3.back * boundsSize.z / 2, Quaternion.Euler(0, 0, 0));

        if (!left)
            Instantiate(wallPrefab, transform.position + Vector3.left * boundsSize.x / 2, Quaternion.Euler(0, 90, 0));

        if (!right)     
            Instantiate(wallPrefab, transform.position + Vector3.right * boundsSize.x / 2, Quaternion.Euler(0, 270, 0));
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
    }
}
