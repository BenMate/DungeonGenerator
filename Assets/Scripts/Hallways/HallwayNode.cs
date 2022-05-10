using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class HallwayNode : Node
{
    Node structure1;
    Node structure2;
    int hallwayWidth;
    int modifierDistanceFromWall = 1; //to default it away from the corners

    public HallwayNode(Node node1, Node node2, int hallwayWidth) : base(null)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.hallwayWidth = hallwayWidth;
        GenerateHallway();
    }

    void GenerateHallway()
    {
        var relativePositionofStructure2 = CheckPosStructures();
        switch (relativePositionofStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomInRelationUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomInRelationUpOrDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomInRelationLeftOrRight(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomInRelationLeftOrRight(this.structure2, this.structure1);
                break;
            default:
                break;
        }
    }

    private void ProcessRoomInRelationLeftOrRight(Node structure1, Node structure2)
    {
        Node leftStructure = null;
        List<Node> leftStructureChildren = StructureHelper.ExtractLowestLeafes(structure1);
        Node rightStructure = null;
        List<Node> rightStructureChildren = StructureHelper.ExtractLowestLeafes(structure2);

        //sorted all the children on the left by the x value
        var sortedLeftStructure = leftStructureChildren.OrderByDescending(child => child.TopRightCorner.x).ToList();

        //if the sorted left structures have no children
        if (sortedLeftStructure.Count == 1) 
            leftStructure = sortedLeftStructure[0];     
        else
        {
            int maxX = sortedLeftStructure[0].TopRightCorner.x;
            sortedLeftStructure = sortedLeftStructure.Where(
                children => Math.Abs(maxX - children.TopRightCorner.x) < 10).ToList();

            int index = UnityEngine.Random.Range(0,sortedLeftStructure.Count);
            leftStructure = sortedLeftStructure[index];
        }

        var possibleNeiboursInRightStructureList = rightStructureChildren.Where(
            child => GetValidYForNeibourLeftRight(
                leftStructure.TopRightCorner,
                leftStructure.BottomRightCorner,
                child.TopLeftCorner,
                child.BottomLeftCorner
                )!= -1
                ).OrderBy(child => child.BottomRightCorner.x).ToList();

        if (possibleNeiboursInRightStructureList.Count <= 0)      
            rightStructure = structure2;
        else
            rightStructure = possibleNeiboursInRightStructureList[0];
        

        //hallway position
        int y = GetValidYForNeibourLeftRight(leftStructure.TopLeftCorner, leftStructure.BottomRightCorner,
            rightStructure.TopLeftCorner, rightStructure.BottomLeftCorner);
        
        //while they cant match because they are not neibours
        while (y == -1 && sortedLeftStructure.Count > 1)
        {
            sortedLeftStructure = sortedLeftStructure.Where(
                child => child.TopLeftCorner.y != leftStructure.TopLeftCorner.y).ToList();
            leftStructure = sortedLeftStructure[0];

            //re calculate y
            y = GetValidYForNeibourLeftRight(leftStructure.TopLeftCorner, leftStructure.BottomRightCorner,
            rightStructure.TopLeftCorner, rightStructure.BottomLeftCorner);
        }

        BottomLeftCorner = new Vector2Int(leftStructure.BottomRightCorner.x, y);
        TopRightCorner = new Vector2Int(rightStructure.TopLeftCorner.x, y + this.hallwayWidth);
    }

    int GetValidYForNeibourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        //left structure is bigger on y
        if (rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.hallwayWidth)
                ).y;
        }
        //left structure is smaller on y
        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.hallwayWidth)
                ).y;
        }
        //right structure is bigger on y
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceFromWall)
                ).y;
        }
        //right structure is smaller on y
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.hallwayWidth)
                ).y;
        }
        return -1;
    }

    void ProcessRoomInRelationUpOrDown(Node structure1, Node structure2)
    {
        Node bottomStructure = null;
        List<Node> structureBottomChildren = StructureHelper.ExtractLowestLeafes(structure1);
        Node TopStructure = null;
        List<Node> structureTopChildren = StructureHelper.ExtractLowestLeafes(structure2);

        var sortedBottomStructure = structureBottomChildren.OrderByDescending(child => child.TopRightCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottomChildren[0];
        }
        else
        {
            //will only choose the room closest to the y value
            int maxY = sortedBottomStructure[0].TopLeftCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.TopLeftCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        var possibleNeighboursInTopStructure = structureTopChildren.Where(
            child => GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                child.BottomLeftCorner,
                child.BottomRightCorner)
            != -1).OrderBy(child => child.BottomRightCorner.y).ToList();

        if (possibleNeighboursInTopStructure.Count == 0)
        {
            TopStructure = structure2;
        }
        else
        {
            TopStructure = possibleNeighboursInTopStructure[0];
        }
        int x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                TopStructure.BottomLeftCorner,
                TopStructure.BottomRightCorner);

        while (x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(
                child => child.TopLeftCorner.x != TopStructure.TopLeftCorner.x).ToList();

            x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                TopStructure.BottomLeftCorner,
                TopStructure.BottomRightCorner);
        }
        BottomLeftCorner = new Vector2Int(x, bottomStructure.TopLeftCorner.y);
        TopRightCorner = new Vector2Int(x + this.hallwayWidth, TopStructure.BottomLeftCorner.y);
    }

    private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft,
        Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall,0),
                bottomNodeRight - new Vector2Int(this.hallwayWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
               topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
               topNodeRight - new Vector2Int(this.hallwayWidth + modifierDistanceFromWall, 0)
               ).x;
        }
        if (bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
               bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
               topNodeRight - new Vector2Int(this.hallwayWidth + modifierDistanceFromWall, 0)
               ).x;
        }
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
               topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
               bottomNodeRight - new Vector2Int(this.hallwayWidth + modifierDistanceFromWall, 0)
               ).x;
        }
        return -1;
    }

    RelativePosition CheckPosStructures()
    {
        Vector2 middlePoint1Temp = ((Vector2)structure1.TopRightCorner + structure1.BottomLeftCorner) / 2;
        Vector2 middlePoint2Temp = ((Vector2)structure2.TopRightCorner + structure2.BottomLeftCorner) / 2;

        float angle = CalculateAngle(middlePoint1Temp, middlePoint2Temp);
        if ((angle < 45 && angle >= 0) || (angle > -45 && angle < 0))      
            return RelativePosition.Right;
        
        else if (angle > 45 && angle < 135)       
            return RelativePosition.Up;
        
        else if (angle > -135 && angle < -45)     
            return RelativePosition.Down;
        
        else       
            return RelativePosition.Left;
        
    }

    float CalculateAngle(Vector2 middlePoint1Temp, Vector2 middlePoint2Temp)
    {
        return Mathf.Atan2(
            middlePoint2Temp.y - middlePoint1Temp.y,
            middlePoint2Temp.x - middlePoint1Temp.x) * Mathf.Rad2Deg;
    }
}