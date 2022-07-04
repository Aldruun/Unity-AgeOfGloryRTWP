using System;

[Flags]
public enum DoorFlags
{
    OPEN = 1,
    LOCKED = 2,
    RESET = 4,
    DETECTABLE = 8,
    BROKEN = 16,
    CANTCLOSE = 32,
    LINKED = 64,
    SECRET = 128,
    FOUND = 256,
    TRANSPARENT = 512,
    KEY = 1024,
    SLIDE = 2048,
    WARNINGTEXTDISPLAYED = 0x1000,
    HIDDEN = 8192,
    USEUPKEY = 0x4000,
    LOCKEDINFOTEXT = 0x8000,
    WARNINGINFOTEXT = 0x10000
}

public class Door : Highlightable
{
    public string keyRef;
    //internal ContainerType Type = ContainerType.CONTAINER;
    internal int LockDifficulty = 0;
    internal DoorFlags Flags = 0;
    public int Health = 100;
    //int inventory.SetInventoryType(INVENTORY_HEAP);
    //int memset(groundicons, 0, sizeof(groundicons) );
    //internal int groundiconcover = 0;
    internal int OpenFail = 0;

    public Door(HighlightableMonoObject scrMono, int lockDifficulty,
        bool locked, int trapDetectionDiff, int trapRemovalDiff, int trapped, int trapDetected)
    {
        highlightObject = scrMono;
        LockDifficulty = lockDifficulty;
        if(locked)
            SetDoorLocked(true);
        this.trapDetectionDiff = trapDetectionDiff;
        this.trapRemovalDiff = trapRemovalDiff;
        this.trapped = trapped;
        this.trapDetected = trapDetected;
        //this.groundiconcover = groundiconcover;
        //OpenFail = openFail;
        //Interface.GetCurrentGame().GetCurrentMap().AddDoor(this);
    }

    internal void SetDoorLocked(bool lockit)
    {
        if(lockit)
        {
            Flags |= DoorFlags.LOCKED;
        }
        else
        {
            Flags &= ~DoorFlags.LOCKED;
        }
    }

    internal bool Visible()
    {
        return (((Flags & DoorFlags.SECRET) == 0 || (Flags & DoorFlags.FOUND) != 0) && (Flags & DoorFlags.HIDDEN) == 0);
    }
}
