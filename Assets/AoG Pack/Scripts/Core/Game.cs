
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//[System.Serializable]
//public class ControlStatus
//{
//    public int partyAIEnabled = 1;
//}

//public class Game : Scriptable
//{
//    private Dictionary<string, Map> _maps;
//    private Map _currentMap;

//    public List<ActorInput> NPCs { get; } = new List<ActorInput>();

//    //List<BehaviourUpdater> _behaviourUpdaters;
//    //public List<Party> agentGroups = new List<Party>();
//    public List<ActorInput> PCs { get; } = new List<ActorInput>();
//    public List<GameObject> garbage { get; } = new List<GameObject>();
//    //public List<ActorInput> selected { get; } = new List<ActorInput>();

//    //   protagonist = PM_YES; //set it to 2 for iwd/iwd2 and 0 for pst
//    //partysize = 6;
//    //Ticks = 0;
//    //GameTime = RealTime = 0;
//    //version = 0;
//    //Expansion = 0;
//    //LoadMos[0] = 0;
//    //TextScreen[0] = 0;
//    //SelectedSingle = 1; //the PC we are looking at (inventory, shop)
//    private int PartyGold = 0;
//    //SetScript(core->GlobalScript, 0 );
//    //   MapIndex = -1;
//    //Reputation = 0;
//    //ControlStatus = 0;
//    //CombatCounter = 0; //stored here until we know better
//    //StateOverrideTime = 0;
//    //StateOverrideFlag = 0;
//    //BanterBlockTime = 0;
//    //BanterBlockFlag = 0;
//    //WeatherBits = 0;
//    //kaputz = NULL;
//    //beasts = NULL;
//    //mazedata = NULL;
//    //timestop_owner = NULL;
//    //timestop_end = 0;
//    //event_timer = 0;
//    //event_handler = NULL;
//    //weather = new Particles(200);
//    //   weather->SetRegion(0, 0, core->Width, core->Height);
//    //   LastScriptUpdate = 0;
//    //WhichFormation = 0;
//    //CurrentLink = 0;
//    internal bool PartyAttack { get; set; }
//    internal float CombatCounter { get; private set; }

//    internal ControlStatus controlStatus;

//    public Game(string activeSceneIdentifier)
//    {
//        controlStatus = new ControlStatus();

//        Debug.Log("# <color=green>Setting initial game</color>");
//        InitScriptable(ScriptableType.GLOBAL);
//        _maps = new Dictionary<string, Map>();
//        SetCurrentMap(activeSceneIdentifier);
//        _currentMap.InitScriptable(ScriptableType.AREA);
//        SetMap(_currentMap);


//    }

//    public void UpdateScripts()
//    {
//        UpdateScriptableLoop();

//        PartyAttack = false;

//        foreach(var map in _maps.Values)
//        {
//            map.UpdateScripts();
//        }

//        if(PartyAttack)
//        {
//            CombatCounter = 10;
//        }
//        else
//        {
//            if(CombatCounter > 0)
//            {
//                CombatCounter -= Time.deltaTime;
//            }
//            else
//            {
//                //foreach(var pc in PCs)
//                //{
//                //    if(pc.GetCurrentArea().AnyEnemyNearPoint(pc.transform.position))
//                //    {
//                //        CombatCounter = 10;
//                //        return;
//                //    }
//                //}

//                //foreach(var pc in PCs)
//                //{
//                //    if(pc.essential && pc.dead)
//                //    {
//                //        pc.Execute_ModifyHealth(1, ModType.ABSOLUTE);
//                //        pc.Reactivate();

//                //        if(pc.CheckStatusEffect(Status.SLEEP) == 0)
//                //            pc.Execute_StandUp();

//                //        //pc.Stop();
//                //    }
//                //    pc.animator.SetFloat("fIdleStance", 0);

//                //}
//            }
//        }

//        //if(EveryoneDead())
//        //{
//        //    //TODO Death scene


//        //}
//    }

//    internal void Release()
//    {
//        _currentMap.Release();
//        PCs.Clear();
//        NPCs.Clear();
//    }

//    public int LoadMap(string sceneName)
//    {
//        Map map = FindMap(sceneName);
//        SetMap(map);
//        Scene scene = SceneManager.GetSceneByName(sceneName);
//        SceneManager.LoadScene(scene.buildIndex);
//        map.sceneIndex = scene.buildIndex;
        
        
//        //TODO Loading screen


