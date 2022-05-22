using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProceduralDungeonGen : MonoBehaviour
{
    public GameObject floorObject;

    public int dungWidth = 400;
    public int dungLength = 400;
    public int dungHeight = 1;

    public int minNodesSize = 50; //min possible size of a node
    
    //need a better name - higher the number the less there is of width and height
    public int roomWidth = 10; //the higher the number the smaller the width
    public int roomHeight = 10; // the higher the number the smaler the height

    //adds extra width and height to hallways - dont add to much
    public int extraHallWayWidth = 3;
    public int extraHallWayHeight = 3;

    public const int max_node_size = 4;
    List<BSPNode> nodes = new List<BSPNode>();
 

    // Start is called before the first frame update
    void Start()
    {
        CreateDungeon();
    }

    void CreateDungeon()
    {
        BSPNode root = new BSPNode(floorObject, 1, 1, dungWidth, dungLength, dungHeight, minNodesSize, roomWidth, roomHeight, extraHallWayWidth, extraHallWayHeight);
        nodes.Add(root);

        bool didSplit = true;

        //we loop through every node in the vector.
        while (didSplit)
        {
            didSplit = false;

            for (int i = 0; i < nodes.Count; i++)
            {
                //if left or right nodes is null
                if (nodes[i].leftNode == null && nodes[i].rightNode == null)
                {
                    //if the node is to big
                    if (nodes[i].width > max_node_size || nodes[i].length > max_node_size)
                    {
                        //if this node has not already been split
                        if (nodes[i].BSPSplit())
                        {
                            //if we have did split, add child nodes
                            nodes.Add(nodes[i].leftNode);
                            nodes.Add(nodes[i].rightNode);
                       
                            didSplit = true;
                        }
                    }
                }
            }
        }
        //iterate through each leaf and create a room in each one
       root.GenerateRoom();


    }


}









