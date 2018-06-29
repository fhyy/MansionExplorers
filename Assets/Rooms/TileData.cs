#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;

[System.Serializable]
public class TileData
{
    public Coordinate tileCoordinate = new Coordinate(0, 0, 0);
    public DoorData[] doors = { };

    public TileData(int x, int y, int z)
    {
        tileCoordinate = new Coordinate(x, y, z);
    }

    public TileData(TileData tileData)
    {
        this.tileCoordinate = new Coordinate(tileData.tileCoordinate);
        this.doors = (DoorData[]) tileData.doors.Clone();
    }
}