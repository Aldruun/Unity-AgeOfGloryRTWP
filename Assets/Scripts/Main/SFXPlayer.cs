using AoG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum VerbalConstantType
{
    TAUNT,
    SELECT,
    HURT,
    DEAD,
    CHANT,
    ATTACK,
    ATTACKLONG,
    JUMP,
    MOVECOMMANDYES,
    MOVECOMMANDNO
}

public static class SFXPlayer
{
    // Start is called before the first frame update

    internal static void PlaySound_DrawWeapon(WeaponCategory weaponCategory, Vector3 point)
    {
        switch(weaponCategory)
        {
            case WeaponCategory.Unarmed:
                return;
            case WeaponCategory.Shortbow:
                break;
            case WeaponCategory.Longbow:
                break;
            case WeaponCategory.XBow:
                break;
            case WeaponCategory.Sling:
                break;
            case WeaponCategory.Dart:
                break;
            case WeaponCategory.ThrowingKnife:
                break;
            case WeaponCategory.Blunt:
                break;
            case WeaponCategory.Spiked:
                break;
            case WeaponCategory.Dagger:
                break;
            case WeaponCategory.ShortSword:
                break;
            case WeaponCategory.LongSword:
                break;
            case WeaponCategory.GreatSword:
                break;
            case WeaponCategory.Club:
                break;
            case WeaponCategory.Hammer:
                break;
            case WeaponCategory.Morningstar:
                break;
            case WeaponCategory.Axe:
                break;
        }

        TriggerSFX(ResourceManager.sfx_sword_draw, point);
    }

    internal static void PlaySound_SheathWeapon(WeaponCategory weaponCategory, Vector3 point)
    {
        switch(weaponCategory)
        {
            case WeaponCategory.Unarmed:
                return;
            case WeaponCategory.Shortbow:
                break;
            case WeaponCategory.Longbow:
                break;
            case WeaponCategory.XBow:
                break;
            case WeaponCategory.Sling:
                break;
            case WeaponCategory.Dart:
                break;
            case WeaponCategory.ThrowingKnife:
                break;
            case WeaponCategory.Blunt:
                break;
            case WeaponCategory.Spiked:
                break;
            case WeaponCategory.Dagger:
                break;
            case WeaponCategory.ShortSword:
                break;
            case WeaponCategory.LongSword:
                break;
            case WeaponCategory.GreatSword:
                break;
            case WeaponCategory.Club:
                break;
            case WeaponCategory.Hammer:
                break;
            case WeaponCategory.Morningstar:
                break;
            case WeaponCategory.Axe:
                break;
        }

        TriggerSFX(ResourceManager.sfx_sword_sheath, point);
    }

    internal static void PlaySound_DrawSpell(Spell spell, Vector3 point)
    {
        
        TriggerSFX(ResourceManager.sfx_spell_draw_default, point);
    }

    internal static void PlaySound_SheathSpell(Spell spell, Vector3 point)
    {

        TriggerSFX(ResourceManager.sfx_spell_draw_default, point);
    }

    public static void PlaySound_OnHit(Vector3 position, DamageType damageType, bool isUnarmed)
    {
        if(isUnarmed)
        {
            TriggerSFX(ResourceManager.sfx_list_unarmedAttackSounds[Random.Range(0, ResourceManager.sfx_list_unarmedAttackSounds.Length)], position);
            return;
        }

        switch(damageType)
        {
            case DamageType.CRUSHING:
            case DamageType.PIERCING:
            case DamageType.SLASHING:
                TriggerSFX(ResourceManager.sfx_list_hits_blade[Random.Range(0, ResourceManager.sfx_list_hits_blade.Length)], position);

                break;
            case DamageType.MISSILE:
                TriggerSFX(ResourceManager.sfx_list_hits_arrow[Random.Range(0, ResourceManager.sfx_list_hits_arrow.Length)], position);
                break;
            case DamageType.MAGICFIRE:
            case DamageType.FIRE:
                TriggerSFX(ResourceManager.sfx_list_hits_fire[Random.Range(0, ResourceManager.sfx_list_hits_fire.Length)], position);
                break;
            case DamageType.MAGICCOLD:
            case DamageType.COLD:
                TriggerSFX(ResourceManager.sfx_list_hits_frost[Random.Range(0, ResourceManager.sfx_list_hits_frost.Length)], position);
                break;
            case DamageType.ELECTRICITY:
                TriggerSFX(ResourceManager.sfx_list_hits_lightning[Random.Range(0, ResourceManager.sfx_list_hits_lightning.Length)], position);
                break;
            //case DamageType.HOLY:
            //    TriggerSFX(ResourceManager.sfx_list_hits_holy[Random.Range(0, ResourceManager.sfx_list_hits_holy.Length)], position);
            //    break;
            //case DamageType.UNHOLY:
            //    TriggerSFX(ResourceManager.sfx_list_hits_unholy[Random.Range(0, ResourceManager.sfx_list_hits_unholy.Length)], position);
                //break;
            case DamageType.POISON:
                TriggerSFX(ResourceManager.sfx_list_hits_poison[Random.Range(0, ResourceManager.sfx_list_hits_poison.Length)], position);
                break;
        }

    }

