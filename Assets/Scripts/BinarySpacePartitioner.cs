using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class BinarySpacePartitioner
{
    RoomNode rootnode;
    public RoomNode Rootnode { get => rootnode; }

    public BinarySpacePartitioner(int dungeonWidth, int dungeonLength)
    {
        this.rootnode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(dungeonWidth, dungeonLength), null, 0);
    }

    public List<RoomNode> PreparenodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();  
        graph.Enqueue(this.rootnode);
        listToReturn.Add(this.rootnode);
        int iterations = 0;
        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            RoomNode currentNode = graph.Dequeue();
            if (currentNode.Width >= roomWidthMin*2 || currentNode.Length >= roomLengthMin * 2)
            {
                SplitTheSpace(currentNode, listToReturn, roomLengthMin, roomWidthMin, graph);
            }
        }
        return listToReturn;
    }

    void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomLengthMin, int roomWidthMin, Queue<RoomNode> graph)
    {

        Line line = GetLineDividingSpace(
            currentNode.BottomLeftCorner, 
            currentNode.TopRightCorner, 
            roomWidthMin, 
            roomLengthMin);

        RoomNode node1, node2;

        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottomLeftCorner,
                new Vector2Int(currentNode.TopRightCorner.x,
                line.Coodinates.y),
                currentNode,
                currentNode.TreeLayerIndex + 1);

            node2 = new RoomNode(new Vector2Int(currentNode.BottomLeftCorner.x, line.Coodinates.y),
               currentNode.TopRightCorner,
               currentNode,
               currentNode.TreeLayerIndex + 1);
        }
        else
        {
            node1 = new RoomNode(currentNode.BottomLeftCorner,
                new Vector2Int(line.Coodinates.x, currentNode.TopRightCorner.y),
                currentNode,
                currentNode.TreeLayerIndex + 1);

            node2 = new RoomNode(new Vector2Int(line.Coodinates.x, currentNode.BottomLeftCorner.y),
               currentNode.TopRightCorner,
               currentNode,
               currentNode.TreeLayerIndex + 1);
        }

        AddnewNodeToCollections(listToReturn, graph, node1);
        AddnewNodeToCollections(listToReturn, graph, node2);
    }

    void AddnewNodeToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    Line GetLineDividingSpace(Vector2Int bottomLeftCorner, Vector2Int topRightCorner, int roomWidthMin, int roomLengthMin)
    {
        Orientation orientation;
        bool lengthStatus = (topRightCorner.y - bottomLeftCorner.y) >= 2 * roomLengthMin;
        bool widthStatus = (topRightCorner.x - bottomLeftCorner.x) >= 2 * roomWidthMin;


        if (lengthStatus && widthStatus)       
            orientation = (Orientation)(Random.Range(0, 2));
        
        else if (widthStatus)     
            orientation = Orientation.Vertical;

        else     
            orientation = Orientation.Horizontal; ;
        
        return new Line(orientation, GetCoordinatesForOrientation(
            orientation,
            bottomLeftCorner,
            topRightCorner, 
            roomWidthMin, 
            roomLengthMin));
    }

    Vector2Int GetCoordinatesForOrientation(Orientation orientation, Vector2Int bottomLeftCorner, Vector2Int topRightCorner, int roomWidthMin, int roomLengthMin)
    {
        Vector2Int coordinates = Vector2Int.zero;

        if (orientation == Orientation.Horizontal)
        {
            //we are creating a point where the min is the distance is from the bottom left and maximum value is the top right corner
            coordinates = new Vector2Int(
                0, 
                Random.Range(
                (bottomLeftCorner.y + roomLengthMin), 
                (topRightCorner.y - roomLengthMin)));
        }
        else
        {
            coordinates = new Vector2Int(  
                Random.Range(
                (bottomLeftCorner.x + roomWidthMin),
                (topRightCorner.x - roomWidthMin)),
                0);
        }
        return coordinates;
    }
}