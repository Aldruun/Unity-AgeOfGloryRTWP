//using System.Collections;
//using UnityEngine;

//public class Skill_DefaultAttack : Skill
//{
//    private int currentMeleeComboStage;
//    private float[] meleeComboTimes;
//    private readonly int MeleeComboStage = Animator.StringToHash("iMeleeComboStage");

//    public override void Init()
//    {
//        base.Init();
//        meleeComboTimes = new float[] { 0.5f, 0.5f, 0.5f };
//    }

//    public override bool ConditionsMetAI(NPCInput ai)
//    {
//        NPCInput self = ai;
//        if(self.Equipment.equippedWeapon.Weapon.IsRanged == false && self.AIProfile.AIFlags.HasFlag(AIFlags.AllowMelee) == false)
//        {
//            return false;
//        }

//        if(self.Equipment.equippedWeapon.Weapon.IsRanged && self.AIProfile.AIFlags.HasFlag(AIFlags.AllowRanged) == false)
//        {
//            return false;
//        }

//        skillTarget = self.Combat.GetHostileTarget();
        
//        return /*agent.weaponDrawn &&*/ skillTarget != null;
//    }

//    public override bool ConditionsMetPlayer(Actor actor)
//    {
//        return true;
//    }

//    public override void IndividualSetup(Actor self)
//    {

//        if(skillTarget == null)
//            return;

//        currentMeleeComboStage = 0;
//        self.Animation.Animator.SetInteger(MeleeComboStage, 0);

//        self.Combat.Execute_Attack();
        
//        cooldown = self.Equipment.equippedWeapon.Weapon.speed;

//        if(self.Equipment.equippedWeapon.Weapon.IsRanged == false)
//        {
//            if(Random.value > 0.6f)
//            {
//                cooldown *= 2;
//                currentMeleeComboStage = 1;
//                self.Animation.Animator.SetInteger(MeleeComboStage, currentMeleeComboStage);
//                self.StartCoroutine(CR_DoCombo(self));
//            }
//        }
//    }

//    private IEnumerator CR_DoCombo(Actor self)
//    {
//        yield return new WaitForSeconds(0.5f);

//        if(skillTarget == null)
//            yield break;

//        self.Combat.Execute_Attack();
//    }

//    public override void SpawnVFX(Actor self, Actor target, Vector3 targetPosition)
//    {
        
//        //AgentVFXController.TriggerVFX(ResourceManager.GetPoolObject("vfx_spell_healsingletarget", ObjectPoolingCategory.VFX), skillTarget.m_transform, Quaternion.identity);
//    }

//    internal override float GetActivationRange(Actor self)
//    {
//        return self.Equipment.equippedWeapon.Weapon.Range - 0.1f;
//    }
//}