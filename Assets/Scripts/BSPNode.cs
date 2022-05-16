using UnityEngine;
using System.Collections.Generic;

public class BSPNode
{
    //children dimensions
    public int x, y, z, width, height, scale;

    public BSPNode leftNode;
    public BSPNode rightNode;

    public bool hasRoom = false;

    public Vector2 roomSize;
    public Vector2 roomPos;

    GameObject box;
 
    //default Constructor
    public BSPNode(int a_x, int a_y, int a_width, int a_height, int a_scale)
    {
        //x,y,z position
        x = a_x;
        y = a_y;
       
        
        width = a_width;
        height = a_height;
        scale = a_scale;

        hasRoom = false;

        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.position = new Vector3(x + (width * scale), y + (height * scale), 0.0f); //might not work ;c
        box.transform.localScale = new Vector3(width, height, 1.0f);

        //creates a random colour
        Color randColor = new Color(Random.Range(0, 1.0f),
            Random.Range(0, 1.0f), Random.Range(0, 1.0f), 1.0f);
        box.GetComponent<Renderer>().material.color = randColor;
        
    }

    public bool BSPNode()
    {
        //if leaf already has children skip over
        if (leftNode != null || rightNode != null)
            return false;

        //split either vertically or horizontally
        bool horizontal = Random.Range(0.0f, 1.0f) < 0.5f;

        //if the room is wider than it is tall, split vert instead 
        if (width > height && height / width >= 0.5f)
            horizontal = false;

        else if (height > width && width / height >= 0.5f)
            horizontal = true;

        //determine the max size of the new leaf
        int max = (horizontal ? height : width) - min

            https://www.reddit.com/r/Unity3D/comments/2kjh1p/bsp_and_procedural_generation/clmaagr/

    }

}