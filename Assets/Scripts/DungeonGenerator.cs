using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int corridorLength = 10;
    public int roomCount = 5;
    [Range(0, 100)] //0-100 chance of generating a room
    public int roomChance = 50;

    Dictionary<Vector3, Node> knownPositions = new Dictionary<Vector3, Node>();

    Node root;
    Node currentNode;

    int nodeCount = 1;

    List<Node> deletedNodes = new List<Node>();

    void Start()
    {
        //add a root node, also add it to known positions
        root = new Node(Vector3.zero);
        knownPositions.Add(root.position, root);
        currentNode = root;

        while (nodeCount < roomCount)
        {
            Vector3 randDir = Direction3D.GetRandomDirectionXZ();
            Vector3 newPos = currentNode.position + randDir * corridorLength;

            //check if currentnode moved back onto an existing node, dont add room.
            if (knownPositions.ContainsKey(newPos))
            {
                currentNode = knownPositions[newPos];
                continue;
            }

            //generate a new node with:
            //a new position and parent
            //ability to have a room.
            Node newNode = new Node(newPos);
            newNode.isRoom = Random.Range(0, 100) < roomChance;
            currentNode.children.Add(newNode);
            newNode.parent = currentNode;


            //add new position to known positions
            knownPositions.Add(newPos, newNode);

            //update the current node
            currentNode = newNode;

            //update node count if generated in a room
            if (newNode.isRoom)
                nodeCount++;
        }

        List<Node> leafNodes = new List<Node>();
        leafNodes.AddRange(GetLeafNodes(root));

        foreach (Node leaf in leafNodes)
            RemoveDeadEnd(leaf);

        //temp
        print(nodeCount);
    }

    void RemoveDeadEnd(Node node)
    {
        if (node.children.Count == 0 && node.parent != null && !node.isRoom)
        {
            node.parent.children.Remove(node);
            RemoveDeadEnd(node.parent);
            node.parent = null;
        }
    }

    List<Node> GetLeafNodes(Node node)
    {
        List<Node> leafNodes = new List<Node>();

        if (node.children.Count == 0)
            leafNodes.Add(node);

        foreach (Node child in node.children)
            leafNodes.AddRange(GetLeafNodes(child));

        return leafNodes;
    }

    void OnDrawGizmos()
    {
        if (root != null)
            DrawNode(root);

        foreach(Node node in deletedNodes)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(node.position, 1);
        }
    }

    void DrawNode(Node node)
    {
        Gizmos.color = Color.red;

        if (node.isRoom)
            Gizmos.color = Color.green;

        Gizmos.DrawSphere(node.position, 1);

        foreach (Node child in node.children)
        {
            DrawNode(child);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(node.position, child.position);
        }
    }
}

public class Node
{
    public Vector3 position = Vector3.zero;

    public List<Node> children = new List<Node>();

    public Node parent = null;

    public bool isRoom = true;

    public Node(Vector3 position)
    {
        this.position = position;
    }
}

public static class Direction3D
{
    static List<Vector3> directions = new List<Vector3>()
    {
        Vector3.forward,
        Vector3.back,
        Vector3.right,
        Vector3.left
    };

    public static Vector3 GetRandomDirectionXZ()
    {
        //returns a random value from the list of directions
        return directions[Random.Range(0, directions.Count)];
    }
}