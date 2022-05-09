using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator
{
    
    List<RoomNode> allSpaceNodes = new List<RoomNode>();

    int dungeonWidth;
    int dungeonLength;

    public DungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength);
        allSpaceNodes = bsp.PreparenodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        
        List<Node> roomSpaces = StructureHelper.ExtractLowestLeafes(bsp.Rootnode);

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomInGivenSpaces(roomSpaces);

        return new List<Node>(allSpaceNodes);
    }
}