//        return scene.buildIndex;
//    }

//    public void AddMap(string sceneIdentifier)
//    {
//        if(_maps.ContainsKey(sceneIdentifier) == false)
//        {
//            Map newMap = new Map();
//            //newMap.sceneIndex = sceneIdentifier;
//            _maps.Add(sceneIdentifier, newMap);
//        }
//    }

//    public Map FindMap(string sceneIdentifier)
//    {
//        Map map = _maps[sceneIdentifier];
//        return map;
//    }

//    public Map GetCurrentMap()
//    {
//        return _currentMap;
//    }

//    public Map SetCurrentMap(string sceneIdentifier)
//    {
//        if(_maps.ContainsKey(sceneIdentifier) == false)
//        {
//            AddMap(sceneIdentifier);
//        }

//        _currentMap = _maps[sceneIdentifier];
    
//        return _currentMap;
//    }

//    private bool IsDay()
//    {

//        int daynight = GameTimeManager.GetGameTimeInHours();
//	    // FIXME: doesn't match GameScript::TimeOfDay
//	    if(daynight<4 || daynight>20) {
//		    return false;
//	    }
//	    return true;
//    }

//    /* sends the hotkey trigger to all selected actors */
//    public static void SendHotKey(long Key)
//    {
//        //for(var actor : selected)
//        //{
//        //    if(actor->IsSelected())
//        //    {
//        //        actor->AddTrigger(TriggerEntry(trigger_hotkey, (ieDword)Key));
//        //    }
//        //}
//    }

//    // Gets sum of party level, if onlyalive is true, then counts only living PCs
//    // If you need average party level, divide this with GetPartySize
//    public static int GetTotalPartyLevel(bool onlyalive)
//    {
//        //int amount = 0;

//        //for (auto pc : PCs) {
//        //		if (onlyalive) {
//        //			if (pc->GetStat(IE_STATE_ID) & STATE_DEAD) {
//        //				continue;
//        //			}
//        //		}
//        //amount += pc->GetXPLevel(0);
//        //}

//        //return amount;

//        return -1000;
//    }

//    internal ActorInput GetPC(int slot, bool onlyalive)
//    {
//        if(slot >= PCs.Count)
//        {
//            return null;
//        }
//        if(onlyalive)
//        {
//            foreach(var pc in PCs)
//            {
//                if(IsAlive(pc) && slot-- == 0)
//                {
//                    return pc;
//                }
//            }
//            return null;
//        }
//        return PCs[slot];
//    }

//    private void SwapPCs(int Index1, int Index2)
//    {
//	    if (Index1 >= PCs.Count) {
//		    return;
//	    }

//	    if (Index2 >= PCs.Count) {
//		    return;
//	    }

//	    int tmp = PCs[Index1].InParty;
//        PCs[Index1].InParty = PCs[Index2].InParty;
//	    PCs[Index2].InParty = tmp;
//	    //signal a change of the portrait window
//	    //TODO core.SetEventFlag(EF_PORTRAIT | EF_SELECTION);
//    }

//    public bool IsAlive(ActorInput actor)
//    {
//        return actor.dead == false;
//    }

//    public ActorInput FindPC(int partyID)
//    {
//        foreach(var pc in PCs)
//        {
//            if(partyID == pc.InParty)
//            {
//                return pc;
//            }
//        }

//        return null;
//    }

//    public int GetPartySize(bool onlyAlive)
//    {
//        if(onlyAlive)
//        {
//            int count = 0;
//            foreach(var pc in PCs)
//            {
//                if(pc.dead)
//                {
//                    continue;
//                }
//                count++;
//            }
//            return count;
//        }
//        return PCs.Count;
//    }

//    public int JoinParty(ActorInput actor, int join)
//    {
//        Debug.Assert(PCs.Contains(actor) == false, "ActorInput already in party");

//        //var portrait = UIScript.AddPartymemberPortrait(actor, ResourceManager.GetPortraitSprite(actor.GetPortraitSprite()) /*ResourceManager.GetRandomPortraitSprite(actor.ActorRecord.gender, actor.ActorRecord.race, actor.ActorRecord.m_class)*/  /*ResourceManager.GetRandomPortraitSprite(actor.ActorRecord.gender, actor.ActorRecord.race, actor.ActorRecord.m_class)*/);
//        //actor.SetPortrait(portrait);

