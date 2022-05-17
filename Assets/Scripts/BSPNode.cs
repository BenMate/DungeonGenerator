using UnityEngine;
using System.Collections.Generic;
using System.Drawing;

public class BSPNode
{
    //children dimensions
    public int x, y, z, width, height, scale;
    public int minNodeSize = 50;

    //public node data
    public BSPNode leftNode;
    public BSPNode rightNode;

    //public room data
    public bool hasRoom = false;

    public Rectangle room;

    GameObject box;

    List<Rectangle> hallways = new List<Rectangle>(); //hallways


    //default Constructor
    public BSPNode(int a_x, int a_y, int a_width, int a_height)
    {
        //x,y position
        x = a_x;
        y = a_y;
       
        width = a_width;
        height = a_height;
       
        hasRoom = false;

        //spawn objects
        box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.position = new Vector3(x + (width * 0.5f), y + (height * 0.5f), 0.0f);
        box.transform.localScale = new Vector3(width, height, 1.0f);

        //random Colour 
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
            //checks left node for children, if there is one create a room
            if (leftNode != null)
                leftNode.GenerateRoom();

            //check right node for children, if there is one create a room
            if (rightNode != null)
                rightNode.GenerateRoom();

            //if there are children on both left and right, create a hallway between both rooms
            if (leftNode != null && rightNode != null)
                CreateHalls(leftNode.GetRoom(), rightNode.GetRoom());

            hasRoom = false;
        }
        else
        {
            Vector2 roomSize;
            Vector2 roomPos;
            roomSize = new Vector2(Random.Range(3, width - 2), Random.Range(3, height - 2));
            roomPos = new Vector2(Random.Range(2, width - roomSize.x - 2), Random.Range(2, height - roomSize.y - 2));
            room = new Rectangle((int)(x + roomPos.x), (int)(y + roomPos.y), (int)roomSize.x, (int)roomSize.y);
      
            hasRoom = false;
        }

    }

    public Rectangle GetRoom()
    {
        // iterate all the way down these nodes to find a room, if one exists.
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
                Debug.Log("should have returned null");

            else if (rRoom == null)
                return lRoom;

            else if (lRoom == null)
                return rRoom;

            //50% to returrn left
            else if (Random.Range(0.0f, 1.0f) > 0.5)
                return lRoom;

            else
                return rRoom;
        }
        return room;
 
    }


    public void CreateHalls(Rectangle leftRoom, Rectangle rightRoom)
    {
        //we now Create a hall in between the 2 rooms

        //create a new list
        hallways = new List<Rectangle>();

        //get 2 random points from each room
        
        Vector2 point1 = new Vector2(Random.Range(
            leftRoom.Left + 1,
            leftRoom.Right - 2),Random.Range(
            leftRoom.Top + 1, 
            leftRoom.Bottom - 2));

        Vector2 point2 = new Vector2(Random.Range(
            rightRoom.Left + 1,
            rightRoom.Right - 2),Random.Range(
            rightRoom.Top + 1,
            rightRoom.Bottom - 2));

        int width = (int)(point2.x - point1.x);
        int height = (int)(point2.y - point1.y);

        if (width < 0)
        {
            if (height < 0)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5) //50%
                {
                    
                    hallways.Add(new Rectangle((int)point2.x, (int)point1.y, Mathf.Abs(width), 1));
                    hallways.Add(new Rectangle((int)point2.x, (int)point2.y, 1, Mathf.Abs(height)));
                }
                else
                {
                    hallways.Add(new Rectangle((int)point2.x, (int)point2.y, Mathf.Abs(width), 1));
                    hallways.Add(new Rectangle((int)point1.x, (int)point2.y, 1, Mathf.Abs(height)));
                }
            }
            else if (height > 0)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5) //50%
                {
                    hallways.Add(new Rectangle((int)point2.x, (int)point1.y, Mathf.Abs(width), 1));
                    hallways.Add(new Rectangle((int)point2.x, (int)point1.y, 1, Mathf.Abs(height)));
                }
                else
                {
                    hallways.Add(new Rectangle((int)point2.x, (int)point2.y, Mathf.Abs(width), 1));
                    hallways.Add(new Rectangle((int)point1.x, (int)point1.y, 1, Mathf.Abs(height)));
                }
            }
            else // if height == 0
            {
                hallways.Add(new Rectangle((int)point2.x, (int)point2.y, Mathf.Abs(width), 1));
            }
        }
        else if (width > 0)
        {
            if (height < 0)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    hallways.Add(new Rectangle((int)point1.x, (int)point2.y, Mathf.Abs(width), 1));
                    hallways.Add(new Rectangle((int)point1.x, (int)point2.y, 1, Mathf.Abs(height)));
                }
                else
                {
                    hallways.Add(new Rectangle((int)point1.x, (int)point1.y, Mathf.Abs(width), 1));
                    hallways.Add(new Rectangle((int)point2.x, (int)point2.y, 1, Mathf.Abs(height)));
                }
            }
            else if (height > 0)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    hallways.Add(new Rectangle((int)point1.x, (int)point1.y, Mathf.Abs(width), 1));
                    hallways.Add(new Rectangle((int)point2.x, (int)point1.y, 1, Mathf.Abs(height)));
                }
                else
                {
                    hallways.Add(new Rectangle((int)point1.x, (int)point2.y, Mathf.Abs(width), 1));
                    hallways.Add(new Rectangle((int)point1.x, (int)point1.y, 1, Mathf.Abs(height)));
                }
            }
            else // height == 0
            {
                hallways.Add(new Rectangle((int)point1.x, (int)point1.y, Mathf.Abs(width), 1));
            }
        }
        else // width == 0
        {
            if (height < 0)
            {
                hallways.Add(new Rectangle((int)point2.x, (int)point2.y, 1, Mathf.Abs(height)));
            }
            else if (height > 0)
            {
                hallways.Add(new Rectangle((int)point1.x, (int)point1.y, 1, Mathf.Abs(height)));
            }

        }
    }





}