using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    //the space the dungeon can generate inside.
    public int dungeonWidth, dungeonLength;

    //rooms data
    public int roomWidthMin, roomLengthMin;

    //how many times we iterate in the bsp
    public int maxIterations;

    //hallway data
    public int hallWayWidth;

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin);
    }
}
