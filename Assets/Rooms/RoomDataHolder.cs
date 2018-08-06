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
    public TileData[] occupiedTiles = { new TileData(0, 0, 0) };
    public GameObject[] spawnableDoors = { };
    public Storey[] allowedSpawnStoreys = { Storey.ENTRANCE };
    public SpawnableObject[] spawnableObjects = { };
    public RoomColliderHandler colliderHandler = null;

    public OnEnterCallback onEnterCallback = null;

    public RoomDataHolder(RoomDataHolder roomData)
    {
        this.worldPosition = new Coordinate(roomData.worldPosition);
        this.worldOrientation = roomData.worldOrientation;
        this.occupiedTiles = (TileData[]) roomData.occupiedTiles.Clone();
        this.allowedSpawnStoreys = (Storey[])roomData.allowedSpawnStoreys;
        this.spawnableObjects = (SpawnableObject[])roomData.spawnableObjects.Clone();
        this.colliderHandler = (roomData.colliderHandler);
    }

    public void registerCallback(RoomColliderHandler.Callback callback)
    {
        this.colliderHandler.registerCallback(callback);
    }

    public void removeCallback()
    {
        this.colliderHandler.removeCallback();
    }

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

    public GameObject getRandomSpawnableDoor()
    {
        GameObject doorObject = CommonOperations.getRandomItemFromList<GameObject>(spawnableDoors);
        //TODO: Check if door fulfills door constraints?
        return doorObject;
    }


    public abstract class OnEnterCallback
    {
        public abstract void onEnter();
    }
}