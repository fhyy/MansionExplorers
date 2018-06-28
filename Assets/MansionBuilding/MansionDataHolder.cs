using UnityEngine;
using System.Collections;

public class MansionDataHolder {

    public MansionTileInfo[,,] occupiedTiles;
    public ushort height;
    public ushort width;
    public ushort depth;

    public MansionDataHolder(ushort width, ushort height, ushort depth)
    {
        occupiedTiles = new MansionTileInfo[width, height, depth]; //[x, y, z]
        this.width = width;
        this.height = height;
        this.depth = depth;
    }
}
