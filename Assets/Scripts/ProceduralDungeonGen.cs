using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProceduralDungeonGen : MonoBehaviour
{

    public int width = 400;
    public int height = 400;

    BSPNode root;
    public const int max_node_size = 50;
    List<BSPNode> nodes = new List<BSPNode>();

    // Start is called before the first frame update
    void Start()
    {
        CreateRooms();


    }

    void CreateRooms()
    {
        root = new BSPNode(0, 0, width, height);
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
                    if (nodes[i].width > max_node_size || nodes[i].height > max_node_size)
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

        root.GenerateRoom();
    }




}