//        //core->SetEventFlag(EF_PORTRAIT);
//        //actor->CreateStats(); //create stats if they didn't exist yet
//        //actor->InitButtons(actor->GetActiveClass(), false); // init actor's action bar
//        //actor->SetBase(IE_EXPLORE, 1);
//        //if(join & JP_INITPOS)
//        //{
//        //	InitActorPos(actor);
//        //}
//        int slot = InParty(actor);
//        if(slot != -1)
//        {
//            return slot;
//        }
//        int size = PCs.Count;

//        //if(join & JP_JOIN)
//        //{
//        //	//update kit abilities of actor
//        //	ieDword baseclass = 0;
//        //	if(core->HasFeature(GF_LEVELSLOT_PER_CLASS))
//        //	{
//        //		// get the class for iwd2; luckily there are no NPCs, everyone joins at level 1, so multi-kit annoyances can be ignored
//        //		baseclass = actor->GetBase(IE_CLASS);
//        //	}
//        //	actor->ApplyKit(false, baseclass);
//        //	//update the quickslots
//        //	actor->ReinitQuickSlots();
//        //	//set the joining date
//        //	actor->PCStats->JoinDate = GameTime;
//        //	//if the protagonist has the same portrait replace it
//        //	ActorInput* prot = GetPC(0, false);
//        //	if(prot && (!strcmp(actor->SmallPortrait, prot->SmallPortrait) || !strcmp(actor->LargePortrait, prot->LargePortrait)))
//        //	{
//        //		AutoTable ptab("portrait");
//        //		if(ptab)
//        //		{
//        //			CopyResRef(actor->SmallPortrait, ptab->QueryField(actor->SmallPortrait, "REPLACEMENT"));
//        //			CopyResRef(actor->LargePortrait, ptab->QueryField(actor->LargePortrait, "REPLACEMENT"));
//        //		}
//        //	}

//        //	//set the lastjoined trigger
//        //	if(size)
//        //	{
//        //		ieDword id = actor->GetGlobalID();
//        //		for(size_t i = 0; i < size; i++)
//        //		{
//        //			ActorInput* a = GetPC(i, false);
//        //			a->PCStats->LastJoined = id;
//        //		}
//        //	}
//        //	else
//        //	{
//        //		Reputation = actor->GetStat(IE_REPUTATION);
//        //	}
//        //	AddTrigger(TriggerEntry(trigger_joins, actor->GetGlobalID()));
//        //}
//        slot = InStore(actor); // If actor was NPC, remove him from the NPCs list
//        if(slot >= 0)
//        {
//            //ActorInput m = NPCs.begin() + slot;
//            NPCs.Remove(actor);
//        }

//        PCs.Add(actor);
//        if(actor.InParty == 0)
//        {
//            actor.InParty = (size + 1);
//        }

//        GameEventSystem.OnPartyMemberAdded?.Invoke(actor);

//        //if(PCs.Count == 1)
//        //{
//        //    //! Select first actor
//        //    GameEventSystem.OnHeroPortraitClicked?.Invoke(actor, true);
//        //    GameStateManager.Instance.GetCameraScript().JumpTo(actor.transform.position);
//        //    GameStateManager.Instance.GetUIScript().SetUIAIToggleOn(actor.aiControlled);
//        //}

//        //if(join & (JP_INITPOS | JP_SELECT))
//        //{
//        //	actor->Selected = 0; // don't confuse SelectActor!
//        //	SelectActor(actor, true, SELECT_NORMAL);
//        //}

//        //return (int)size;

//        return size;
//    }

//    public int LeaveParty(ActorInput actor)
//    {
//        Debug.Assert(PCs.Contains(actor), "ActorInput not in party");
//        GameEventSystem.OnPartyMemberRemoved(actor);
//        //UIScript.RemovePartymemberPortrait(actor.InParty-1);
//        //	core->SetEventFlag(EF_PORTRAIT);
//        //	actor->CreateStats(); //create or update stats for leaving
//        //	actor->SetBase(IE_EXPLORE, 0);

//        SelectionManager.DeselectPC(actor);
//        int slot = InParty(actor);
//        if(slot < 0)
//        {
//            return slot;
//        }

