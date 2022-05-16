using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class StructureHelper
{
    public static List<Node> ExtractLowestLeafes(Node parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<Node> listToReturn = new List<Node>();

        if (parentNode.ChildrenNodeList.Count == 0)     
            return new List<Node>() { parentNode };
        
        foreach (Node childNode in parentNode.ChildrenNodeList)
            nodesToCheck.Enqueue(childNode);
        
        while (nodesToCheck.Count > 0)
        {
            var currentnode = nodesToCheck.Dequeue();

            if (currentnode.ChildrenNodeList.Count == 0)         
                listToReturn.Add(currentnode);
            
            else         
                foreach (var child in currentnode.ChildrenNodeList)             
                    nodesToCheck.Enqueue(child);  
        }
        return listToReturn;
    }

    public static Vector2Int GenerateBottomLeftCornerBetween(
        Vector2Int boundryLeftPoint, Vector2Int boundryRightPoint, float pointModifier, int offSet)
    {
        int minX = boundryLeftPoint.x + offSet;
        int maxX = boundryRightPoint.x - offSet;

        int minY = boundryLeftPoint.y + offSet;
        int maxY = boundryRightPoint.y - offSet;

        return new Vector2Int(
            Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (int)(minY + (minY - minY) * pointModifier)));
    }

    public static Vector2Int GenerateTopRightCornerBetween(
        Vector2Int boundryLeftPoint, Vector2Int boundryRightPoint, float pointModifier, int offSet)
    {
        int minX = boundryLeftPoint.x + offSet;
        int maxX = boundryRightPoint.x - offSet;

        int minY = boundryLeftPoint.y + offSet;
        int maxY = boundryRightPoint.y - offSet;

        return new Vector2Int(
            Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
            Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY)
            );
    }

    public static Vector2Int CalculateMiddlePoint(Vector2Int v1, Vector2Int v2)
    {
        Vector2 sum = v1 + v2;
        Vector2 tempVector = sum / 2;
        return new Vector2Int((int)tempVector.x, (int)tempVector.y);
    }
}

public enum RelativePosition
{
    Up,
    Down,
    Left,
    Right
}