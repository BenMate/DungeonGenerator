using System;
using System.Collections.Generic;

public static class StructureHelper
{
    public static List<Node> ExtractLowestLeafes(RoomNode parentNode)
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
}