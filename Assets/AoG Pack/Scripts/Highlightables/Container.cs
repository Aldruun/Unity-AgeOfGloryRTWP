using System;
using UnityEngine;

[Flags]
public enum TriggerFlags
{
    INVISIBLE = 1,
    RESET = 2,
    TRAVEL_PARTY = 4,
    DETECTABLE = 8,
    TRAP_TUTORIAL = 32,
    TRAP_NPC = 64,
    TRAP_DEACTIVATED = 256,
    TRAVEL_NONPC = 512,
    TRAP_USEPOINT = 1024,
    INFO_DOOR = 2048
}

public enum ContainerType
{
    CONTAINER,
    PILE
}

[Flags]
public enum ContainerFlags
{
    LOCKED = 1,
    RESET = 8,
    DISABLED = (32 | 128)
}

public class Container : Highlightable
{
    //#######################
    // VISUALS
    //#######################
    public string keyRef;
    internal ContainerType Type = ContainerType.CONTAINER;
    internal int LockDifficulty = 0;
    internal ContainerFlags Flags = 0;
    //int inventory.SetInventoryType(INVENTORY_HEAP);
    //int memset(groundicons, 0, sizeof(groundicons) );
    //internal int groundiconcover = 0;
    internal int OpenFail = 0;

    internal int Health = 100;

    public Transform transform;
    public GameObject gameObject;

    public Container(HighlightableMonoObject scrMono, ContainerType type, int lockDifficulty,
        bool locked, int trapDetectionDiff, int trapRemovalDiff, int trapped, int trapDetected)
    {
        transform = scrMono.transform;
        gameObject = scrMono.gameObject;
        highlightObject = scrMono;
        Type = type;
        LockDifficulty = lockDifficulty;
        if(locked)
            SetContainerLocked(true);
        this.trapDetectionDiff = trapDetectionDiff;
        this.trapRemovalDiff = trapRemovalDiff;
        this.trapped = trapped;
        this.trapDetected = trapDetected;
        //this.groundiconcover = groundiconcover;
        //OpenFail = openFail;
        //Interface.GetCurrentGame().GetCurrentMap().AddContainer(this);
    }

    internal void SetContainerLocked(bool lockit)
    {
        if(lockit)
        {
            Flags |= ContainerFlags.LOCKED;
        }
        else
        {
            Flags &= ~ContainerFlags.LOCKED;
        }
    }

    internal bool IsOpen()
    {
        if(Flags.HasFlag(ContainerFlags.LOCKED))
            return false;
        return true;
    }

    internal void TryPickLock(Actor actor)
    {
        if(LockDifficulty == 100)
        {
            if(OpenFail != -1)
            {
                //displaymsg->DisplayStringName(OpenFail, DMC_BG2XPGREEN, actor, IE_STR_SOUND | IE_STR_SPEECH);
            }
            else
            {
                //displaymsg->DisplayConstantStringName(STR_CONT_NOPICK, DMC_BG2XPGREEN, actor);
            }
            return;
        }
        //int stat = actor.GetStat(ActorAttribute.loc.LOCKPICKING);
        //if(core->HasFeature(GF_3ED_RULES))
        //{
        //    int skill = actor->GetSkill(IE_LOCKPICKING);
        //    if(skill == 0)
        //    { // a trained skill, make sure we fail
        //        stat = 0;
        //    }
        //    else
        //    {
        //        stat *= 7; // convert to percent (magic 7 is from RE)
        //        int dexmod = actor->GetAbilityBonus(IE_DEX);
        //        stat += dexmod; // the original didn't use it, so let's not multiply it
        //        displaymsg->DisplayRollStringName(39301, DMC_LIGHTGREY, actor, stat - dexmod, LockDifficulty, dexmod);
        //    }
        //}
        //if(stat < LockDifficulty)
        //{
            //displaymsg->DisplayConstantStringName(STR_LOCKPICK_FAILED, DMC_BG2XPGREEN, actor);
            //AddTrigger(TriggerEntry(trigger_picklockfailed, actor->GetGlobalID()));
            //core->PlaySound(DS_PICKFAIL, SFX_CHAN_HITS); //AMB_D21
            //return;
        //}
        SetContainerLocked(false);
        //core->GetGameControl()->ResetTargetMode();
        //displaymsg->DisplayConstantStringName(STR_LOCKPICK_DONE, DMC_LIGHTGREY, actor);
        //AddTrigger(TriggerEntry(trigger_unlocked, actor->GetGlobalID()));
        //core->PlaySound(DS_PICKLOCK, SFX_CHAN_HITS); //AMB_D21D
        //ImmediateEvent();
        //int xp = actor->CalculateExperience(XP_LOCKPICK, actor->GetXPLevel(1));
        //Game game = Interface.GetCurrentGame();
        //game->ShareXP(xp, SX_DIVIDE);
    }

