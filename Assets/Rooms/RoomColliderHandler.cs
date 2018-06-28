using UnityEngine;
using System.Collections;

public class RoomColliderHandler : MonoBehaviour {

    private Callback callback;

	public void registerCallback(Callback callback)
    {
        this.callback = callback;
    }

    public void removeCallback()
    {
        callback = null;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (callback != null)
            {
                callback.onEnter();
            }
        }
    }

    public abstract class Callback
    {
        public abstract void onEnter();
    }
}
