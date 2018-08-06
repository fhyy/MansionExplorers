#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;
using UnityStandardAssets;

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
    private GameObject doorObject = null;

    public void setDoorObject(GameObject doorObject)
    {
        this.doorObject = doorObject;
    }

    public GameObject getDoorObject()
    {
        return this.doorObject;
    }
}