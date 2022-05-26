using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3 offset = Vector3.zero;
    public Vector3 size = Vector3.one;

    //4 doors are requiered, they are turned off for corridors to appear
    public List<GameObject> doorList = new List<GameObject>(4);

    private void Start()
    {
        //RestrictDoorListCount();

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + offset, size);
    }

    void RestrictDoorListCount()
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
}