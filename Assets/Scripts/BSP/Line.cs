using UnityEngine;

public class Line
{
    Orientation orientation;
    Vector2Int coodinates;

    public Line(Orientation orientation, Vector2Int cordinates)
    {
        this.orientation = orientation;
        this.coodinates = cordinates;
    }

    public Orientation Orientation { get => orientation; set => orientation = value; }
    public Vector2Int Coodinates { get => coodinates; set => coodinates = value; }
}

public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}