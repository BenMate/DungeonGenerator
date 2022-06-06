using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 position = Vector3.zero;

    public List<Node> children = new List<Node>();

    public Node parent = null;

    public bool isRoom = true;

    public DungeonArea area = null;

    public Node(Vector3 position)
    {
        this.position = position;
    }
}
