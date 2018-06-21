using UnityEngine;
using System.Collections;

[System.Serializable]
public class Coordinate
{
    public int x;
    public int y;
    public int z;

    public Coordinate(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Coordinate operator +(Coordinate a, Orientation o)
    {
        switch (o)
        {
            case Orientation.EAST:
                return new Coordinate(a.x+1,a.y,a.z);
            case Orientation.SOUTH:
                return new Coordinate(a.x, a.y, a.z-1);
            case Orientation.WEST:
                return new Coordinate(a.x - 1, a.y, a.z);
            default:
                return new Coordinate(a.x, a.y, a.z+1);
        }
    }
    public static Coordinate operator+ (Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static Coordinate operator- (Coordinate a, Coordinate b)
    {
        return new Coordinate(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static Coordinate operator- (Coordinate a)
    {
        return new Coordinate(-a.x, -a.y, -a.z);
    }

    public string getCoordinateString()
    {
        return "|x:" + x + "| |y:" + y + "| |z:" + z +"|";
    }
}
