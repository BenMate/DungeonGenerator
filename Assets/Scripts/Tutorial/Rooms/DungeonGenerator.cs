using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator
{
    
    List<RoomNode> AllNodesCollection = new List<RoomNode>();

    int dungeonWidth;
    int dungeonLength;

    public DungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }

    public List<Node> CalculateDungeon(int maxIterations, int roomWidthMin, int roomLengthMin,
        float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int hallwayWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength);
        AllNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.ExtractLowestLeafes(bsp.Rootnode);

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomInGivenSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerModifier, roomOffset);

        HallwayGenerator corridorGenerator = new HallwayGenerator();
        var hallwayList = corridorGenerator.CreateHallway(AllNodesCollection, hallwayWidth);

        return new List<Node>(roomList).Concat(hallwayList).ToList();
    }
}