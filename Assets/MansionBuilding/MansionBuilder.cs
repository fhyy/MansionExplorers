using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MansionBuilder : MonoBehaviour {

    private MansionDataHandler mansionDataHandler = new MansionDataHandler(10, 8, 10);
    public GameObject[] spawnableRooms = { };

    public void placeRoom(Coordinate coordinate)
    {
        if (mansionDataHandler.canPlaceRoomOnTile(coordinate))
        {
            GameObject roomObject = getNextFittingRoom(coordinate);
            if (roomObject != null)
            {
                RoomDataHolder roomData = roomObject.GetComponent<RoomDataHolder>();

                GameObject builtObject = buildRoomObject(roomObject, roomData);
                RoomDataHolder builtRoomData = builtObject.GetComponent<RoomDataHolder>();

                builtRoomData.setWorldPosition(roomData.getWorldPosition());
                builtRoomData.setOrientation(roomData.getOrientation());

                builtRoomData.registerCallback(new OnRoomEnteredCallback(this, builtRoomData));
                spawnRandomObjects(builtRoomData);
                mansionDataHandler.addRoomTiles(builtRoomData);
            }
        }
    }

    public IEnumerator placeRoomsOnAllDoors(RoomDataHolder srcRoomData)
    {
        if (srcRoomData != null) {
            foreach (TileData tile in srcRoomData.occupiedTiles)
            {
                foreach (DoorData door in tile.doors)
                {
                    Coordinate tileBehindDoorCoordinate = getCoordinateOfTileBehindDoor(srcRoomData.getWorldPosition(), tile, door, srcRoomData.getOrientation());
                    placeRoom(tileBehindDoorCoordinate);

                    yield return null;
                }
            }
        }
    }

    public GameObject getNextFittingRoom(Coordinate baseTile)
    {

        RoomDataHolder checkRoom = mansionDataHandler.getRoomDataOnCoordinate(new Coordinate(0, 0, 1));

        //List<UnityEngine.Object> roomPrefabs = getAllRoomPrefabs();
        GameObject[] checkableSpawnableRooms = spawnableRooms;
        while (checkableSpawnableRooms.Length > 0)
        {
            GameObject roomObject = CommonOperations.getRandomItemFromList<GameObject>(checkableSpawnableRooms);

            RoomDataHolder roomData = roomObject.GetComponent(typeof(RoomDataHolder)) as RoomDataHolder;
            if (roomData != null)
            {
                RoomFitTransform roomFitTransform = checkIfRoomFits(baseTile, roomData);
                if (roomFitTransform != null)
                {
                    roomData.setWorldPosition(baseTile + roomFitTransform.baseLocation);
                    roomData.setOrientation(roomFitTransform.orientation);
                    RoomDataHolder roomData2 = roomObject.GetComponent(typeof(RoomDataHolder)) as RoomDataHolder;

                    return roomObject;
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
        while (checkableOrientations.Length > 0)
        {
            // Test random orientation
            Orientation orientationToCheck = CommonOperations.getRandomItemFromList<Orientation>(checkableOrientations);

            // Null if it didn't fit with the orientation, otherwise success!
            resultingTransformation = checkIfRoomFits(baseTile, roomData, orientationToCheck);
            if (resultingTransformation != null)
            {
                break;
            }

            // Remove checked orientation from list
            Orientation[] newCheckableOrientations = new Orientation[checkableOrientations.Length - 1];
            int index = 0;
            for (int i = 0; i < checkableOrientations.Length && index < newCheckableOrientations.Length; ++i)
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

    private RoomFitTransform checkIfRoomFits(Coordinate baseTileWorldLocation, RoomDataHolder roomData, Orientation orientation)
    {
        // Try moving the room so that any occupied tile is positioned on baseTile
        TileData[] testTiles = roomData.occupiedTiles;
        while (testTiles.Length > 0) {
            TileData newRoomBaseTile = CommonOperations.getRandomItemFromList<TileData>(testTiles);

            bool collisionDetected = false;
            // Room may be rotated, transform coordinates accordingly
            Coordinate transformed_newRoomBaseTile = CommonOperations.getTransformedCoordinate(newRoomBaseTile.tileCoordinate, orientation);
            foreach (TileData roomOccupationTile in roomData.occupiedTiles)
            {
                // Room may be rotated, transform coordinates accordingly
                Coordinate transformed_roomOccupationTile = CommonOperations.getTransformedCoordinate(roomOccupationTile.tileCoordinate, orientation);

                // Get the coordinate of the tiles in the rotated and shifted room
                Coordinate checkCoordinate = baseTileWorldLocation + (transformed_roomOccupationTile - transformed_newRoomBaseTile);

                // Check if room can be placed on this tile
                if (!mansionDataHandler.canPlaceRoomOnTile(checkCoordinate))
                {
                    collisionDetected = true;
                    break;
                }
            }
            // Check if all doors align with existing doors
            if (!collisionDetected && !checkIfDoorsAlign(baseTileWorldLocation, newRoomBaseTile.tileCoordinate, roomData, orientation))
            {
                collisionDetected = true;
            }

            // No collisions with the transformation above. Use that transformation to later place the room!
            if (!collisionDetected)
            {
                return new RoomFitTransform(-transformed_newRoomBaseTile, orientation);
            }

            // Remove checked coordinate from list
            TileData[] newTestTiles = new TileData[testTiles.Length - 1];
            int index = 0;
            for (int i = 0; i < testTiles.Length && index < newTestTiles.Length; ++i)
            {
                if (testTiles[i].tileCoordinate != newRoomBaseTile.tileCoordinate)
                    newTestTiles[index++] = testTiles[i];
            }
            testTiles = newTestTiles.Clone() as TileData[];
        }
        // No transformation of this room fit on this 
        return null;
    }

    private bool checkIfDoorsAlign(Coordinate baseTileWorldLocation, Coordinate baseTileCoordinateLocal, RoomDataHolder roomData, Orientation orientationOfTheRoom)
    {
        bool misalignmentDetected = false;
        foreach (TileData tile in roomData.occupiedTiles)
        {

            Coordinate transformed_tileLocation = CommonOperations.getTransformedCoordinate(tile.tileCoordinate, orientationOfTheRoom);
            Coordinate transformed_tileCoordinateLocal = CommonOperations.getTransformedCoordinate(tile.tileCoordinate - baseTileCoordinateLocal, orientationOfTheRoom);
            Coordinate world_tileLocation = baseTileWorldLocation + (transformed_tileCoordinateLocal);

            // Check doors on walls in each direction
            if (!checkIfDoorsAlign_helper(tile, world_tileLocation, orientationOfTheRoom, Orientation.NORTH)) {
                misalignmentDetected = true;
            }
            if (!checkIfDoorsAlign_helper(tile, world_tileLocation, orientationOfTheRoom, Orientation.EAST)) {
                misalignmentDetected = true;
            }
            if (!checkIfDoorsAlign_helper(tile, world_tileLocation, orientationOfTheRoom, Orientation.SOUTH)) {
                misalignmentDetected = true;
            }
            if (!checkIfDoorsAlign_helper(tile, world_tileLocation, orientationOfTheRoom, Orientation.WEST)) {
                misalignmentDetected = true;
            }
        }
        return !misalignmentDetected;
    }

    private bool checkIfDoorsAlign_helper(TileData tile, Coordinate world_tileLocation, Orientation orientationOfTheRoom, Orientation doorDirection)
    {
        bool canPlaceRoomOnNeighbourTile = mansionDataHandler.canPlaceRoomOnTile(world_tileLocation + doorDirection);
        bool hasDoorOnWall = checkIfDoorAgainstWall(tile, orientationOfTheRoom, doorDirection);
        bool neighbourRoomExist = mansionDataHandler.getTileDataOnCoordinate(world_tileLocation + doorDirection) != null;
        bool hasDoorOnNeighbourWall = checkIfDoorAgainstWall(world_tileLocation + doorDirection, CommonOperations.getInvertedOrientation(doorDirection));

        if (canPlaceRoomOnNeighbourTile && hasDoorOnWall)
        {
            return true;
        }
        if(hasDoorOnWall && neighbourRoomExist && hasDoorOnNeighbourWall)
        {
            return true;
        }
        if(!hasDoorOnWall && !hasDoorOnNeighbourWall)
        {
            return true;
        }
        return false;
    }

    private bool checkIfDoorAgainstWall(TileData targetTileData, Orientation roomOrientation, Orientation wallDirection)
    {
        if (targetTileData == null)
        {
            return false;
        }

        bool foundDoor = false;
        foreach (DoorData door in targetTileData.doors)
        {
            if (CommonOperations.getTransformedOrientation(door.wallDirection, roomOrientation) == wallDirection)
            {
                foundDoor = true;
            }
        }
        return foundDoor;
    }
    private bool checkIfDoorAgainstWall(Coordinate coordinate, Orientation wallDirection)
    {
        TileData targetTileData = mansionDataHandler.getTileDataOnCoordinate(coordinate);
        RoomDataHolder targetRoomData = mansionDataHandler.getRoomDataOnCoordinate(coordinate);
        if (targetRoomData == null)
        {
            return false;
        }
        return checkIfDoorAgainstWall(targetTileData, targetRoomData.getOrientation(), wallDirection);
    }

    private Coordinate getCoordinateOfTileBehindDoor(Coordinate baseTile, TileData tile, DoorData door, Orientation orientationOfTheRoom)
    {
        Coordinate transformed_doorTile_offset = CommonOperations.getTransformedCoordinate(tile.tileCoordinate, orientationOfTheRoom);
        Coordinate doorTileLocation = baseTile + transformed_doorTile_offset;
        Orientation transformed_doorOrientation = CommonOperations.getTransformedOrientation(door.wallDirection, orientationOfTheRoom);
        //Coordinate + Orientation = one step in that direction
        return doorTileLocation + transformed_doorOrientation;
    }

    private GameObject buildRoomObject(GameObject roomPrefab, RoomDataHolder roomData)
    {
        GameObject builtObject = Instantiate(roomPrefab);
        setRoomLocation(builtObject, roomData.getWorldPosition(), roomData.getOrientation());
        return builtObject;
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
                return new Vector3(0, 90, 0);
            case Orientation.SOUTH:
                return new Vector3(0, 180, 0);
            case Orientation.WEST:
                return new Vector3(0, 270, 0);
            default:
                return new Vector3(0, 0, 0);
        }
    }

    public void setRoomLocation(GameObject room, Coordinate coordinate, Orientation orientation)
    {
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


    private List<SpawnableObject> getAllIndependentObjects(RoomDataHolder roomData)
    {
        List<SpawnableObject> independent = new List<SpawnableObject>();
        foreach (SpawnableObject obj in roomData.spawnableObjects)
        {
            if(obj.requiredObjects.Length == 0)
            {
                independent.Add(obj);
            }
        }
        return independent;
    }

    private List<SpawnableObject> getAllDependentObjects(RoomDataHolder roomData)
    {
        List<SpawnableObject> independent = new List<SpawnableObject>();
        foreach (SpawnableObject obj in roomData.spawnableObjects)
        {
            if (obj.requiredObjects.Length != 0)
            {
                independent.Add(obj);
            }
        }
        return independent;
    }

    private void inactivateAllSpawnableObjects(RoomDataHolder roomData)
    {
        foreach(SpawnableObject obj in roomData.spawnableObjects)
        {
            if(obj == null)
            {
                continue;
            }
            try
            {
                ((GameObject)obj.roomObject).SetActive(false);
            }
            catch (System.InvalidCastException e){}
        }
    }

    private void spawnRandomObjects(RoomDataHolder roomData)
    {
        int numPossibleObj = roomData.spawnableObjects.Length;
        if (numPossibleObj == 0)
        {
            return;
        }
        inactivateAllSpawnableObjects(roomData);
        List<SpawnableObject> spawnableList = getAllIndependentObjects(roomData);
        List<SpawnableObject> dependentList = getAllDependentObjects(roomData);
        List<SpawnableObject> spawnedObjects = new List<SpawnableObject>();

        int numToSpawn = (int)Random.Range(1, numPossibleObj+1);

        while(numToSpawn > 0)
        {
            if (spawnableList.Count == 0)
            {
                break;
            }

            /* Spawn object */
            SpawnableObject obj = CommonOperations.getRandomItemFromList(spawnableList);
            if(obj == null)
            {
                --numToSpawn;
                continue;
            }
            try
            {
                GameObject gameObject = (GameObject)obj.roomObject;
                gameObject.SetActive(true);
            }
            catch (System.InvalidCastException e)
            {
                --numToSpawn;
                continue;
            }

            if (spawnedObjects.Contains(obj))
            {
                --numToSpawn;
                continue;
            }
            else
            {
                spawnedObjects.Add(obj);
            }

            /* Remove conflicting objects from spawn list */
            foreach (Object conflictingObj in obj.conflictingObjects)
            {
                if(conflictingObj == null)
                {
                    continue;
                }
                for (int i = spawnableList.Count-1; i >= 0; i--)
                {
                    SpawnableObject spawnableObj = spawnableList[i];
                    if (spawnableObj.roomObject.GetInstanceID() == conflictingObj.GetInstanceID())
                    {
                        spawnableList.RemoveAt(i);
                    }
                }
            }

            /* Add dependent object to spawn list if all good */
            for(int i = dependentList.Count-1; i >= 0; i--)
            {
                SpawnableObject dependentObj = dependentList[i];
                if(dependentObj == null)
                {
                    continue;
                }
                bool missingReuirement = false;
                foreach (Object requiredObj in dependentObj.requiredObjects)
                {
                    if(requiredObj == null)
                    {
                        continue;
                    }
                    bool foundIt = false;
                    foreach(SpawnableObject spawnedObj in spawnedObjects)
                    {
                        if (spawnedObj.roomObject.GetInstanceID() == requiredObj.GetInstanceID())
                        {
                            foundIt = true;
                        }
                    }
                    if (!foundIt)
                    {
                        missingReuirement = true;
                    }
                }
                if (!missingReuirement)
                {
                    spawnableList.Add(dependentObj);
                    dependentList.RemoveAt(i);
                }
            }
        }
    }

    // Use this for initialization
    void Start () {
        //Random.seed = 1489245699;
        Debug.Log("Random seed: " + Random.seed);
        int width = 6;
        int depth = 6;
        for (int i = 100; i > 0; --i)
        {
            //placeRoom(new Coordinate((int)(Random.value*width), 1, (int)(Random.value * depth)));
        }
        //placeRoom(new Coordinate(0, 0, 0));
        //placeRoom(new Coordinate(0, 0, 1));
        //placeRoom(new Coordinate(1, 0, 0));
        placeRoom(new Coordinate(5, 1, 5));
    }

    // Update is called once per frame
    int updateCounter = 0;
    int roomsSpawned = 0;
    void Update () {
        if (roomsSpawned > 00)
            return;
        ++updateCounter;
        if (updateCounter == 30)
        {
            updateCounter = 0;
            //placeRoom(1, 0, 1, 0, 10, 1, 30);
            ++roomsSpawned;
        }
	}

    private void placeRoom(int amount, int startX, int startZ, int startY, int width, int height, int depth)
    {
        for (int i = amount; i > 0; --i)
        {
            placeRoom(new Coordinate(startX + (int)(Random.value * width), startY + (int)(Random.value * height), startZ + (int)(Random.value * depth)));
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

    private class OnRoomEnteredCallback : RoomColliderHandler.Callback
    {
        private RoomDataHolder roomData;
        private MansionBuilder mansionBuilder;
        public OnRoomEnteredCallback(MansionBuilder parentBuilder, RoomDataHolder roomData)
        {
            this.roomData = roomData;
            this.mansionBuilder = parentBuilder;
        }

        override
        public void onEnter()
        {
            IEnumerator coroutine = mansionBuilder.placeRoomsOnAllDoors(roomData);
            mansionBuilder.StartCoroutine(coroutine);
            //mansionBuilder.placeRoomsOnAllDoors(roomData);
            roomData.removeCallback();
        }
    }
}