//        //	std::vector<ActorInput*>::iterator m = PCs.begin() + slot;
//        PCs.Remove(actor);
//        GameEventSystem.OnPartyMemberRemoved(actor);

//        //	ieDword id = actor->GetGlobalID();
//        //	for(auto pc : PCs)
//        //	{
//        //	pc->PCStats->LastLeft = id;
//        //if(actor.InParty > actor.InParty)
//        //{
//            actor.InParty = 0;
//        //}
//        //}
//        ////removing from party, but actor remains in 'game'
//        //actor->SetPersistent(0);
//        //NPCs.push_back(actor);
//        NPCs.Add(actor);
//        //if(core->HasFeature(GF_HAS_DPLAYER))
//        //{
//        //	// we must reset various existing scripts
//        //	actor->SetScript("", SCR_DEFAULT);
//        //	actor->SetScript("", SCR_CLASS, false);
//        //	actor->SetScript("", SCR_RACE, false);
//        //	actor->SetScript("WTASIGHT", SCR_GENERAL, false);
//        //	if(actor->GetBase(IE_MC_FLAGS) & MC_EXPORTABLE)
//        //	{
//        //		actor->SetDialog("MULTIJ");
//        //	}
//        //}
//        //actor->SetBase(IE_EA, EA_NEUTRAL);
//        //AddTrigger(TriggerEntry(trigger_leaves, actor->GetGlobalID()));
//        //return (int)NPCs.size() - 1;
//        return -1000;
//    }

//    private int InParty(ActorInput pc)
//    {
//        for(int i = 0; i < PCs.Count; i++)
//        {
//            if(PCs[i] == pc)
//            {
//                return i;
//            }
//        }
//        return -1;
//    }

//    private int InStore(ActorInput pc)
//    {
//	    for (int i = 0; i < NPCs.Count; i++) {
//		    if (NPCs[i] == pc) {
//			    return i;
//		    }
//	    }
//	    return -1;
//    }

//    /*
//     * SelectActor() - handle (de)selecting actors.
//     * If selection was changed, runs "SelectionChanged" handler
//     *
//     * actor - either specific actor, or NULL for all
//     * select - whether actor(s) should be selected or deselected
//     * flags:
//     * SELECT_REPLACE - if true, deselect all other actors when selecting one
//     * SELECT_QUIET   - do not run handler if selection was changed. Used for
//     * nested calls to SelectActor()
//     */
   
//    public static void ShareXP(int xp, int flags)
//    {
//        //int individual;

//        //if(flags & SX_CR)
//        //{
//        //    xp = GetXPFromCR(xp);
//        //}

//        //if(flags & SX_DIVIDE)
//        //{
//        //    int PartySize = GetPartySize(true); //party size, only alive
//        //    if(PartySize < 1)
//        //    {
//        //        return;
//        //    }
//        //    individual = xp / PartySize;
//        //}
//        //else
//        //{
//        //    individual = xp;
//        //}

//        //if(!individual)
//        //{
//        //    return;
//        //}

//        ////you have gained/lost ... xp
//        //if(core->HasFeedback(FT_MISC))
//        //{
//        //    if(xp > 0)
//        //    {
//        //        displaymsg->DisplayConstantStringValue(STR_GOTXP, DMC_BG2XPGREEN, (ieDword)xp);
//        //    }
//        //    else
//        //    {
//        //        displaymsg->DisplayConstantStringValue(STR_LOSTXP, DMC_BG2XPGREEN, (ieDword) - xp);
//        //    }
//        //}
//        //for(auto pc : PCs)
//        //{
//        //    if(pc->GetStat(IE_STATE_ID) & STATE_DEAD)
//        //    {
//        //        continue;
//        //    }
//        //    pc->AddExperience(individual, flags & SX_COMBAT);
//        //}
//    }

//    private void AddGold(int add)
//    {
//        if(add == 0)
//        {
//            return;
//        }

//        int old = PartyGold;
//        PartyGold = Mathf.Max(0, PartyGold + add);
//        if(old < PartyGold)
//        {
//            DevConsole.Log("Gold received: " + (PartyGold - old));
//        }
//        else
//        {
//            DevConsole.Log("Gold lost: " + (old - PartyGold));
//        }
//    }

//    public static bool EveryoneStopped()
//    {
//        //foreach(var pc in PCs)
//        //{
//        //    if(pc.InMove())
//        //        return false;
//        //}
//        //return true;


