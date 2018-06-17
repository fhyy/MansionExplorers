using UnityEngine;

[System.Serializable]
public class DoorData
{
    public enum doorDirection {
        NORTH = 0,
        EAST = 1,
        SOUTH = 2,
        WEST = 3
    }
    public enum doorState
    {
        CLOSED = 0,
        OPEN = 1
    }

    public Vector3 position = new Vector3(0,0,0);
    public doorDirection wallDirection = doorDirection.NORTH;
    public doorState startingState = doorState.CLOSED;
    public bool overwriteable = true;
    public int health = 0;
}