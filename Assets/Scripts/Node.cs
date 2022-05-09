using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    List<Node> childrenNodeList;

    public List<Node> ChildrenNodeList { get => childrenNodeList;}

    public bool Visited { get; set; }
    public Vector2Int BottomLeftCorner { get; set; }
    public Vector2Int BottomRightCorner { get; set; }
    public Vector2Int TopLeftCorner { get; set; }
    public Vector2Int TopRightCorner { get; set; }

    public Node Parent { get; set; }
    public int TreeLayerIndex { get; set; }

    public Node(Node parentNode)
    {
        childrenNodeList = new List<Node>();
        this.Parent = parentNode;
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    //add a child to the list
    public void AddChild(Node node)
    {
        childrenNodeList.Add(node);
    }

    //remove a child from the list
    public void RemoveChild(Node node)
    {
        childrenNodeList.Remove(node);
    }
}