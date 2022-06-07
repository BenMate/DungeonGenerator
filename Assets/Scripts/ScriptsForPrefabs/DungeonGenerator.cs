using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGenerator
{
    public class DungeonGenerator : MonoBehaviour
    {
        //room generation
        [Header("Gen Config")]
        [Tooltip("Can Corridors Lead to Nothing / special rooms")]
        public bool GenerateDeadEnds = false;

        [Tooltip("The Amount of Corridor Space Between each Room")]
        public float minRoomGap = 5.5f;

        [Tooltip("The Total Amount of Rooms that will be Generated (Big Numbers, Big Wait Time)")]
        [Range(0, 1000)]
        public int roomCount = 5;

        [Tooltip("The Chances of Generating a Room")]
        [Range(0, 100)]
        public int roomChance = 50;

        [Header("Special Room Config")]
        [Tooltip("Gives the Ability to Generate Boss Rooms")]
        public bool CanBossRoomGenerate = true;

        [Tooltip("Makes it Possible More Than One Boss Room can Spawn")]
        public bool multipleBossRooms = false;

        //corridor
        [Header("Corridor Config")]
        [Tooltip("Clamps CorridorLength to Rooms Size, Helps Prevent Rooms from Touching")]
        public bool forceCorriderMin = false;

        [Tooltip("The Total Length of the Corridors")]
        public int corridorLength = 10;

        [Tooltip("Max Amount of Times Rooms Can have Segments Between them")]
        public int maxCorridorSegments = 2;

        [Tooltip("Changes the rotation of the corridors")]
        public float CorridorRotationOffset;

        //simple editor debugs
        [Header("Debug")]
        [Range(0.0f, 1.0f)]
        public float genSpeed = 1.0f;
        public bool waitForGizmosGen = false;

        List<Node> deletedNodes = new List<Node>();
        Dictionary<Vector3, Node> knownPositions = new();

        Node root;
        Node currentNode;

        int totalRooms = 1;
        int nodeCount = 0;
        int bossCount = 0;

        PrefabDatabase database = new PrefabDatabase();

        GameObject roomContainer;
        GameObject enemyContainer;
        GameObject corridorContainer;
        GameObject doorContainer;

        void Start()
        {
            Random.InitState(0);

            roomContainer = new GameObject("Dungeon Rooms");
            enemyContainer = new GameObject("Dungeon Enemies");
            corridorContainer = new GameObject("Dungeon Corridors");
            doorContainer = new GameObject("Dungeon Doors");


            database.LoadPrefabs();
            StartCoroutine(GenerateDungeon());
        }

        public IEnumerator GenerateDungeon()
        {
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

            if (!GenerateDeadEnds)
            {
                foreach (Node leaf in leafNodes)
                    RemoveDeadEnd(leaf);
            }
        }
        IEnumerator GenerateRoom(Node node)
        {
            if (genSpeed < 1.0f)
                yield return new WaitForSeconds(1.0f - genSpeed);

            //generate spawn room if they have provided a prefab
            if (node.parent == null && database.spawnRoom != null)
            {
                DungeonRoom spawn = Instantiate(database.spawnRoom, node.position, Quaternion.identity, roomContainer.transform);
                node.area = spawn;
            }

            //dead ends can generate boss room
            else if (node.children.Count == 0 && database.bossRooms.Length != 0 && CanBossRoomGenerate)
            {
                bossCount++;

                if (!multipleBossRooms && bossCount > 0)
                    CanBossRoomGenerate = false;

                DungeonRoom bRoom = Instantiate(database.bossRooms[Random.Range(0, database.bossRooms.Length)], node.position, Quaternion.identity, roomContainer.transform);
                node.area = bRoom;

                if(bRoom.allowBossSpawns)
                bRoom.SpawnEnemyPrefabs(database.allBosses, enemyContainer.transform);
            }

            //generate the room if the array isnt empty
            else if (node.isRoom && database.allRooms.Length != 0)
            {
                DungeonRoom room = Instantiate(database.allRooms[Random.Range(0, database.allRooms.Length)], node.position, Quaternion.identity, roomContainer.transform);

                node.area = room;

                room.SpawnEnemyPrefabs(database.allEnemies, enemyContainer.transform);
            }

            //generate corridorIntersection
            else if (!node.isRoom && database.corridorIntersection != null)
            {
                DungeonArea intersection = Instantiate(database.corridorIntersection, node.position, Quaternion.identity, roomContainer.transform).GetComponent<DungeonArea>();

                node.area = intersection;
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
                    Vector3 scale = database.allRooms[i].boundsSize;

                    if (scale.z > scale.x)
                        maxLength = (int)(scale.z > maxLength ? scale.z + minRoomGap : maxLength);
                    else
                        maxLength = (int)(scale.x > maxLength ? scale.x + minRoomGap : maxLength);
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
                Vector3 nodePosOffset = node.position + node.area.boundsOffset;
                Vector3 nodePosSize = node.area.boundsSize;

                //set the childs size and offset
                Vector3 childPosOffset = child.position + child.area.boundsOffset;
                Vector3 childPosSize = child.area.boundsSize;

                //make the offset y = 0 to keep on the same level
                Vector3 nodeXZOffset = new Vector3(nodePosOffset.x, 0, nodePosOffset.z);
                nodeXZOffset += Vector3.Scale(dir, nodePosSize) / 2;

                childPosOffset += Vector3.Scale(-dir, childPosSize) / 2;
                Vector3 childXZOffSet = new Vector3(childPosOffset.x, 0, childPosOffset.z);

                //calculate where the offsets are
                Vector3 difference = nodeXZOffset - childXZOffSet;

                float tempLength = 3;

                //spawn segment Prefab
                if (database.corridorSegment != null)
                {
                    //how many segments are needed to be placed and total length
                    float totalLength = (Vector3.Distance(difference, Vector3.zero));
                    int segmentCount = (int)(totalLength / tempLength);

                    //the total length of all segments
                    float totalSegmentLength = segmentCount * tempLength;

                    //the remainder of length from the bounds 
                    float remainderLength = totalLength - totalSegmentLength;

                    //how much to scale each segment
                    float segmentScale = remainderLength / segmentCount;

                    Vector3 dirOffset = difference.normalized * (tempLength + segmentScale);


                    for (int i = 0; i < segmentCount; i++)
                    {
                        Quaternion angle = Quaternion.Euler(0, 0, 0);

                        if (dir == Vector3.left || dir == Vector3.right)
                            angle = Quaternion.Euler(0, 90 + CorridorRotationOffset, 0);
                        if (dir == Vector3.forward || dir == Vector3.back)
                            angle = Quaternion.Euler(0, CorridorRotationOffset, 0);

                        GameObject segment = Instantiate(database.corridorSegment, nodeXZOffset - dirOffset * i - dirOffset / 2, angle, corridorContainer.transform);
                        Vector3 scale = segment.transform.localScale * (segmentScale / tempLength);
                        segment.transform.localScale += new Vector3(0, 0, scale.z * Mathf.Abs(Vector3.Distance(dir, Vector3.zero)));
                    }
                }

                node.area.SetCorridorDirection(dir);
                child.area.SetCorridorDirection(-dir);

                yield return StartCoroutine(GenerateCorridor(child));
            }
        }

        IEnumerator GenerateRoomDoor(Node node)
        {
            //set the gen speed
            if (genSpeed < 1.0f)
                yield return new WaitForSeconds(1.0f - genSpeed);

            if (node.isRoom)
                (node.area as DungeonRoom).GenerateDoors(doorContainer.transform);

            node.area.GenerateWalls(doorContainer.transform);

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
}


