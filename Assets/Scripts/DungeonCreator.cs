using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int hallWayWidth;
    public Material material;
    [Range(0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerModifier;
    [Range(0, 2)]
    public int roomOffset;

    public GameObject WallZAxis, WallXAxis;
    List<Vector3Int> DoorVertPos;
    List<Vector3Int> DoorHorizPos;
    List<Vector3Int> WallHorizPos;
    List<Vector3Int> WallVertPos;

    public

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CreateDungeon()
    {
        //delete old dungeon if there is one
        DestroyAllChildren();

        //create dungeon 
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateDungeon(maxIterations, roomWidthMin, roomLengthMin,
            roomBottomCornerModifier, roomTopCornerModifier, roomOffset, hallWayWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;

        DoorVertPos = new List<Vector3Int>();
        DoorHorizPos = new List<Vector3Int>();
        WallHorizPos = new List<Vector3Int>();
        WallVertPos = new List<Vector3Int>();

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftCorner, listOfRooms[i].TopRightCorner);
        }
        CreateWalls(wallParent);
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPos in WallHorizPos)
        {
            CreateWall(wallParent, wallPos, WallXAxis);
        }

        foreach (var wallpos in WallVertPos)
        {
            CreateWall(wallParent, wallpos, WallZAxis);
        }
    }

    void CreateWall(GameObject wallParent, Vector3Int wallPos, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPos, Quaternion.identity, wallParent.transform);
    }

    void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightcorner)
    {
        Vector3 bottomleftVertex = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightVertex = new Vector3(topRightcorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftVertex = new Vector3(bottomLeftCorner.x, 0, topRightcorner.y);
        Vector3 topRightVertex = new Vector3(topRightcorner.x, 0, topRightcorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftVertex,topRightVertex,
            bottomleftVertex, bottomRightVertex
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0, 1, 2, 2, 1, 3
        };

        //align the vertices to create the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        //create an object to have the mesh
        GameObject dungFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungFloor.transform.position = Vector3.zero;
        dungFloor.transform.localScale = Vector3.one;

        //set obj's mesh and material
        dungFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungFloor.GetComponent<MeshRenderer>().material = material;
        dungFloor.transform.parent = transform;

        for (int row = (int)bottomleftVertex.x; row < (int)bottomRightVertex.x + 1; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomleftVertex.z);
            AddWallPositionToList(wallPosition, WallHorizPos, DoorHorizPos);
        }

        for (int row = (int)topLeftVertex.x; row < (int)topRightVertex.x + 1; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightVertex.z);
            AddWallPositionToList(wallPosition, WallHorizPos, DoorHorizPos);
        }

        for (int col = (int)bottomleftVertex.z; col < (int)topLeftVertex.z + 1; col++)
        {
            var wallPosition = new Vector3(bottomleftVertex.x, 0, col);
            AddWallPositionToList(wallPosition, WallVertPos, DoorVertPos);
        }

        for (int col = (int)bottomRightVertex.z; col < (int)topRightVertex.z + 1; col++)
        {
            var wallPosition = new Vector3(bottomRightVertex.x, 0, col);
            AddWallPositionToList(wallPosition, WallVertPos, DoorVertPos);
        }
    }

    void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);

        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
            wallList.Add(point);
        
    }

    void DestroyAllChildren()
    {
        while(transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}
