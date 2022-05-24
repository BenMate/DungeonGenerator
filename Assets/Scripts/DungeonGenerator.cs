using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int corridorLength = 10;

    public int roomCount = 5;
    [Range(0, 100)] //0-100 chance of generating a room
    public int roomChance = 50;
    public int maxCorridorSegments = 2; //set a max amount of times rooms cannot be generated.

    [Header("GameObjects")]
    public GameObject corridorFloor;
    public GameObject roomPrefab;

    [Header("Debug")]
    [Range(0.0f, 1.0f)]
    public float genSpeed = 1.0f;

    Dictionary<Vector3, Node> knownPositions = new Dictionary<Vector3, Node>();

    Node root;
    Node currentNode;

    int totalRooms = 1;
    int nodeCount = 0;

    List<Node> deletedNodes = new List<Node>();

    void Start()
    {

        GenerateDungeon();

    }

    void GenerateDungeon()
    {
        StartCoroutine(GenerateNodes());
        GenerateCorridors();
        GenerateRooms();

        print($"Total Generated Rooms : {totalRooms}\nTotal Generated Corridors : {nodeCount}");
    }

    void GenerateCorridors()
    {
        GenerateCorridor(root);

    }

    void GenerateCorridor(Node node)
    {
        //loop through nodes children and generates a corridor for each child
        foreach (Node child in node.children)
        {
            Vector3 difference = node.position - child.position;
            Vector3 midPoint = (node.position + child.position) / 2;

            GameObject corridor = Instantiate(corridorFloor, midPoint, Quaternion.identity);

            if (difference.z != 0)
                corridor.transform.localScale = new Vector3(1, 1, Mathf.Abs(difference.z));
            else if (difference.x != 0)
                corridor.transform.localScale = new Vector3(Mathf.Abs(difference.x), 1, 1);

            GenerateCorridor(child);
        }
    }

    void GenerateRooms()
    {

    }

    IEnumerator GenerateNodes()
    {
        //add a root node, also add to known positions
        root = new Node(Vector3.zero);
        knownPositions.Add(root.position, root);
        currentNode = root;

        //the count of each time a room isnt generated.
        int segmentCount = 0;

        while (totalRooms < roomCount)
        {
            //if genSpeed isnt maxed be able to visualise it.
            if (genSpeed < 1.0f)
                yield return new WaitForSeconds(1.0f - genSpeed);

            Vector3 randDir = Direction3D.GetRandomDirectionXZ();
            Vector3 newPos = currentNode.position + randDir * corridorLength;

            //check if the currentNode moved back onto an existing node if so, dont add a room.
            if (knownPositions.ContainsKey(newPos))
            {
                currentNode = knownPositions[newPos];
                segmentCount++;
                continue;
            }

            //generate a new node with:
            //a new position a parent and an ability to have a room.
            Node newNode = new Node(newPos);
            nodeCount++;
            newNode.isRoom = Random.Range(0, 100) < roomChance || segmentCount >= maxCorridorSegments - 1;
            currentNode.children.Add(newNode);
            newNode.parent = currentNode;



            //add new position to known positions
            knownPositions.Add(newPos, newNode);

            //update the current node
            currentNode = newNode;

            //update node count if generated in a room
            if (newNode.isRoom)
            {
                totalRooms++;
                segmentCount = 0;

                //generate room prefab
            }
            else
            {
                segmentCount++;
            }
        }

        List<Node> leafNodes = new List<Node>();
        leafNodes.AddRange(GetLeafNodes(root));

        foreach (Node leaf in leafNodes)
            RemoveDeadEnd(leaf);


    }

    void RemoveDeadEnd(Node node)
    {
        if (node.children.Count == 0 && node.parent != null && !node.isRoom)
        {
            node.parent.children.Remove(node);
            RemoveDeadEnd(node.parent);
            node.parent = null;

            nodeCount--;
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

        foreach (Node node in deletedNodes)
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

        if (node == currentNode)
            Gizmos.color = Color.blue;

        Gizmos.DrawSphere(node.position, 1);

        foreach (Node child in node.children)
        {
            DrawNode(child);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(node.position, child.position);
        }
    }
}



