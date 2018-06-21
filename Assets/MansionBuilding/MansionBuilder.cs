using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MansionBuilder : MonoBehaviour {

    private MansionDataHandler mansionDataHandler = new MansionDataHandler(255, 4, 255);
    public GameObject[] spawnableRooms = { };

    public void placeRoom(Coordinate coordinate)
    {
        Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>");
        if (!mansionDataHandler.isRoomTileOccupied(coordinate))
        {
            GameObject roomObject = getNextFittingRoom(coordinate);
            if (roomObject != null)
            {
                RoomDataHolder roomData = roomObject.GetComponent<RoomDataHolder>();

                Debug.Log("----------Fitting room data-----------");
                Debug.Log("Occupied tiles:");
                foreach(Coordinate occupiedTile in roomData.occupiedTiles)
                {
                    Debug.Log(occupiedTile.getCoordinateString());
                }
                Debug.Log("Room position: " + roomData.getWorldPosition().getCoordinateString());
                Debug.Log("==================\n");

                mansionDataHandler.addRoomTiles(roomData);
                buildRoomObject(roomObject, roomData);
            }
            else
            {
                Debug.LogError("Could not place room on: " + coordinate.getCoordinateString());
            }
        }
        else
        {
            Debug.Log("Room tile occupied: " + coordinate.getCoordinateString());
        }
        Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<\n\n");
    }

    public GameObject getNextFittingRoom(Coordinate baseTile)
    {
        //List<UnityEngine.Object> roomPrefabs = getAllRoomPrefabs();
        GameObject[] checkableSpawnableRooms = spawnableRooms;
        while(checkableSpawnableRooms.Length > 0) {
            GameObject roomObject = CommonOperations.getRandomItemFromList<GameObject>(checkableSpawnableRooms);
            Debug.Log("Trying to fit a room: " + roomObject.name);

            RoomDataHolder roomData = roomObject.GetComponent(typeof(RoomDataHolder)) as RoomDataHolder;
            if (roomData != null)
            {
                RoomFitTransform roomFitTransform = checkIfRoomFits(baseTile, roomData);
                if (roomFitTransform != null)
                {
                    Debug.Log("RoomFitTransform: " + roomFitTransform.baseLocation.getCoordinateString());
                    roomData.setWorldPosition(baseTile + roomFitTransform.baseLocation);
                    roomData.setOrientation(roomFitTransform.orientation);
                    RoomDataHolder roomData2 = roomObject.GetComponent(typeof(RoomDataHolder)) as RoomDataHolder;

                    Debug.Log("Roomdata1: " + roomData.getWorldPosition().getCoordinateString());
                    Debug.Log("Roomdata1 set to: " + roomFitTransform.baseLocation.getCoordinateString());
                    Debug.Log("Roomdata2: " + roomData2.getWorldPosition().getCoordinateString());

                    return roomObject;
                }
                else
                {
                    Debug.Log("Room did not fit!");
                }
            }

            // Remove checked GameObject from list
            GameObject[] newCheckableSpawnableRooms = new GameObject[checkableSpawnableRooms.Length - 1];
            int index = 0;
            for (int i = 0; i < checkableSpawnableRooms.Length && index < newCheckableSpawnableRooms.Length; ++i)
            {
                if (checkableSpawnableRooms[i] != roomObject)
                    newCheckableSpawnableRooms[index++] = checkableSpawnableRooms[i];
            }
            checkableSpawnableRooms = newCheckableSpawnableRooms.Clone() as GameObject[];
        }
        return null;
    }

    private RoomFitTransform checkIfRoomFits(Coordinate baseTile, RoomDataHolder roomData)
    {
        RoomFitTransform resultingTransformation = null;

        // Test each orientation in a random order
        Orientation[] checkableOrientations = { Orientation.NORTH, Orientation.EAST, Orientation.SOUTH, Orientation.WEST };
        while(checkableOrientations.Length > 0)
        { 
            // Test random orientation
            Orientation orientationToCheck = CommonOperations.getRandomItemFromList<Orientation>(checkableOrientations);

            // Null if it didn't fit with the orientation, otherwise success!
            resultingTransformation = checkIfRoomFits(baseTile, roomData, orientationToCheck);
            if(resultingTransformation != null)
            {
                break;
            }

            // Remove checked orientation from list
            Orientation[] newCheckableOrientations = new Orientation[checkableOrientations.Length-1];
            int index = 0;
            for(int i = 0; i < checkableOrientations.Length && index < newCheckableOrientations.Length; ++i)
            {
                if (checkableOrientations[i] != orientationToCheck)
                    newCheckableOrientations[index++] = checkableOrientations[i];
            }
            checkableOrientations = newCheckableOrientations.Clone() as Orientation[];

            // No orientation could fit the tile
            if (checkableOrientations.Length == 0)
            {
                break;
            }
        }

        // Pass any result to the caller!
        return resultingTransformation;
    }

    private RoomFitTransform checkIfRoomFits(Coordinate baseTile, RoomDataHolder roomData, Orientation orientation)
    {
        // Try moving the room so that any occupied tile is positioned on baseTile
        Coordinate[] testTiles = roomData.occupiedTiles;
        while(testTiles.Length > 0) {
            Coordinate newRoomBaseTile = CommonOperations.getRandomItemFromList<Coordinate>(testTiles);
            Debug.Log("Trying to place room tile with base on: " + baseTile.getCoordinateString());
            bool collisionDetected = false;
            // Room may be rotated, transform coordinates accordingly
            Coordinate transformed_newRoomBaseTile = CommonOperations.getTransformedCoordinate(newRoomBaseTile, orientation);
            foreach (Coordinate roomOccupationTile in roomData.occupiedTiles)
            {
                // Room may be rotated, transform coordinates accordingly
                Coordinate transformed_roomOccupationTile = CommonOperations.getTransformedCoordinate(roomOccupationTile, orientation);

                // Get the coordinate of the tiles in the rotated and shifted room
                Coordinate checkCoordinate = baseTile + (transformed_roomOccupationTile - transformed_newRoomBaseTile);
                Debug.Log("CheckTile: " + checkCoordinate.getCoordinateString());
                Debug.Log("(transformed_roomOccupationTile - transformed_newRoomBaseTile) = " + (transformed_roomOccupationTile - transformed_newRoomBaseTile).getCoordinateString());

                // Check if this tile is occupied
                if (mansionDataHandler.isRoomTileOccupied(checkCoordinate))
                {
                    collisionDetected = true;
                    break;
                }
                else
                {
                    Debug.Log("Tile was occupied: " + checkCoordinate.getCoordinateString());
                }
            }
            // Check if all doors align with existing doors
            if(!checkIfDoorsAlign(baseTile, roomData, transformed_newRoomBaseTile, orientation))
            {
                collisionDetected = true;
            }

            // No collisions with the transformation above. Use that transformation to later place the room!
            if (!collisionDetected)
            {
                Debug.Log("Room okay, transform like this: " + (-transformed_newRoomBaseTile).getCoordinateString());
                return new RoomFitTransform(-transformed_newRoomBaseTile, orientation);
            }

            // Remove checked coordinate from list
            Coordinate[] newTestTiles = new Coordinate[testTiles.Length - 1];
            int index = 0;
            for (int i = 0; i < testTiles.Length && index < newTestTiles.Length; ++i)
            {
                if (testTiles[i] != newRoomBaseTile)
                    newTestTiles[index++] = testTiles[i];
            }
            testTiles = newTestTiles.Clone() as Coordinate[];
        }
        // No transformation of this room fit on this tile
        return null;
    }

    private bool checkIfDoorsAlign(Coordinate baseTile, RoomDataHolder roomData, Coordinate transformed_roomShiftAmount, Orientation orientationOfTheRoom)
    {
        bool misalignmentDetected = false;
        foreach(DoorData door in roomData.doors)
        {
            Coordinate transformed_doorLocation = CommonOperations.getTransformedCoordinate(door.position, orientationOfTheRoom);
            Coordinate neightbourTileCoordinate = getCoordinateOfTileBehindDoor(baseTile, door, transformed_roomShiftAmount, orientationOfTheRoom);
            if(neightbourTileCoordinate == null)
            {
                continue;
            }
            //TODO!!! <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        }
        return !misalignmentDetected;
    }

    private Coordinate getCoordinateOfTileBehindDoor(Coordinate baseTile, DoorData door, Coordinate transformed_roomShiftAmount, Orientation orientationOfTheRoom)
    {
        Coordinate transformed_doorTile_offset = CommonOperations.getTransformedCoordinate(door.position, orientationOfTheRoom);
        Coordinate doorTileLocation = baseTile + (transformed_doorTile_offset + transformed_roomShiftAmount);
        Orientation transformed_doorOrientation = CommonOperations.getTransformedOrientation(door.wallDirection, orientationOfTheRoom);
        return doorTileLocation + transformed_doorOrientation;
    }

    private GameObject buildRoomObject(GameObject roomPrefab, RoomDataHolder roomData)
    {
        setRoomLocation(Instantiate(roomPrefab), roomData.getWorldPosition(), roomData.getOrientation());
        return null;
    }

    private Vector3 getRealPosition(Coordinate matrixIndexes)
    {
        return new Vector3(matrixIndexes.x * 8, matrixIndexes.y * 4, matrixIndexes.z * 8);
    }

    private Vector3 getRealRotation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.EAST:
                return new Vector3(270, 90, 0);
            case Orientation.SOUTH:
                return new Vector3(270, 180, 0);
            case Orientation.WEST:
                return new Vector3(270, 270, 0);
            default:
                return new Vector3(270, 0, 0);
        }
    }

    public void setRoomLocation(GameObject room, Coordinate coordinate, Orientation orientation)
    {
        Debug.Log("Setting room location to: " + coordinate.getCoordinateString());
        Transform transform = room.transform;
        if (transform != null)
        {
            room.transform.parent = this.gameObject.transform;

            Vector3 pos = getRealPosition(coordinate);
            Vector3 rot = getRealRotation(orientation);

            transform.position = pos;
            transform.eulerAngles = rot;
        }
    }

    // Use this for initialization
    void Start () {
        int width = 8;
        int depth = 8;
        for(int i = 10; i > 0; --i){
            placeRoom(new Coordinate((int)(Random.value*width), 1, (int)(Random.value * depth)));
        } 
        //placeRoom(new Coordinate(1,0,1));
        //placeRoom(new Coordinate(1, 0, 0));
        //placeRoom(new Coordinate(0, 0, 1));
        //placeRoom(new Coordinate(0, 0, 0));
    }

    // Update is called once per frame
    int updateCounter = 0;
    int roomsSpawned = 0;
    void Update () {
        if (roomsSpawned > 300)
            return;
        ++updateCounter;
        int width = 10;
        int depth = 10;
        if (updateCounter == 30)
        {
            updateCounter = 0;
            placeRoom(new Coordinate((int)(Random.value * width), 1, (int)(Random.value * depth)));
            ++roomsSpawned;
        }
	}

    private class RoomFitTransform
    {
        public Orientation orientation;
        public Coordinate baseLocation;

        public RoomFitTransform(Coordinate baseLocation, Orientation orientation)
        {
            this.baseLocation = baseLocation;
            this.orientation = orientation;
        }
    }
}
