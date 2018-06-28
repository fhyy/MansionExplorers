using UnityEngine;
using System.Collections;

public class MansionDataHandler {
    private MansionDataHolder mansionData;

    public MansionDataHandler(ushort mansionMaxWidth, ushort mansionMaxHeight, ushort mansionMaxDepth) {
       mansionData = new MansionDataHolder(mansionMaxWidth, mansionMaxHeight, mansionMaxDepth);
    }

    private bool checkIfValidCoordinates(Coordinate coordinate)
    {
        if (coordinate.x >= mansionData.width || coordinate.x < 0)
        {
            return false;
        }
        if (coordinate.y >= mansionData.height || coordinate.y < 0)
        {
            return false;
        }
        if (coordinate.z >= mansionData.depth || coordinate.z < 0)
        {
            return false;
        }
        return true;
    }

    public bool canPlaceRoomOnTile(Coordinate coordinate)
    {
        if (!checkIfValidCoordinates(coordinate))
        {
            return false;
        }
        return mansionData.occupiedTiles[coordinate.x, coordinate.y, coordinate.z] == null;
    }

    public bool isRoomTileOccupied(Coordinate coordinate)
    {

        if (!checkIfValidCoordinates(coordinate))
        {
            return false;
        }
        return mansionData.occupiedTiles[coordinate.x, coordinate.y, coordinate.z] != null;
    }

    public RoomDataHolder getRoomDataOnCoordinate(Coordinate coordinate)
    {
        if (!checkIfValidCoordinates(coordinate))
        {
            return null;
        }
        MansionTileInfo tileInfo = mansionData.occupiedTiles[coordinate.x, coordinate.y, coordinate.z];
        if (tileInfo != null)
        {
            return tileInfo.roomData;
        }
        return null;
    }

    public TileData getTileDataOnCoordinate(Coordinate coordinate)
    {
        if (!checkIfValidCoordinates(coordinate))
        {
            return null;
        }
        MansionTileInfo tileInfo = mansionData.occupiedTiles[coordinate.x, coordinate.y, coordinate.z];
        if(tileInfo != null)
        {
            return tileInfo.tileData;
        }
        return null;
    }

    public void addRoomTiles(RoomDataHolder roomData)
    {
        Coordinate roomPosition = roomData.getWorldPosition();
        foreach (TileData tile in roomData.occupiedTiles)
        {
            Coordinate transformed_tile = CommonOperations.getTransformedCoordinate(tile.tileCoordinate, roomData.getOrientation());
            Coordinate transformedCoordinate = roomPosition + transformed_tile;
            if(transformedCoordinate.x < 0 || transformedCoordinate.y < 0 || transformedCoordinate.z < 0)
            {
                return;
            }
            mansionData.occupiedTiles[transformedCoordinate.x, transformedCoordinate.y, transformedCoordinate.z] = new MansionTileInfo(roomData, tile);
        }
    }
}