//        // Just to make code compile
//        return false;
//    }

//    //canmove=true: if some PC can't move (or hostile), then this returns false
//    public bool EveryoneNearPoint(Map area, Vector3 point, int flags)
//    {
//        foreach(var pc in PCs)
//        {
//            //if(flags & ENP_ONLYSELECT)
//            //{
//            //    if(!pc->Selected)
//            //    {
//            //        continue;
//            //    }
//            //}
//            if(pc.dead/*->GetStat(IE_STATE_ID) & STATE_DEAD*/)
//            {
//                continue;
//            }
//            //if(flags & ENP_CANMOVE)
//            //{
//            //    //someone is uncontrollable, can't move
//            //    if(pc->GetStat(IE_EA) > EA_GOODCUTOFF)
//            //    {
//            //        return false;
//            //    }

//            //    if(pc->GetStat(IE_STATE_ID) & STATE_CANTMOVE)
//            //    {
//            //        return false;
//            //    }
//            //}
//            //if(pc->GetCurrentArea() != area)
//            //{
//            //    return false;
//            //}
//            if(Vector3.Distance(point, pc.transform.position) > Constants.MAX_TRAVELING_DISTANCE)
//            {
//                //Log(MESSAGE, "Game", "ActorInput %s is not near!", pc->LongName);
//                return false;
//            }
//        }
//        return true;
//    }

//    //called when someone died
//    public static void PartyMemberDied(ActorInput actor)
//    {
//        //this could be null, in some extreme cases...
//        //const Map* area = actor->GetCurrentArea();

//        //unsigned int size = PCs.size();
//        //ActorInput* react = NULL;
//        //for(unsigned int i = core->Roll(1, size, 0), n = 0; n < size; i++, n++)
//        //{
//        //    ActorInput* pc = PCs[i % size];
//        //    if(pc == actor)
//        //    {
//        //        continue;
//        //    }
//        //    if(pc->GetStat(IE_STATE_ID) & STATE_DEAD)
//        //    {
//        //        continue;
//        //    }
//        //    if(pc->GetStat(IE_MC_FLAGS) & MC_EXPORTABLE)
//        //    {
//        //        continue;
//        //    }
//        //    if(pc->GetCurrentArea() != area)
//        //    {
//        //        continue;
//        //    }
//        //    if(pc->HasSpecialDeathReaction(actor->GetScriptName()))
//        //    {
//        //        react = pc;
//        //        break;
//        //    }
//        //    else if(react == NULL)
//        //    {
//        //        react = pc;
//        //    }
//        //}
//        //AddTrigger(TriggerEntry(trigger_partymemberdied, actor->GetGlobalID()));
//        //if(react != NULL)
//        //{
//        //    react->ReactToDeath(actor->GetScriptName());
//        //}
//    }

//    public bool AnyPCInCombat()
//    {
//        if(CombatCounter <= 0)
//        {
//            return false;
//        }

//        return true;
//    }

//    //returns true if the protagonist (or the whole party died)
//    public bool EveryoneDead()
//    {
//        //if there are no PCs, then we assume everyone dead
//        if(PCs.Count == 0)
//        {
//            return true;
//        }
//        //if(protagonist == PM_NO)
//        //{
//        //    const ActorInput* nameless = PCs[0];
//        //    // don't trigger this outside pst, our game loop depends on it
//        //    if(nameless->GetStat(IE_STATE_ID) & STATE_NOSAVE && core->HasFeature(GF_PST_STATE_FLAGS))
//        //    {
//        //        if(area->INISpawn)
//        //        {
//        //            area->INISpawn->RespawnNameless();
//        //        }
//        //    }
//        //    return false;
//        //}
//        //// if protagonist died
//        //if(protagonist == PM_YES)
//        //{
//        //    if(PCs[0]->GetStat(IE_STATE_ID) & STATE_NOSAVE)
//        //    {
//        //        return true;
//        //    }
//        //    return false;
//        //}
//        ////protagonist == 2
//        foreach(var pc in PCs)
//        {
//            if(pc.dead == false/*!(pc->GetStat(IE_STATE_ID) & STATE_NOSAVE)*/)
//            {
//                return false;
//            }
//        }
//        return true;
//    }

//    // returns 0 if it can
//    // returns strref or -1 if it can't
//    public int CanPartyRest(int checks)
//    {
//        //if(checks == REST_NOCHECKS)
//        //    return 0;

