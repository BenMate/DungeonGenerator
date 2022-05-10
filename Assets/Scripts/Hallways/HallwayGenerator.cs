using System;
using System.Collections.Generic;
using System.Linq;

public class HallwayGenerator
{
    public List<Node> CreateHallway(List<RoomNode> allNodesCollection, int hallwayWidth)
    {
       List<Node> hallwayList = new List<Node>();
       Queue<RoomNode> structuresToCheck = new Queue<RoomNode>(allNodesCollection.OrderByDescending(Node => Node.TreeLayerIndex).ToList());
       while (structuresToCheck.Count > 0)
       {
            var node = structuresToCheck.Dequeue();
            if (node.ChildrenNodeList.Count == 0)
            {
                continue;
            }

            HallwayNode hallway = new HallwayNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], hallwayWidth);
            hallwayList.Add(hallway);
       }
        return hallwayList;
    }
}