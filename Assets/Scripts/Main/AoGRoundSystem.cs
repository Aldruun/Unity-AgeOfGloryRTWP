using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AoGRoundSystem
{
    public const float ROUNDTIME = 6f;
    public int actionCounter;
    public int attackCounter;
    public float timeToNextRound;
    public Action<int> SetAttackState;
    public Action<float> TimeToNextActionHook;
    public Action<float> TimeToNextCastHook;
    private float _freezeRoundTimer;
    private float _initiativeTimer;
    private float _spellPauseTimer;
    private bool debugRoundSystem;
    private Actor self;
    private StatusEffectSystem statusEffectSystem;
    private ActorStats stats;

    [Flags]
    enum LuckyRollFlags
    {
        CRITICAL = 1,
        DAMAGELUCK = 2,
        NEGATIVE = 4
    }

    public AoGRoundSystem(Actor actor, StatusEffectSystem statusEffectSystem)
    {
        self = actor;
        stats = actor.ActorStats;
        this.statusEffectSystem = statusEffectSystem;
    }

    public void ProcessRoundTime()
    {
        if(_spellPauseTimer > 0)
        {
            _spellPauseTimer -= Time.deltaTime;
            TimeToNextCastHook?.Invoke(_spellPauseTimer);
        }

        if(_initiativeTimer > 0)
        {
            timeToNextRound = ROUNDTIME;
            _initiativeTimer -= Time.deltaTime;
        }

        if(_freezeRoundTimer > 0)
        {
            _freezeRoundTimer -= Time.deltaTime;
            return;
        }

        if(debugRoundSystem)
            Debug.Log(self.GetName() + " checking round status");
        timeToNextRound -= Time.deltaTime;
        TimeToNextActionHook?.Invoke(timeToNextRound);
        //flurry = timeToNextRound > 4 ? 1 : timeToNextRound > 2 ? 2 : 3;
        if(timeToNextRound <= 0)
        {
            statusEffectSystem.DecrementRounds();
            actionCounter = 0;
            attackCounter = GetTotalAPR();
            timeToNextRound = ROUNDTIME;
            if(debugRoundSystem)
                Debug.Log(self.GetName() + " next round");

        }
    }

    public bool CanPerformAttack()
    {
        //if(self.IsPlayer)
        //{
        //    AoG.Core.GameInterface.Instance.GetCurrentGame().PartyAttack = true;
        //}

        if(attackCounter <= 0 || ROUNDTIME / GetTotalAPR() < timeToNextRound)
        {
            return false;
        }

        //! Example: 5.5 sec left (Ceil = 6) % 6 / 3 = 2 remaining 0 -> Attack
        if(/*(int)*/(Mathf.CeilToInt(timeToNextRound/* + 0.5f*/) /*+ 1*/) % (int)((ROUNDTIME / GetTotalAPR()) + 0.5f) != 0)
        {
            if(debugRoundSystem)
            {
                SetAttackState?.Invoke(0);
            }
            attackCounter--;
            return true;
        }

        if(debugRoundSystem)
        {
            SetAttackState?.Invoke(1);
        }

        return false;
    }

    public int GetTotalAPR()
    {
        int apr = self.ActorStats.GetStat(ActorStat.APR);
        //if(debug)
        //    Debug.Log(GetName() + " APR " + apr);

        Debug.Assert(apr > 0, "APR may not be 0");

        return apr;
    }

    public int GetAC()
    {
        return stats.GetBaseStat(ActorStat.AC);
    }

    public int GetACFromEquipment()
    {
        //TODO Merge and return equipment ac
        return GetAC();
    }

    public void SetSpellPause()
    {
        _spellPauseTimer = 6;
    }

    public bool CanCast()
    {
        if(InSpellPause() || self.Animation.Animator.GetCurrentAnimatorStateInfo(2).IsName("New State") == false || self.isCasting || self.isDowned || self.dead)
        {
            return false;
        }

        return true;
    }

    public float GetSpellPauseTimer()
    {
        return _spellPauseTimer;
    }

    public bool InSpellPause()
    {
        return _spellPauseTimer > 0;
    }

    public void FreezeRoundTimer(float freezeTime)
    {
        _freezeRoundTimer = freezeTime;
    }

    // luck increases the minimum roll per dice, but only up to the number of dice sides;
    // luck does not affect critical hit chances:
    // if critical is set, it will return 1/sides on a critical, otherwise it can never
    // return a critical miss when luck is positive and can return a false critical hit
    // Callees with LR_CRITICAL should check if the result matches 1 or size*dice.
    private int LuckyRoll(int dice, int size, int add, LuckyRollFlags flags, Actor opponent = null)
    {
        int luck;

        luck = stats.GetBaseStat(ActorStat.LUCK);

        //damageluck is additive with regular luck (used for maximized damage, righteous magic)
        if(flags.HasFlag(LuckyRollFlags.DAMAGELUCK))
        {
            //luck += GetStat(ActorStat.DAMAGELUCK);
        }

        //it is always the opponent's luck that decrease damage (or anything)
        if(opponent != null)
            luck -= opponent.ActorStats.GetBaseStat(ActorStat.LUCK);

        if(flags.HasFlag(LuckyRollFlags.NEGATIVE))
        {
            luck = -luck;
        }

        if(dice < 1 || size < 1)
        {
            return (add + luck > 1 ? add + luck : 1);
        }

        bool critical = flags.HasFlag(LuckyRollFlags.CRITICAL);
        if(dice > 100)
        {
            int bonus;
            if(Mathf.Abs(luck) > size)
            {
                bonus = luck / Mathf.Abs(luck) * size;
            }
            else
            {
                bonus = luck;
            }
            int roll1 = DnD.RollBG(1, dice * size, 0);
            if(critical && (roll1 == 1 || roll1 == size))
            {
                return roll1;
            }
            else
            {
                return add + dice * (size + bonus) / 2;
            }
        }

        int roll, result = 0, misses = 0, hits = 0;
        for(int i = 0; i < dice; i++)
        {
            roll = DnD.RollBG(1, size, 0);
            if(roll == 1)
            {
                misses++;
            }
            else if(roll == size)
            {
                hits++;
            }
            roll += luck;
            if(roll > size)
            {
                roll = size;
            }
            else if(roll < 1)
            {
                roll = 1;
            }
            result += roll;
        }

        // ensure we can still return a critical failure/success
        if(critical && dice == misses)
            return 1;
        if(critical && dice == hits)
            return size * dice;

        // hack for critical mode, so overbearing luck does not cause a critical hit
        // FIXME: decouple the result from the critical info
        if(critical && result + add >= size * dice)
        {
            return size * dice - 1;
        }
        else
        {
            return result + add;
        }
    }

    public void RollForInitiative()
    {
        _initiativeTimer = Random.Range(0f, 6f);
    }
}