    public static void PlaySound_OnChargeSpell(Actor actor, Spell spell)
    {
        AudioClip audioClip = spell.magicEffects[0].sfxChargeSpell;
        if(audioClip != null)
            TriggerSFX(audioClip, actor.transform.position);
    }
    
    public static void PlaySound_Swing(Vector3 point, WeaponCategory weaponType)
    {
        switch (weaponType)
        {
            case WeaponCategory.Unarmed:
            case WeaponCategory.Sling:
            case WeaponCategory.Dart:
            case WeaponCategory.ThrowingKnife:
                break;
            case WeaponCategory.Club:
            case WeaponCategory.Hammer:
            case WeaponCategory.Blunt:
            case WeaponCategory.Spiked:
                break;
            case WeaponCategory.Dagger:
            case WeaponCategory.ShortSword:
            case WeaponCategory.LongSword:
                TriggerSFX(ResourceManager.sfx_list_swings_sword[Random.Range(0, ResourceManager.sfx_list_swings_sword.Length)], point);
                break;
            case WeaponCategory.GreatSword:
                break;
            case WeaponCategory.Axe:
                break;
            case WeaponCategory.Spear:
                break;
        }
       
    }
    
    public static void PlaySound_AttackBlocked(Vector3 point)
    {
        TriggerSFX(ResourceManager.sfx_list_blocks_sword[Random.Range(0, ResourceManager.sfx_list_blocks_sword.Length)], point);
    }

    public static AudioSource TriggerSFX(AudioClip audioClip, Vector3 position, int priority = 128)
    {
        GameObject sfxObject = PoolSystem.GetPoolObject("SFXObject", ObjectPoolingCategory.SFX);
        sfxObject.transform.position = position;
        AudioSource audiso = sfxObject.GetComponent<AudioSource>();
        audiso.priority = priority;
        audiso.clip = audioClip;
        audiso.pitch = Random.Range(0.95f, 1.05f);
        audiso.Play();
        CoroutineRunner.Instance?.StartCoroutine(CR_TriggerSFX(audiso, sfxObject));
        return audiso;
    }

    private static IEnumerator CR_TriggerSFX(AudioSource audiso, GameObject sfxObject) {

        yield return new WaitUntil(() => audiso.isPlaying == false);

        sfxObject.SetActive(false);
    }

    public static class ActorSFX
    {
        public static void VerbalConstant(CharacterVoiceSet characterVoiceSet, AudioSource audiosource, VerbalConstantType verbalConstantType, float chance = 1.0f)
        {
            if(Random.value > chance)
            {
                return;
            }

            if(characterVoiceSet == null)
            {
                return;
            }

            switch(verbalConstantType)
            {
                case VerbalConstantType.TAUNT:
                    Say(audiosource, characterVoiceSet.taunt, 127);
                    break;
                case VerbalConstantType.SELECT:
                    Say(audiosource, characterVoiceSet.selected, 127);
                    break;
                case VerbalConstantType.ATTACK:
                    Say(audiosource, characterVoiceSet.attack);
                    break;
                case VerbalConstantType.ATTACKLONG:
                    Say(audiosource, characterVoiceSet.attackLong);
                    break;
                case VerbalConstantType.HURT:
                    Say(audiosource, characterVoiceSet.hurt);
                    break;
                case VerbalConstantType.JUMP:
                    Say(audiosource, characterVoiceSet.jump);
                    break;
                case VerbalConstantType.DEAD:
                    Say(audiosource, characterVoiceSet.death, 126);
                    break;
                case VerbalConstantType.MOVECOMMANDYES:
                    Say(audiosource, characterVoiceSet.moveOrderYes, 127);
                    break;
                case VerbalConstantType.MOVECOMMANDNO:
                    Say(audiosource, characterVoiceSet.moveOrderNo, 127);
                    break;
            }
        }

        private static void Say(AudioSource audiosource, AudioClip[] audioClips, int priority = 128)
        {
            if(audioClips.Length > 0)
            {
                audiosource.priority = priority;
                audiosource.clip = audioClips[Random.Range(0, audioClips.Length)];
                audiosource.pitch = Random.Range(0.95f, 1.05f);
                audiosource.Play();
            }
        }
    }
}
