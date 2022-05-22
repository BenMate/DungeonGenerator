using UnityEngine;
using System.Collections.Generic;
using System.Drawing;

public class BSPNode
{
    //children dimensions
    public int x, y, z, width, length, height, scale;
    public int minNodeSize = 50;

    public int roomW;
    public int roomH;

    public int extraHallwayWidth;
    public int extraHallwayHeight;

    //public node data
    public BSPNode leftNode;
    public BSPNode rightNode;

    //public room data
    public bool hasRoom = false;

    public Rectangle room;

    public GameObject floor;

    List<Rectangle> hallways = new List<Rectangle>(); //hallways


    //default Constructor
    public BSPNode(GameObject a_floor, int a_x, int a_y, int a_width, int a_Length, int a_height, int a_minNodeSize, int a_roomOffSetW, int a_roomOffSetH, int a_hallWayWidth, int a_hallwayHeight)
    {
        //prefab
        floor = a_floor;

        x = a_x;
        y = a_y;

        //dungeon values
        width = a_width;
        length = a_Length;
        height = a_height;

        //min node size
        minNodeSize = a_minNodeSize;

        //room values
        roomW = a_roomOffSetW;
        roomH = a_roomOffSetH;
        hasRoom = false;

        //hallway values
        extraHallwayHeight = a_hallwayHeight;
        extraHallwayWidth = a_hallWayWidth;  
    }

    public bool BSPSplit()
    {

        
        //if node already has children skip over
        if (leftNode != null || rightNode != null)
            return false;

        //split either vertically or horizontally
        bool horizontal = Random.Range(0.0f, 1.0f) < 0.5f;

        //if the room is wider than it is tall, split vert instead 
        if (width > length && length / width >= 0.5f)
            horizontal = false;

        else if (length > width && width / length >= 0.5f)
            horizontal = true;

        //determine the max height or width
        int max = (horizontal ? length : width) - minNodeSize;

        //if its to small, dont split
        if (max <= minNodeSize)
            return false;

        //generate a new dimension for the split
        int split = (int)Random.Range(minNodeSize, max);

        //split
        if (horizontal)
        {
            leftNode = new BSPNode(floor, x, y, width, split, height, minNodeSize, roomW, roomH, extraHallwayWidth, extraHallwayHeight);
            rightNode = new BSPNode(floor, x, y + split, width, length - split, height, minNodeSize, roomW, roomH, extraHallwayWidth, extraHallwayHeight);
        }
        else //vertical
        {
            leftNode = new BSPNode(floor, x, y, split, length, height, minNodeSize, roomW, roomH, extraHallwayWidth, extraHallwayHeight);
            rightNode = new BSPNode(floor, x + split, y, width - split, length, height, minNodeSize, roomW, roomH, extraHallwayWidth, extraHallwayHeight);
        }

        //return true if split happened   
        return true;
    }

    public void GenerateRoom()
    {
        if (leftNode != null || rightNode != null)
        {
            //Creates Lft rooms
            if (leftNode != null)
                leftNode.GenerateRoom();

            //Creates Right rooms
            if (rightNode != null)
                rightNode.GenerateRoom();

            //Generates Hallways
            if (leftNode != null || rightNode != null)
                CreateHalls(leftNode.GetRoom(), rightNode.GetRoom());

            //generate walls


            hasRoom = false;
        }
        else
        {         
            Vector2 roomSize = new Vector2(width - roomW, length - roomH); //actual size of room 
            Vector2 newRoomPos = new Vector2(Random.Range(1, width - roomSize.x - 1), Random.Range(3, length - roomSize.y - 1));

            room = new Rectangle((int)(x + newRoomPos.x), (int)(y + newRoomPos.y), (int)roomSize.x, (int)roomSize.y);

            hasRoom = false;
        }
            
        //generate floor prefab
        floor = GameObject.Instantiate(floor);
        //position
        floor.transform.position = new Vector3(room.X + (room.Width * 0.5f), height, room.Y + (room.Height * 0.5f));
        //scale
        floor.transform.localScale = new Vector3(room.Width, 1.0f, room.Height);
        //colour
        floor.GetComponent<Renderer>().material.color = UnityEngine.Color.gray;
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

            else if (Random.Range(0.0f, 1.0f) > 0.5)//50% to returrn left
                return lRoom;

            else
                return rRoom;
        }
        return room;

    }

    public void CreateHalls(Rectangle leftRoom, Rectangle rightRoom)
    {
        //Creates a hall in between the 2 rooms

        //create a new list
        hallways = new List<Rectangle>();

        //get 2 random points from each room

        Vector2 point1 = new Vector2(Random.Range(
            leftRoom.Left + 1,
            leftRoom.Right - 2), Random.Range(
            leftRoom.Top + 1,
            leftRoom.Bottom - 2));

        Vector2 point2 = new Vector2(Random.Range(
            rightRoom.Left + 1,
            rightRoom.Right - 2), Random.Range(
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

        SpawnHallways();

    }


    public void SpawnHallways()
    {
        //Create hallways
        for (int i = 0; i < hallways.Count; i++)
        {
            floor = GameObject.CreatePrimitive(PrimitiveType.Cube);

            //position
            floor.transform.position = new Vector3(hallways[i].X + (hallways[i].Width * 0.5f), height, hallways[i].Y + (hallways[i].Height * 0.5f));

            //scale extra
            int extraWidth = (hallways[i].Width > hallways[i].Height ? 0 : extraHallwayHeight); //if there is less width, scale more width
            int extraHeight = (hallways[i].Height > hallways[i].Width ? 0 : extraHallwayWidth); // if there is less height, scale more height

            //scale
            floor.transform.localScale = new Vector3(hallways[i].Width + extraWidth, 1.0f, hallways[i].Height + extraHeight);

            //colour
            floor.GetComponent<Renderer>().material.color = UnityEngine.Color.white; //temp colour
        }
    }




}