using UnityEngine;
using System.Collections;

public class DoorColliderHandler : MonoBehaviour
{

    private ArrayList callbacks;

    public void registerCallback(Callback callback)
    {
        this.callbacks.Add(callback);
    }

    public void removeCallback(Callback callback)
    {
        callbacks.Remove(callback);
    }

    public void removeCallbacks()
    {
        callbacks.Clear();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (callbacks != null)
            {
                foreach(Callback callback in callbacks)
                {
                    callback.onEnter();
                }
            }
        }
    }

    public abstract class Callback
    {
        public abstract void onEnter();
    }
}
