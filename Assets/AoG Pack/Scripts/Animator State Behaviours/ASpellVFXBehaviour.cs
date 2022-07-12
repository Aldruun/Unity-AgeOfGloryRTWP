using System.Collections;
using UnityEngine;

public class ASpellVFXBehaviour : StateMachineBehaviour
{
    private Actor actor;
    private ParticleSystem[] chargeVFX;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if(actor == null)
        {
            actor = animator.GetComponent<Actor>();
        }

        Vector3 chargeVFXPos = actor.transform.position + Vector3.up * 1.1f + actor.transform.forward * 0.5f;
        float castTime = actor.Combat.equippedSpell.castTimeMult;

        //switch(actor.Combat.equippedSpell.damageType)
        //{
        //    case DamageType.MAGIC:
        //        chargeVFX = VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_chargespell_magic", ObjectPoolingCategory.VFX), chargeVFXPos, Quaternion.identity, castTime);

        //        break;
        //    case DamageType.MAGICFIRE:
        //    case DamageType.FIRE:
        //        chargeVFX = VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_chargespell_fire", ObjectPoolingCategory.VFX), chargeVFXPos, Quaternion.identity, castTime);

        //        break;
        //    case DamageType.MAGICCOLD:
        //    case DamageType.COLD:
        //        chargeVFX = VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_chargespell_frost", ObjectPoolingCategory.VFX), chargeVFXPos, Quaternion.identity, castTime);
        //        break;
        //    case DamageType.ELECTRICITY:
        //        chargeVFX = VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_chargespell_lightning", ObjectPoolingCategory.VFX), chargeVFXPos, Quaternion.identity, castTime);
        //        break;
        //    case DamageType.RADIANT:
        //        chargeVFX = VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_chargespell_radiant", ObjectPoolingCategory.VFX), chargeVFXPos, Quaternion.identity, castTime);
        //        break;
        //    //case DamageType.MAGICCOLD:
        //    case DamageType.HEAL:
        //        chargeVFX = VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_chargespell_heal", ObjectPoolingCategory.VFX), chargeVFXPos, Quaternion.identity, castTime);
        //        break;
        //    case DamageType.POISON:
        //        chargeVFX = VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_chargespell_poison", ObjectPoolingCategory.VFX), chargeVFXPos, Quaternion.identity, castTime);
        //        break;
        //    default:
        //        chargeVFX = VFXPlayer.TriggerVFX(PoolSystem.GetPoolObject("vfx_chargespell_radiant", ObjectPoolingCategory.VFX), chargeVFXPos, Quaternion.identity, castTime);
        //        break;
        //}

        if(actor.ActorStats.Race != ActorRace.GOBLIN)
        {
            switch(actor.ActorStats.Gender)
            {
                case Gender.Female:
                    SFXPlayer.TriggerSFX(ResourceManager.sfx_list_spellchants_gibberish_f[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_f.Length)], actor.transform.position);
                    break;
                //case Gender.MALE:

                //    break;
                default:
                    SFXPlayer.TriggerSFX(ResourceManager.sfx_list_spellchants_gibberish_m[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_m.Length)], actor.transform.position);
                    break;
            }
        }

        //if(chargeVFX == null)
        //{
        //    Debug.LogError("_chargeVFX = null for effectType '" + actor.Combat.equippedSpell.damageType + "'");
        //}
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(var pe in chargeVFX)
        {
            pe.Stop();
        }
    }

}