//        //if(checks & REST_CONTROL)
//        //{
//        //    for(auto pc : PCs)
//        //    {
//        //        if(pc->GetStat(IE_STATE_ID) & STATE_MINDLESS)
//        //        {
//        //            // You cannot rest at this time because you do not have control of all your party members
//        //            return displaymsg->GetStringReference(STR_CANTTRESTNOCONTROL);
//        //        }
//        //    }
//        //}

//        ActorInput leader = GetPC(0, true);
//        Debug.Assert(leader != null);
//        //Map area = leader.GetCurrentArea();
//        ////we let them rest if someone is paralyzed, but the others gather around
//        //if(checks & REST_SCATTER)
//        //{
//        //if(EveryoneNearPoint(area, leader.transform.position, 0) == false)
//        //{
//        //    //party too scattered
//        //    return -1;
//        //    //return displaymsg->GetStringReference(STR_SCATTERED);
//        //}
//        //}

//        //if(checks & REST_CRITTER)
//        //{
//        //    //don't allow resting while in combat
//        if(AnyPCInCombat())
//        {
//            return -1;
//            //return displaymsg->GetStringReference(STR_CANTRESTMONS);
//        }
//        //    //don't allow resting if hostiles are nearby
//        //if(area.AnyEnemyNearPoint(leader.transform.position))
//        //{
//        //    return -1;
//        //    //return displaymsg->GetStringReference(STR_CANTRESTMONS);
//        //}
//        //}

//        ////rest check, if PartyRested should be set, area should return true
//        //if(checks & REST_AREA)
//        //{
//        //    //you cannot rest here
//        //    if(area->AreaFlags & AF_NOSAVE)
//        //    {
//        //        return displaymsg->GetStringReference(STR_MAYNOTREST);
//        //    }

//        //    if(core->HasFeature(GF_AREA_OVERRIDE))
//        //    {
//        //        // pst doesn't care about area types (see comments near AF_NOSAVE definition)
//        //        // and repurposes these area flags!
//        //        if((area->AreaFlags & (AF_TUTORIAL | AF_DEADMAGIC)) == (AF_TUTORIAL | AF_DEADMAGIC))
//        //        {
//        //            // you must obtain permission
//        //            return 38587;
//        //        }
//        //        else if(area->AreaFlags & AF_TUTORIAL)
//        //        {
//        //            // you cannot rest in this area
//        //            return 34601;
//        //        }
//        //        else if(area->AreaFlags & AF_DEADMAGIC)
//        //        {
//        //            // you cannot rest right now
//        //            return displaymsg->GetStringReference(STR_MAYNOTREST);
//        //        }
//        //    }
//        //    else
//        //    {
//        //        // you may not rest here, find an inn
//        //        if(!(area->AreaType & (AT_FOREST | AT_DUNGEON | AT_CAN_REST_INDOORS)))
//        //        {
//        //            // at least in iwd1, the outdoor bit is not enough
//        //            if(area->AreaType & AT_OUTDOOR && !core->HasFeature(GF_AREA_VISITED_VAR))
//        //            {
//        //                return 0;
//        //            }
//        //            return displaymsg->GetStringReference(STR_MAYNOTREST);
//        //        }
//        //    }
//        //}

//        return 0;
//    }

//    // checks: can anything prevent us from resting?
//    // dream:
//    //   -1: no dream
//    //    0, 8+: dream based on area
//    //    1-7: dream selected from a fixed list
//    // hp: how much hp the rest will heal
//    // returns true if a cutscene dream is about to be played
//    public static bool RestParty(int checks, int dream, int hp)
//    {
//        //if(CanPartyRest(checks))
//        //{
//        //    return false;
//        //}

