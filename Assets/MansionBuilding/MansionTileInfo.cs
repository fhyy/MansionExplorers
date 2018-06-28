using UnityEngine;
using System.Collections;

public class MansionTileInfo
{
    public RoomDataHolder roomData;
    public TileData tileData;

    public MansionTileInfo(RoomDataHolder roomData, TileData tileData)
    {
        this.roomData = roomData;
        this.tileData = tileData;
    }
}
