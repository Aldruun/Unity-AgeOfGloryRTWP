using UnityEngine;

public class State_Escort : State<NPCInput>
{
    public NPCInput ctrl;

    private float _idleTimer = 2;
    private bool _init;
    private float _actionTimer;

    public State_Escort()
    {
    }

    public override void Enter(NPCInput ctrl)
    {
        if(ctrl.debugInput)
            Debug.Log(ctrl.GetName() + ":<color=cyan>#</color> Escort State Enter");
        this.ctrl = ctrl;
        //ctrl.agent.actorLocomotion.ChangeForm(AnimationSet.DEFAULT);
        _idleTimer = Random.Range(0, 2);
        _init = true;
        _actionTimer = Random.Range(2, 6);
        ctrl.NavAgent.ResetPath();
        //ctrl.agent.pathfinder.enableRotation = true;
    }

    public override void Execute(NPCInput ctrl)
    {
        if(ctrl.EscortTarget == null)
        {
            return;
        }
        // if(ctrl.debugInput)
        //     Debug.Log(ctrl.GetName() + ":<color=cyan>#</color> Escort State Update");

        Vector3 dirToEscTarget = ctrl.EscortTarget.transform.position - ctrl.transform.position;
        if(dirToEscTarget.magnitude > 5 + ctrl.EscortIndex)
        {
            ctrl.ChangeMovementSpeed(MovementSpeed.Run);
        }
        else if(dirToEscTarget.magnitude < 3 + ctrl.EscortIndex)
        {
            ctrl.ChangeMovementSpeed(MovementSpeed.Walk);
        }

        ctrl.SetDestination(ctrl.EscortTarget.transform.position, 2 + ctrl.EscortIndex);
        

        if(ctrl.EscortTarget.Combat.WeaponDrawn || ctrl.EscortTarget.Combat.SpellDrawn)
        {
            if(ctrl.Combat.WeaponDrawn || ctrl.Combat.SpellDrawn)
                return;

            if(ctrl.HasSpells)
            {
                if(ctrl.Combat.equippedSpell == null)
                {
                    Spell spell = SpellCasting.ChooseAnySpellWithKeyword(ctrl, Keyword.LightningDamage | Keyword.FireDamage | Keyword.FrostDamage);
                    if(spell != null)
                    {
                        ctrl.Combat.SetEquippedSpell(spell);
                    }
                }
                if(ctrl.Combat.equippedSpell != null)
                    ctrl.Combat.Execute_ReadySpells();
            }
            else if(ctrl.Combat.WeaponDrawn == false)
                ctrl.Combat.Execute_DrawWeapon();
        }
        else
        {
            if(ctrl.Combat.SpellDrawn)
                ctrl.Combat.Execute_SheathSpells();
            else if(ctrl.Combat.WeaponDrawn)
                ctrl.Combat.Execute_SheathWeapon();
        }
    }

    public override void Exit(NPCInput ctrl)
    {
        //Debug.Log("# Idle State Exit");
    }


    //Vector3 PickRandomPoint()
    //{
    //    var point = ;

    //    point.y = 0;
    //    point += ctrl.agent.pathfinder.position;
    //    return point;
    //}


    //Vector3 GetRandomNavMeshPointAroundOrigin(Vector3 origin)
    //{
    //    return origin + Random.insideUnitSphere * 20;
    //}
}