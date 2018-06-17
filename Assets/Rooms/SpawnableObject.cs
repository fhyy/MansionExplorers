using UnityEngine;

[System.Serializable]
public class SpawnableObject
{
    public Object roomObject;
    public Object[] requiredObjects = { };
    public Object[] conflictingObjects = { };
}