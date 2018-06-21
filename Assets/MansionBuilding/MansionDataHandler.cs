using UnityEngine;
using System.Collections;

public class MansionDataHandler {
    private MansionDataHolder mansionData;

    public MansionDataHandler(ushort mansionMaxWidth, ushort mansionMaxHeight, ushort mansionMaxDepth) {
       mansionData = new MansionDataHolder(mansionMaxWidth, mansionMaxHeight, mansionMaxDepth);
    }

    public bool isRoomTileOccupied(Coordinate coordinate)
    {
        if(coordinate.x >= mansionData.width || coordinate.x < 0)
        {
            return true;
        }
        if (coordinate.y >= mansionData.height || coordinate.y < 0)
        {
            return true;
        }
        if (coordinate.z >= mansionData.depth || coordinate.z < 0)
        {
            return true;
        }
        return mansionData.occupiedTiles[coordinate.x, coordinate.y, coordinate.z] != null;
    }

    public bool getRoomDataOnTile(Coordinate coordinate)
    {
        return mansionData.occupiedTiles[coordinate.x, coordinate.y, coordinate.z];
    }

    public void addRoomTiles(RoomDataHolder roomData)
    {
        Coordinate roomPosition = roomData.getWorldPosition();
        foreach (Coordinate tile in roomData.occupiedTiles)
        {
            Coordinate transformed_tile = CommonOperations.getTransformedCoordinate(tile, roomData.getOrientation());
            Coordinate transformedCoordinate = roomPosition + transformed_tile;
            Debug.Log("Occupying tile: " + transformedCoordinate.getCoordinateString());
            if(transformedCoordinate.x < 0 || transformedCoordinate.y < 0 || transformedCoordinate.z < 0)
            {
                return;
            }
            mansionData.occupiedTiles[transformedCoordinate.x, transformedCoordinate.y, transformedCoordinate.z] = roomData;
        }
    }
}
