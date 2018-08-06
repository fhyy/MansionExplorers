using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractionHandler : MonoBehaviour {

    private Coordinate worldPosition = null;
    private Orientation worldOrientation = Orientation.NORTH;
    public DoorColliderHandler colliderHandler = null;

    public OnEnterCallback onEnterCallback = null;

    public DoorInteractionHandler(DoorInteractionHandler doorInteractionHandler)
    {
        this.worldPosition = new Coordinate(doorInteractionHandler.worldPosition);
        this.worldOrientation = doorInteractionHandler.worldOrientation;
        this.colliderHandler = doorInteractionHandler.colliderHandler;
        this.onEnterCallback = doorInteractionHandler.onEnterCallback;
    }

    public void registerCallback(DoorColliderHandler.Callback callback)
    {
        this.colliderHandler.registerCallback(callback);
    }

    public void removeCallbacks()
    {
        this.colliderHandler.removeCallbacks();
    }

    public void setWorldPosition(Coordinate coordinate)
    {
        worldPosition = coordinate;
    }
    public Coordinate getWorldPosition()
    {
        return worldPosition;
    }

    public void setOrientation(Orientation orientation)
    {
        worldOrientation = orientation;
    }
    public Orientation getOrientation()
    {
        return worldOrientation;
    }

    // Use this for initialization
    void Start () {
		
	}

    public abstract class OnEnterCallback
    {
        public abstract void onEnter();
    }
}
