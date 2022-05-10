
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    int maxIterations;
    int roomLengthMin;
    int roomWidthMin;

    public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
    {
        this.maxIterations = maxIterations;
        this.roomLengthMin = roomLengthMin;
        this.roomWidthMin  = roomWidthMin;
    }

    public List<RoomNode> GenerateRoomInGivenSpaces(List<Node> roomSpaces, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(
                space.BottomLeftCorner, space.TopRightCorner,
                roomBottomCornerModifier, roomOffset);

            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(
                space.BottomLeftCorner, space.TopRightCorner,
                roomTopCornerModifier, roomOffset);

            //we are modifying the points in the hiechy
            space.BottomLeftCorner = newBottomLeftPoint;
            space.TopRightCorner = newTopRightPoint;
            space.BottomRightCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            listToReturn.Add((RoomNode)space);
        }
        return listToReturn;
    
    }
}