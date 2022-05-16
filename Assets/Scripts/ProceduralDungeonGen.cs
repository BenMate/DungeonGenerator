using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProceduralDungeonGen : MonoBehaviour
{

    public int dungeonWidth, dungeonLength;
    public int roomsWidthMin, roomsLengthMin;
    public int maxIterations = 10;
    public int hallwayWidth;
    public Material floorMaterial;

    public float roomBottomCornerOffSet;
    public float roomTopCornerOffSet;
    public int roomOffSet;

    BSPNode root;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateBSPTree()
    {
       //leaf 

        


    }

    void CreateRooms()
    {
        

    }

   


}
