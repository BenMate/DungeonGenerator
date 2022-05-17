using UnityEngine;
using System.Collections.Generic;
using System.Drawing;

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

    int minNodeSize = 50;

    Rectangle room;
 
    //default Constructor
    public BSPNode(int a_x, int a_y, int a_width, int a_height)
    {
        //x,y position
        x = a_x;
        y = a_y;
       
        width = a_width;
        height = a_height;
       
        hasRoom = false;

        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.position = new Vector3(x + (width * 0.5f), y + (height * 0.5f), 0.0f);
        box.transform.localScale = new Vector3(width, height, 1.0f);

        //creates a random colour
        UnityEngine.Color randColor = new UnityEngine.Color(Random.Range(0, 1.0f),
            Random.Range(0, 1.0f), Random.Range(0, 1.0f), 1.0f);
        box.GetComponent<Renderer>().material.color = randColor;
        
    }

    public bool BSPSplit()
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
        int max = (horizontal ? height : width) - minNodeSize;

        //if its to small, dont split
        if (max <= minNodeSize)
            return false;

        //generate a new dimension for the split
        int split = (int)Random.Range(minNodeSize, max);

        //split
        if (horizontal)
        {
            leftNode = new BSPNode(x, y, width, split);
            rightNode = new BSPNode(x, y + split, width, height - split);

        }
        else //vertical
        {
            leftNode = new BSPNode(x, y, split, height);
            rightNode = new BSPNode(x + split, y, width - split, height);
        }

        if (box != null)
        {
            GameObject.Destroy(box);
            box = null;
        }

        //return true if split happened   
        return true;
    }

    public void GenerateRoom()
    {
        if (leftNode != null || rightNode != null)
        {
            //checks left node
            if (leftNode != null)
                leftNode.GenerateRoom();

            //checks right node
            if (rightNode != null)
                rightNode.GenerateRoom();

            hasRoom = false;
        }
        else
        {
            roomSize = new Vector2(Random.Range(3, width - 2), Random.Range(3, height - 2));
            roomPos = new Vector2(Random.Range(2, width - roomSize.x - 2), Random.Range(2, height - roomSize.y - 2));

            var room = new Rectangle((int)(x + roomPos.x), (int)(y + roomPos.y), (int)roomSize.x, (int)roomSize.y);
            hasRoom = false;
        }

    }

    public Rectangle GetRoom()
    {
        //iterate through the node to find a room, if one exists

        if (room != null)
            return room;

        else
        {
            Rectangle lRoom;
            Rectangle rRoom;

            if (leftNode != null)
                lRoom = leftNode.GetRoom();

            if (rightNode != null)
                rRoom = rightNode.GetRoom();

            if (lRoom == null && rRoom == null)         
                Debug.Log("should have returned null");// return null;
                                                      
            else if (rRoom == null)
                return lRoom;

            else if (lRoom == null)
                return rRoom;

            

            

            
            

            
        }



        return room;
    }



}