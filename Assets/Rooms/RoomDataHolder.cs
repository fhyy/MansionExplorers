using UnityEngine;
using System.Collections;

public class RoomDataHolder : MonoBehaviour {

    public enum Storey
    {
        ENTRANCE = 0,
        MINUS_ONE = -1,
        PLUS_ONE = 1,
        ROOF = 2
    }

    private Coordinate worldPosition = null;
    private Orientation worldOrientation = Orientation.NORTH;
    public Coordinate[] occupiedTiles = { new Coordinate(0,0,0) };
    public Storey[] allowedSpawnStoreys = { Storey.ENTRANCE };
    public DoorData[] doors = {  };
    public SpawnableObject[] spawnableObjects = { };

    public void setWorldPosition(Coordinate coordinate)
    {
        worldPosition = coordinate;
    }
    public Coordinate getWorldPosition()
    {
        return worldPosition;
    }

    public void setOrientation(Orientation orientation)
    {
        worldOrientation = orientation;
    }
    public Orientation getOrientation()
    {
        return worldOrientation;
    }
}