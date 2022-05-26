using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Config")]

    [Tooltip("Clamps CorridorLength to rooms size, helps prevent rooms from touching")]
    public bool forceCorriderMin = false;

    [Tooltip("The Total Length of the corridors")]
    [Range(5.0f, 100.0f)]
    public int corridorLength = 10;

    [Tooltip("The Total Amount of Rooms That Will Be Generated")]
    [Range(1, 200)]
    public int roomCount = 5;

    [Tooltip("The chances of Generating A Room")]
    [Range(0, 100)]
    public int roomChance = 50;

    [Tooltip("Max Amount of Times Rooms Can have segments between them")]
    public int maxCorridorSegments = 2;

    [Header("GameObjects")]
    public GameObject corridorFloor;

    [Tooltip("Put Prefabs in this list that have 'room' scrips attached")]
    public Room[] roomPreFabs;

    [Header("Debug")]
    [Range(0.0f, 1.0f)]
    public float genSpeed = 1.0f;
    public bool waitForGizmosGen = false;

    public Camera cam;
    public Vector3 offset;

    Vector3 targetPos = Vector3.zero;

    Dictionary<Vector3, Node> knownPositions = new Dictionary<Vector3, Node>();
    Dictionary<Node, Room> nodeBoundsPair = new Dictionary<Node, Room>();

    List<Node> deletedNodes = new List<Node>(); //testing which nodes got deleted

    List<Room> doors = new List<Room>(); //a list of doors

    Node root;
    Node currentNode;

    int totalRooms = 1;
    int nodeCount = 0;

    void Start()
    {
        StartCoroutine(GenerateDungeon());

    }

    private void Update()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos + offset, Time.deltaTime * 3);
    }

    public IEnumerator GenerateDungeon()
    {

        DestroyAllChildren();

        CalculateCorridorLength();

        yield return StartCoroutine(GenerateNodes());
        yield return StartCoroutine(GenerateRooms());
        yield return StartCoroutine(GenerateCorridors());
        yield return StartCoroutine(ToggleRoomDoors());

        print($"Total Generated Rooms : {totalRooms} \nTotal Generated Corridors : {nodeCount}");
    }

    private void CalculateCorridorLength()
    {
        //set min corridor length
        if (forceCorriderMin)
        {
            //loop through each prefab set corrdorlength to be the highest x or z
            int maxLength = 0;
            for (int i = 0; i < roomPreFabs.Length; i++)
            {
                Vector3 scale = roomPreFabs[i].gameObject.transform.localScale;

                if (scale.z > scale.x)
                    maxLength = (int)(scale.z > maxLength ? scale.z + 0.5f : maxLength);
                else
                    maxLength = (int)(scale.x > maxLength ? scale.x + 0.5f : maxLength);
            }
            corridorLength = maxLength;
        }
    }

    IEnumerator ToggleRoomDoors()
    {
        yield return StartCoroutine(ToggleRoomDoor());
    }

    IEnumerator ToggleRoomDoor()
    {
        //set the gen speed
        if (genSpeed < 1.0f)
            yield return new WaitForSeconds(1.0f - genSpeed);

        //todo: gain the knowledge to disable 2 of the 4 doors in the list

        //get a list of all doors

        //check if the door should be disabled

        // -check if door is near corridor???
        // -check on the xor z axis to tell the room what door should be disabled
        // -check on the 

    }

    IEnumerator GenerateCorridors()
    {
        yield return StartCoroutine(GenerateCorridor(root));
    }

    IEnumerator GenerateCorridor(Node node)
    {
        //set the gen speed
        if (genSpeed < 1.0f)
            yield return new WaitForSeconds(1.0f - genSpeed);

        //loop through the nodes children and generates a corridor for each child
        foreach (Node child in node.children)
        {

            //get the direction the parrent is to the child
            Vector3 dir = child.position - node.position;
            dir.Normalize();

            //set the node offset and size
            Vector3 nodePosOffset = node.position + (node.isRoom ? nodeBoundsPair[node].offset : Vector3.zero);
            Vector3 nodePosSize = node.isRoom ? nodeBoundsPair[node].size : Vector3.zero;
            nodePosOffset += Vector3.Scale(dir, nodePosSize) / 2;

            //set the childs size and offset
            Vector3 childPosOffset = child.position + (child.isRoom ? nodeBoundsPair[child].offset : Vector3.zero);
            Vector3 childPosSize = child.isRoom ? nodeBoundsPair[child].size : Vector3.zero;
            childPosOffset += Vector3.Scale(-dir, childPosSize) / 2;

            Vector3 difference = nodePosOffset - childPosOffset;
            Vector3 midPoint = (nodePosOffset + childPosOffset) / 2;

            GameObject corridor = Instantiate(corridorFloor, midPoint, Quaternion.identity, transform);
            targetPos = midPoint;

            if (difference.z != 0)
                corridor.transform.localScale = new Vector3(1, 1, Mathf.Abs(difference.z));
            else if (difference.x != 0)
                corridor.transform.localScale = new Vector3(Mathf.Abs(difference.x), 1, 1);

            yield return StartCoroutine(GenerateCorridor(child));
        }
    }

    IEnumerator GenerateRooms()
    {
        yield return StartCoroutine(GenerateRoom(root));
    }

    IEnumerator GenerateRoom(Node node)
    {
        if (genSpeed < 1.0f)
            yield return new WaitForSeconds(1.0f - genSpeed);

        //generate the room if the list isnt empty
        if (node.isRoom && roomPreFabs.Length != 0)
        {
            Room roomPrefab = Instantiate(roomPreFabs[Random.Range(0, roomPreFabs.Length)], node.position, Quaternion.identity, transform);

            nodeBoundsPair.Add(node, roomPrefab);

            targetPos = node.position;
        }
        //loop through the children and invoke the funtcion
        foreach (Node child in node.children)
            yield return StartCoroutine(GenerateRoom(child));
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
            if (genSpeed < 1.0f && waitForGizmosGen)
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

    void DestroyAllChildren()
    {
        //loop trough the nodes and delete all children
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
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

    //debug functions

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



