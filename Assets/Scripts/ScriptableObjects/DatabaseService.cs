using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseService
{
    public GameSettings GameSettings { get; internal set; }
    public ActorDatabase ActorDatabase { get; internal set; }
    public SpellCompendium SpellCompendium { get; internal set; }

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

        SpellCompendium = Resources.Load<SpellCompendium>("ScriptableObjects/SpellCompendium");
        if(SpellCompendium == null)
        {
            Debug.LogError("GameInterface: Initilization failed -> SpellCompendium was null");
            return null;
        }
        Debug.Log("<color=green>// SpellCompendium Loaded</color>");

        return this;
    }
}
