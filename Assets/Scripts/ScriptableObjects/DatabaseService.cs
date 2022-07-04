using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseService
{
    public GameSettings GameSettings;
    public ActorDatabase ActorDatabase;

    public DatabaseService InitScriptableObjectDatabases()
    {

        GameSettings = Resources.Load<GameSettings>("ScriptableObjects/GameSettings");
        if(GameSettings == null)
        {
            Debug.LogError("GameInterface: Initilization failed -> GameSettings was null");
            return null;
        }
        Debug.Log("<color=green>// GameSettings Loaded</color>");

        ActorDatabase = Resources.Load<ActorDatabase>("ScriptableObjects/ActorDatabase");
        if(ActorDatabase == null)
        {
            Debug.LogError("GameInterface: Initilization failed -> ActorDatabase was null");
            return null;
        }
        Debug.Log("<color=green>// ActorDatabase Loaded</color>");

        return this;
    }
}
