using UnityEngine;

[System.Serializable]
public class DoorLock {
    public enum LockType
    {
        UNIVERSAL = 0,
        UNIQUE = 1
    }

    public LockType lockType = LockType.UNIVERSAL;
    private ushort lockIdentifier = 0; // 0 = universal

    public DoorLock(ushort id)
    {
        lockIdentifier = id;
        lockType = id == 0 ? LockType.UNIVERSAL : LockType.UNIQUE;
    }

    public void setLockIdentifier(ushort id)
    {
        lockIdentifier = id;
        lockType = id == 0 ? LockType.UNIVERSAL : LockType.UNIQUE;
    }
     
    /* A unique Key unlocks a unique lock if the UInt16 id values match */
    public bool canUnlock(DoorKey key)
    {
        return key.getKeyIdentifier() == lockIdentifier || lockType == LockType.UNIVERSAL || key.keyType == DoorKey.KeyType.MASTER;
    }
}
