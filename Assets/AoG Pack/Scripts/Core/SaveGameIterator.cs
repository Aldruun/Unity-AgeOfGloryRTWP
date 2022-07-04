using UnityEngine;

public static class SaveGameIterator
{
    public static int CreateSaveGame(int index, bool mqs)
    {


        return 0;
    }


    /// <summary>
    /// Probably for overwriting existing savegame.
    /// </summary>
    public static int CreateSaveGame(SaveGame save, string slotName)
    {
        if(slotName == "")
        {
            return -1;
        }

        return 0;
    }
}

[System.Serializable]
public class SaveGame
{
    public int PortraitCount;
    public int SaveID;
    public string Name;
    public string Prefix;
    public string Path;
    public string Date;
    public string SlotName;

    public SaveGame()
    {

    }

    public int GetPortraitCount()
    {
        return PortraitCount;
    }

    public int GetSaveID()
    {
        return SaveID;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetPrefix()
    {
        return Prefix;
    }

    public string GetPath()
    {
        return Path;
    }

    public string GetDate()
    {
        return Date;
    }

    public string GetSlotName()
    {
        return SlotName;
    }

    public Sprite GetPortrait(int index)
    {
        //if(index > PortraitCount)
        //{
        //    return null;
        //}
        //char nPath[_MAX_PATH];
        //snprintf(nPath, _MAX_PATH, "PORTRT%d", index);
        //ResourceHolder<ImageMgr> im = GetResourceHolder<ImageMgr>(nPath, manager, true);
        //if(!im)
            return null;
        //return im->GetSprite2D();
    }

    public Sprite GetPreview()
    {
        //ResourceHolder<ImageMgr> im = GetResourceHolder<ImageMgr>(Prefix, manager, true);
        //if(!im)
            return null;
        //return im->GetSprite2D();
    }
}