//        //const ActorInput* leader = GetPC(0, true);
//        //assert(leader);
//        //// TODO: implement "rest until healed", it's an option in some games
//        //int hours = 8;
//        //int hoursLeft = 0;
//        //if(checks & REST_AREA)
//        //{
//        //    //area encounters
//        //    // also advances gametime (so partial rest is possible)
//        //    Trigger* parameters = new Trigger;
//        //    parameters->int0Parameter = 0; // TIMEOFDAY_DAY, with a slight preference for daytime interrupts
//        //    hoursLeft = area->CheckRestInterruptsAndPassTime(leader->Pos, hours, GameScript::TimeOfDay(nullptr, parameters));
//        //    delete parameters;
//        //    if(hoursLeft)
//        //    {
//        //        // partial rest only, so adjust the parameters for the loop below
//        //        if(hp)
//        //        {
//        //            hp = hp * (hours - hoursLeft) / hours;
//        //            // 0 means full heal, so we need to cancel it if we rounded to 0
//        //            if(!hp)
//        //            {
//        //                hp = 1;
//        //            }
//        //        }
//        //        hours -= hoursLeft;
//        //        // the interruption occured before any resting could be done, so just bail out
//        //        if(!hours)
//        //        {
//        //            return false;
//        //        }
//        //    }
//        //}
//        //else
//        //{
//        //    AdvanceTime(hours * core->Time.hour_size);
//        //}

//        //int i = GetPartySize(true); // party size, only alive

//        //while(i--)
//        //{
//        //    ActorInput* tar = GetPC(i, true);
//        //    tar->ClearPath();
//        //    tar->SetModal(MS_NONE, 0);
//        //    //if hp = 0, then healing will be complete
//        //    tar->Heal(hp);
//        //    // auto-cast memorized healing spells if requested and available
//        //    // run it only once, since it loops itself to save time
//        //    if(i + 1 == GetPartySize(true))
//        //    {
//        //        CastOnRest();
//        //    }
//        //    //removes fatigue, recharges spells
//        //    tar->Rest(hours);
//        //    if(!hoursLeft)
//        //        tar->PartyRested();
//        //}

//        //// also let familiars rest
//        //for(auto tar : NPCs)
//        //{
//        //    if(tar->GetBase(IE_EA) == EA_FAMILIAR)
//        //    {
//        //        tar->ClearPath();
//        //        tar->SetModal(MS_NONE, 0);
//        //        tar->Heal(hp);
//        //        tar->Rest(hours);
//        //        if(!hoursLeft)
//        //            tar->PartyRested();
//        //    }
//        //}

//        //// abort the partial rest; we got what we wanted
//        //if(hoursLeft)
//        //{
//        //    return false;
//        //}

//        ////movie, cutscene, and still frame dreams
//        //bool cutscene = false;
//        //if(dream >= 0)
//        //{
//        //    //cutscene dreams
//        //    if(gamedata->Exists("player1d", IE_BCS_CLASS_ID, true))
//        //    {
//        //        cutscene = true;
//        //        PlayerDream();
//        //        // all games have these bg1 leftovers, but only bg2 replaced the content
//        //    }
//        //    else if(gamedata->GetResource("drmtxt2", IE_2DA_CLASS_ID, true)->Size() > 0)
//        //    {
//        //        cutscene = true;
//        //        TextDream();
//        //    }

//        //    //select dream based on area
//        //    ieResRef* movie;
//        //    if(dream == 0 || dream > 7)
//        //    {
//        //        movie = GetDream(area);
//        //    }
//        //    else
//        //    {
//        //        movie = restmovies + dream;
//        //    }
//        //    if(*movie[0] != '*')
//        //    {
//        //        core->PlayMovie(*movie);
//        //    }
//        //}

//        ////set partyrested flags
//        //PartyRested();
//        //area->PartyRested();
//        //core->SetEventFlag(EF_ACTION);

//        ////bg1 has "You have rested for <DURATION>" while pst has "You have
//        ////rested for <HOUR> <DURATION>" and then bg1 has "<HOUR> hours" while
//        ////pst just has "Hours", so this works for both
//        //int restindex = displaymsg->GetStringReference(STR_REST);
//        //int hrsindex = displaymsg->GetStringReference(STR_HOURS);
//        //char* tmpstr = NULL;

//        //core->GetTokenDictionary()->SetAtCopy("HOUR", hours);

//        ////this would be bad
//        //if(hrsindex == -1 || restindex == -1)
//        //    return cutscene;
//        //tmpstr = core->GetCString(hrsindex, 0);
//        ////as would this
//        //if(!tmpstr)
//        //    return cutscene;

//        //core->GetTokenDictionary()->SetAtCopy("DURATION", tmpstr);
//        //core->FreeString(tmpstr);
//        //displaymsg->DisplayString(restindex, DMC_WHITE, 0);
//        //return cutscene;


//        // Just to make code compile
//        return false;
//    }

//}