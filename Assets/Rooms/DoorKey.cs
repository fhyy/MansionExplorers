using UnityEngine;

[System.Serializable]
public class DoorKey : MonoBehaviour
{
    public enum KeyType
    {
        MASTER = 0,
        UNIQUE = 1
    }

    public KeyType keyType = KeyType.MASTER;
    private ushort keyIdentifier = 0; // 0 = master

    public DoorKey(ushort id)
    {
        keyIdentifier = id;
        keyType = id == 0 ? KeyType.MASTER : KeyType.UNIQUE;
    }

    public void setKeyIdentifier(ushort id)
    {
        keyIdentifier = id;
        keyType = id == 0 ? KeyType.MASTER : KeyType.UNIQUE;
    }

    /* A Key unlocks a lock if the UInt16 id values match */
    public ushort getKeyIdentifier()
    {
        return keyIdentifier;
    }
}
