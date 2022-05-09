using UnityEngine;
public class RoomNode : Node
{

    public RoomNode(Vector2Int bottomleftCorner, Vector2Int topRightCorner, Node parentNode, int index) : base(parentNode)
    {
        this.BottomLeftCorner = bottomleftCorner;
        this.TopRightCorner = topRightCorner;
        this.BottomRightCorner = new Vector2Int(topRightCorner.x, BottomLeftCorner.y);
        this.TopLeftCorner = new Vector2Int(BottomLeftCorner.x, topRightCorner.y);
        this.TreeLayerIndex = index;
    }

    public int Width { get => (int)(TopRightCorner.x - BottomLeftCorner.x); }
    public int Length { get => (int)(TopRightCorner.y - BottomLeftCorner.y); }
}