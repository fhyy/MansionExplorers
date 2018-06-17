using UnityEngine;
using System.Collections;

public class RoomDataHolder : MonoBehaviour {

    public enum roomDirection
    {
        NORTH = 0,
        EAST = 1,
        SOUTH = 2,
        WEST = 3
    }

    public roomDirection baseRotation = roomDirection.NORTH;
    public Vector3[] occupiedTiles = { };
    public DoorData[] doors = { };
    public SpawnableObject[] spawnableObjects = { };

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
