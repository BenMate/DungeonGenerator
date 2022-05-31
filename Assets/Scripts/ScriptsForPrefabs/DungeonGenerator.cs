using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Config")]

    [Tooltip("Clamps CorridorLength to Rooms Size, Helps Prevent Rooms from Touching")]
    public bool forceCorriderMin = false;


    public int minRoomGap = 5;

    [Tooltip("The Total Length of the Corridors")]
    public int corridorLength = 10;

    [Tooltip("The Total Amount of Rooms that will be Generated (Big Numbers, Big Wait Time)")]
    public int roomCount = 5;

    [Tooltip("The Chances of Generating a Room")]
    [Range(0, 100)]
    public int roomChance = 50;

    [Tooltip("Max Amount of Times Rooms Can have Segments Between them")]
    public int maxCorridorSegments = 2;

    [Header("GameObjects")]
    public GameObject corridorFloor;

    [Header("DungeonDoor")]
    [Tooltip("Make sure to add the 'DungeonDoor' Script to the prefab")]
    public DungeonDoor dungeonDoor;

    [Tooltip("Invert the way the door faces")]
    public bool invertDoorDirection = false;

    [Header("Debug")]
    [Range(0.0f, 1.0f)]
    public float genSpeed = 1.0f;
    public bool waitForGizmosGen = false;

    //camera data
    public Camera cam;
    public Vector3 offset;

    Vector3 targetPos = Vector3.zero;

    //node data
    List<Node> deletedNodes = new List<Node>();

    Dictionary<Vector3, Node> knownPositions = new Dictionary<Vector3, Node>();

    Node root;
    Node currentNode;

    int totalRooms = 1;
    int nodeCount = 0;

    PrefabDatabase database = new PrefabDatabase();

    GameObject roomContainer;
    GameObject enemyContainer;

    void Start()
    {
        roomContainer = new GameObject("Dungeon Rooms");
        enemyContainer = new GameObject("Dungeon Enemies");

        database.LoadPrefabs();
        StartCoroutine(GenerateDungeon());
    }

    private void Update()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos + offset, Time.deltaTime * 3);
    }

    public IEnumerator GenerateDungeon()
    {
        // deletes dungeon if one laready exists
        DestroyAllChildren();
        CalculateCorridorLength();

        yield return StartCoroutine(GenerateNodes());
        yield return StartCoroutine(GenerateRooms());
        yield return StartCoroutine(GenerateCorridors());
        yield return StartCoroutine(GenerateRoomDoors());


        print($"Total Generated Rooms : {totalRooms} \nTotal Generated Corridors : {nodeCount}");
    }
    IEnumerator GenerateRoomDoors()
    {
        yield return StartCoroutine(GenerateRoomDoor(root));
    }

    IEnumerator GenerateCorridors()
    {
        yield return StartCoroutine(GenerateCorridor(root));
    }
    IEnumerator GenerateRooms()
    {
        yield return StartCoroutine(GenerateRoom(root));
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

            //generate a new position a parent and an ability to have a room.
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
    IEnumerator GenerateRoom(Node node)
    {
        if (genSpeed < 1.0f)
            yield return new WaitForSeconds(1.0f - genSpeed);

        //generate the room if the array isnt empty
        if (node.isRoom && database.allRooms.Length != 0)
        {
            DungeonRoom roomPrefab = Instantiate(database.allRooms[Random.Range(0, database.allRooms.Length)], node.position, Quaternion.identity, roomContainer.transform);

            node.room = roomPrefab;
            targetPos = node.position;

            roomPrefab.SpawnEnemyPrefabs(database.allEnemies, enemyContainer.transform);

        }
        //loop through the children and invoke the funtcion
        foreach (Node child in node.children)
            yield return StartCoroutine(GenerateRoom(child));
    }

    private void CalculateCorridorLength()
    {
        //set min corridor length
        if (forceCorriderMin)
        {
            //loop through each prefab set corrdorlength to be the highest x or z
            int maxLength = 0;

            if (database.allRooms.Length == 0)
                return;

            for (int i = 0; i < database.allRooms.Length; i++)
            {
                Vector3 scale = database.allRooms[i].gameObject.transform.localScale;

                if (scale.z > scale.x)
                    maxLength = (int)(scale.z > maxLength ? scale.z + 5.5f : maxLength);
                else
                    maxLength = (int)(scale.x > maxLength ? scale.x + 5.5f : maxLength);
            }
            corridorLength = maxLength;
        }
    }

    IEnumerator GenerateCorridor(Node node)
    {
        //set the gen speed
        if (genSpeed < 1.0f)
            yield return new WaitForSeconds(1.0f - genSpeed);

        //loop through the nodes children and generates a corridor with a door on each end
        foreach (Node child in node.children)
        {
            //get the direction the parrent is to the child
            Vector3 dir = child.position - node.position;
            dir.Normalize();

            //set the node offset and size
            Vector3 nodePosOffset = node.position + (node.isRoom ? node.room.boundsOffset : Vector3.zero);
            Vector3 nodePosSize = node.isRoom ? node.room.boundsSize : Vector3.zero;

            //set the childs size and offset
            Vector3 childPosOffset = child.position + (child.isRoom ? child.room.boundsOffset : Vector3.zero);
            Vector3 childPosSize = child.isRoom ? child.room.boundsSize : Vector3.zero;

            //make the offset y = 0 to keep on the same level
            Vector3 nodeXZOffset = new Vector3(nodePosOffset.x, 0, nodePosOffset.z);
            nodeXZOffset += Vector3.Scale(dir, nodePosSize) / 2;

            childPosOffset += Vector3.Scale(-dir, childPosSize) / 2;
            Vector3 childXZOffSet = new Vector3(childPosOffset.x, 0, childPosOffset.z);

            //calculate where the offsets are and midpoint
            Vector3 difference = nodeXZOffset - childXZOffSet;
            Vector3 midPoint = (nodeXZOffset + childXZOffSet) / 2;

            //spawn prefab
            if (corridorFloor != null)
            {
                GameObject corridor = Instantiate(corridorFloor, midPoint, Quaternion.identity, transform);
                targetPos = midPoint;

                if (difference.z != 0)
                    corridor.transform.localScale = new Vector3(1, 1, Mathf.Abs(difference.z));
                else if (difference.x != 0)
                    corridor.transform.localScale = new Vector3(Mathf.Abs(difference.x), 1, 1);
            }

            if (node.isRoom)
                node.room.SetCorridorDirection(dir);
            
            if (child.isRoom)   
                child.room.SetCorridorDirection(-dir);

            yield return StartCoroutine(GenerateCorridor(child));
        }
    }

    IEnumerator GenerateRoomDoor(Node node)
    {
        //set the gen speed
        if (genSpeed < 1.0f)
            yield return new WaitForSeconds(1.0f - genSpeed);

        if (node.isRoom)   
            node.room.GenerateDoors(); 
        
        foreach (Node child in node.children)
            yield return StartCoroutine(GenerateRoomDoor(child));


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


}



