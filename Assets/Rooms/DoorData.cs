using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DoorData
{
    public enum doorState
    {
        CLOSED = 0,
        OPEN = 1
    }

    public bool overwriteable = true;
    public Orientation wallDirection = Orientation.NORTH;
    public doorState startingState = doorState.CLOSED;
    public int health = 0;
    [Header("Door Locked")]
    public bool locked = false;
    [ConditionalHide("locked", true)]
    public DoorLock doorLock = new DoorLock(0);
}