    internal void TryBashLock(Actor actor)
    {
        //Get the strength bonus agains lock difficulty
        //int bonus;
        //int roll;

        //if(core->HasFeature(GF_3ED_RULES))
        //{
        //    bonus = actor->GetAbilityBonus(IE_STR);
        //    roll = actor->LuckyRoll(1, 100, bonus, 0);
        //}
        //else
        //{
        int str = ActorUtility.GetModdedStat(actor.ActorStats, ActorStat.STRENGTH);
        //int strEx = actor.GetStat(Stat.STRENGTHEXTRA);
        //bonus = DnD.AttributeModifier(str); //BEND_BARS_LIFT_GATES
        //roll = actor.LuckyRoll(1, 10, bonus, 0);
        //}

        //if(core->HasFeature(GF_3ED_RULES))
        //{
        //    // ~Bash door check. Roll %d + %d Str mod > %d door DC.~
        //    // there is no separate string for non-doors
        //    displaymsg->DisplayRollStringName(20460, DMC_LIGHTGREY, actor, roll, bonus, LockDifficulty);
        //}

        //actor.FaceTarget(this);
        //if(roll < LockDifficulty || LockDifficulty == 100)
        //{
        //    //displaymsg->DisplayConstantStringName(STR_CONTBASH_FAIL, DMC_BG2XPGREEN, actor);
        //    return;
        //}

        //displaymsg->DisplayConstantStringName(STR_CONTBASH_DONE, DMC_LIGHTGREY, actor);
        SetContainerLocked(false);
        //core->GetGameControl()->ResetTargetMode();
        //Is this really useful ?
        //AddTrigger(TriggerEntry(trigger_unlocked, actor->GetGlobalID()));
        //ImmediateEvent();
    }

    internal bool TryUnlock(Actor actor)
    {
        if(Flags.HasFlag(ContainerFlags.LOCKED) == false)
            return true;

        return TryUnlock(actor, false);
    }
    bool TryUnlock(Actor actor, bool removekey)
    {
        string Key = keyRef;
        //Actor haskey = null;

        //if(Key != "" && actor.InParty > 0)
        //{
        //    Game game = Interface.GetCurrentGame();
        //    //allow unlock when the key is on any partymember
        //    for(int idx = 0; idx < game.GetPartySize(false); idx++)
        //    {
        //        Actor pc = game.FindPC(idx + 1);
        //        if(pc == null)
        //            continue;

        //        if(pc.inventory.HasItem(Key, 0))
        //        {
        //            haskey = pc;
        //            break;
        //        }
        //    }
        //}
        //else if(Key != "")
        //{
        //    //actor is not in party, check only actor
        //    if(actor.inventory.HasItem(Key, 0))
        //    {
        //        haskey = actor;
        //    }
        //}

        //if(haskey != null)
        //{
        //    return false;
        //}

        //if(removekey)
        //{
        //    Item item = null;
        //    haskey.inventory.RemoveItem(Key, 0, 1);
        //    //the item should always be existing!!!
        //    item = null;
        //}

        return true;
    }

    public void Execute_ModifyHealth(int value, ModType modType)
    {
        //throw new System.NotImplementedException();
    }


    public Transform GetAttackPoint()
    {
        return transform;
        //throw new System.NotImplementedException();
    }

    public string GetName()
    {
        return transform.name;
    }
}
