//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ActorSFXManager
//{

//    public int numVoicesAtOnce = 1;
//    public List<AudioSource> heroVoiceAudioSources;
//    VoiceSetDatabase _voiceSetDatabase;
//    float _cooldownTimer = 0.2f;

//    //void PlaySound_HeroHit(ActorInput self, int damage, EffectType effectType)
//    //{
//    //    //if(audioSource.isPlaying)
//    //    //{
//    //    //    return;
//    //    //}

//    //    //if(Random.value > 0.6f)
//    //    //    return;

//    //    if(effectType != EffectType.HEAL)
//    //    {
//    //        //SayRandomLineFromCategory();
//    //        AudioClip[] audioClipGroup = ResourceManager.voiceSetDatabase.AccessVoiceSetByIndex(self.ActorRecord.gender, self.ActorRecord.voiceSetIndex).hurt;
//    //        if(audioClipGroup.Length > 0)
//    //            TriggerSFX(audioClipGroup[Random.Range(0, audioClipGroup.Length)], self.transform.position);
//    //    }
//    //    //else
//    //    //{
//    //    //}
//    //}

//    //void PlaySound_HeroSelected(ActorInput hero)
//    //{
//    //    foreach(AudioSource asource in heroVoiceAudioSources)
//    //    {
//    //        if(asource.isPlaying == false)
//    //        {
//    //            asource.transform.position = hero.transform.position;
//    //            SayRandomLineFromCategory(_voiceSetDatabase.AccessVoiceSetByIndex(hero.ActorRecord.gender, hero.ActorRecord.voiceSetIndex).selected, hero, asource);
//    //            return;
//    //        }
//    //    }
//    //}

//    //void PlaySound_LevelUp(ActorInput hero, int newLevel)
//    //{
//    //    foreach(AudioSource asource in heroVoiceAudioSources)
//    //    {
//    //        if(asource.isPlaying == false)
//    //        {
//    //            asource.transform.position = hero.transform.position;
//    //            SayRandomLineFromCategory(_voiceSetDatabase.AccessVoiceSetByIndex(hero.ActorRecord.gender, hero.ActorRecord.voiceSetIndex).levelGained, hero, asource);
//    //            return;
//    //        }
//    //    }
//    //}

//    //void PlaySound_GoldReceived(ActorInput hero, int goldGained, int gemsGained)
//    //{
//    //    foreach(AudioSource asource in heroVoiceAudioSources)
//    //    {
//    //        if(asource.isPlaying == false)
//    //        {
//    //            asource.transform.position = hero.transform.position;
//    //            SayRandomLineFromCategory(_voiceSetDatabase.AccessVoiceSetByIndex(hero.ActorRecord.gender, hero.ActorRecord.voiceSetIndex).foundLoot, hero, asource);
//    //            return;
//    //        }
//    //    }
//    //}

//    //void PlaySound_MobDefeated(ActorInput mob, ActorInput hero)
//    //{
//    //    foreach(AudioSource asource in heroVoiceAudioSources)
//    //    {
//    //        if(asource.isPlaying == false)
//    //        {
//    //            asource.transform.position = hero.transform.position;
//    //            SayRandomLineFromCategory(_voiceSetDatabase.AccessVoiceSetByIndex(hero.ActorRecord.gender, hero.ActorRecord.voiceSetIndex).battleWon, hero, asource);
//    //            return;
//    //        }
//    //    }
//    //}

//    public static void PlaySound_WeaponSwing(ActorInput self, WeaponCategory weaponCategory)
//    {
//        AudioClip[] clips = null;
//        switch(weaponCategory)
//        {
//            case WeaponCategory.Unarmed:
//                break;
//            case WeaponCategory.Shortbow:
//                break;
//            case WeaponCategory.Longbow:
//                break;
//            case WeaponCategory.XBow:
//                break;
//            case WeaponCategory.Sling:
//                clips = ResourceManager.sfx_list_swings;
//                break;
//            case WeaponCategory.Dart:
//                break;
//            case WeaponCategory.ThrowingKnife:
//                break;
//            case WeaponCategory.Blunt:
//                break;
//            case WeaponCategory.Spiked:
//                break;
//            case WeaponCategory.Dagger:
//                break;
//            case WeaponCategory.ShortSword:
//                break;
//            case WeaponCategory.LongSword:
//                break;
//            case WeaponCategory.BastardSword:
//                break;
//            case WeaponCategory.GreatSword:
//                break;
//            case WeaponCategory.Club:
//                break;
//            case WeaponCategory.Hammer:
//                break;
//            case WeaponCategory.Flail:
//                break;
//            case WeaponCategory.Morningstar:
//                break;
//            case WeaponCategory.Axe:
//                break;
//            case WeaponCategory.Spear:
//                break;
//            default:
//                break;
//        }

//        if(clips != null && clips.Length > 0)
//        {
//            TriggerSFX(clips[Random.Range(0, clips.Length)], self.transform.position);
//        }

//        //foreach(AudioSource asource in heroVoiceAudioSources)
//        //{
//        //    if(asource.isPlaying == false)
//        //    {
//        //        asource.transform.position = hero.transform.position;
//        //        SayRandomLineFromCategory(_voiceSetDatabase.AccessVoiceSetByIndex(hero.ActorRecord.gender, hero.ActorRecord.voiceSetIndex).battleWon, hero, asource);
//        //        return;
//        //    }
//        //}
//    }

//    public static void RegisterSoundEvents(ActorInput actor, AudioSource as_misc, AudioSource as_chant)
//    {
//        if(actor == null)
//            return;

//        //agent.OnDamageReceived += (source, dmgAmount) => {  };

//        //actor.Combat.OnDeath += (s, a) =>
//        //{
//        //    foreach(AudioSource au in actor.GetComponentsInChildren<AudioSource>())
//        //    {
//        //        //Debug.Log("Disabling audio on corpse");
//        //        au.Stop();
//        //        au.enabled = false;
//        //    }
//        //};

//        //actor.OnPickUpItem += (item, playAnimation, playSound) =>
//        //{
//        //    if(playSound == false || actor == null)
//        //        return;

//        //    TriggerSFX(ResourceManager.sfx_list_dropitem[Random.Range(0, ResourceManager.sfx_list_dropitem.Length)], actor.transform.position);
//        //};

//        //actor.OnDropItem += (item, playAnimation, playSound) =>
//        //{
//        //    if(playSound == false || actor == null)
//        //        return;

//        //    TriggerSFX(ResourceManager.sfx_list_dropitem[Random.Range(0, ResourceManager.sfx_list_dropitem.Length)], actor.transform.position);
//        //};

//        //actor.OnRequestWeaponEquipEffects += (weaponCategory, equipping, playAnimation, playSound) =>
//        //{
//        //    if(playSound == false || actor == null)
//        //        return;

//        //    switch(weaponCategory)
//        //    {
//        //        case WeaponCategory.Longbow:
//        //        case WeaponCategory.Shortbow:
//        //            if(equipping)
//        //            {
//        //                TriggerSFX(ResourceManager.sfx_list_bow_draw[Random.Range(0, ResourceManager.sfx_list_bow_draw.Length)], actor.transform.position);
//        //            }
//        //            else
//        //            {
//        //                TriggerSFX(ResourceManager.sfx_list_bow_holster[Random.Range(0, ResourceManager.sfx_list_bow_holster.Length)], actor.transform.position);
//        //            }

//        //            break;

//        //        //case WeaponType.SwordAndShield:
//        //        case WeaponCategory.ShortSword:
//        //        case WeaponCategory.LongSword:
//        //        case WeaponCategory.BastardSword:
//        //        case WeaponCategory.GreatSword:
//        //            if(equipping)
//        //            {
//        //                TriggerSFX(ResourceManager.sfx_sword_draw, actor.transform.position);
//        //            }
//        //            else
//        //            {
//        //                TriggerSFX(ResourceManager.sfx_sword_sheath, actor.transform.position);
//        //            }
//        //            break;
//        //    }
//        //};

//        //actor.OnArmorUnequipped += (armor, playAnimation, playSound) =>
//        //{
//        //    if(playSound == false || actor == null)
//        //        return;

//        //    switch(armor.armorType)
//        //    {
//        //        case ArmorType.Leather:
//        //            TriggerSFX(ResourceManager.sfx_list_equiplightarmor[Random.Range(0, ResourceManager.sfx_list_equiplightarmor.Length)], actor.transform.position);

//        //            break;

//        //        case ArmorType.Hide:
//        //            TriggerSFX(ResourceManager.sfx_list_equiplightarmor[Random.Range(0, ResourceManager.sfx_list_equiplightarmor.Length)], actor.transform.position);

//        //            break;

//        //        case ArmorType.Chain:
//        //            TriggerSFX(ResourceManager.sfx_list_equipclothing[Random.Range(0, ResourceManager.sfx_list_equipclothing.Length)], actor.transform.position);

//        //            break;
//        //    }
//        //};

//        //actor.OnArmorEquipped += (armor, playAnimation, playSound) =>
//        //{
//        //    if(playSound == false || actor == null)
//        //        return;

//        //    switch(armor.armorType)
//        //    {
//        //        case ArmorType.Leather:
//        //            TriggerSFX(ResourceManager.sfx_list_equiplightarmor[Random.Range(0, ResourceManager.sfx_list_equiplightarmor.Length)], actor.transform.position);

//        //            break;

//        //        case ArmorType.Hide:
//        //            TriggerSFX(ResourceManager.sfx_list_equiplightarmor[Random.Range(0, ResourceManager.sfx_list_equiplightarmor.Length)], actor.transform.position);

//        //            break;

//        //        case ArmorType.Chain:
//        //            TriggerSFX(ResourceManager.sfx_list_equipclothing[Random.Range(0, ResourceManager.sfx_list_equipclothing.Length)], actor.transform.position);

//        //            break;
//        //    }
//        //};

//        //if(actor.ActorRecord.race != Race.ANIMAL && actor.ActorRecord.race != Race.GOBLIN)
//        //{
//        //    actor.OnHandleSpell += (spell, stage, motionIndex) =>
//        //    {
//        //        if(stage == 0)
//        //        {

//        //            switch(spell.effectType)
//        //            {
//        //            //case DamageType.MAGIC:
//        //            //    break;
//        //            case DamageType.FIRE:
//        //                    switch(actor.ActorRecord.gender)
//        //                    {
//        //                        case Gender.Female:
//        //                            TriggerSFX(ResourceManager.sfx_list_spellchants_fire_f[Random.Range(0, ResourceManager.sfx_list_spellchants_fire_f.Length)], actor.transform.position);
//        //                            break;
//        //                        case Gender.Male:
//        //                            TriggerSFX(ResourceManager.sfx_list_spellchants_gibberish_m[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_m.Length)], actor.transform.position);
//        //                            break;
//        //                        case Gender.OTHER:
//        //                            break;
//        //                    }
//        //                    break;
//        //            //case DamageType.COLD:
//        //            //    break;
//        //            //case DamageType.ELECTRICITY:
//        //            //    break;
//        //            //case DamageType.MAGICFIRE:
//        //            //    break;
//        //            case DamageType.HEAL:
//        //                    switch(actor.ActorRecord.gender)
//        //                    {
//        //                        case Gender.Female:
//        //                            TriggerSFX(ResourceManager.sfx_list_spellchants_heal_f[Random.Range(0, ResourceManager.sfx_list_spellchants_heal_f.Length)], actor.transform.position);
//        //                            break;
//        //                        case Gender.Male:
//        //                            TriggerSFX(ResourceManager.sfx_list_spellchants_gibberish_m[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_m.Length)], actor.transform.position);
//        //                            break;
//        //                        case Gender.OTHER:
//        //                            break;
//        //                    }
//        //                    break;
//        //            //case DamageType.POISON:
//        //            //    break;
//        //            default:
//        //                    switch(actor.ActorRecord.gender)
//        //                    {
//        //                        case Gender.Female:
//        //                            TriggerSFX(ResourceManager.sfx_list_spellchants_gibberish_f[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_f.Length)], actor.transform.position);
//        //                            break;
//        //                        case Gender.Male:
//        //                            TriggerSFX(ResourceManager.sfx_list_spellchants_gibberish_m[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_m.Length)], actor.transform.position);
//        //                            break;
//        //                        case Gender.OTHER:
//        //                            break;
//        //                    }
//        //                    break;
//        //            }
//        //        }
//        //    };
//        //}

//        //agent.OnChargeSpell += spell =>
//        //{
//        //AudioClip audioClip = spell.magicEffects[0].sfxCharge;
//        //if(audioClip != null)
//        //    TriggerSFX(audioClip, agent.transform.position);
//        //};

//        //agent.cb_OnTargetBlockedAttack += (wasCriticalAttack) => {
//        //if(wasCriticalAttack)
//        //    PlaySound(audioSource, 10);
//        //else {
//        //    PlaySound(audioSource, 7);
//        //}
//        //};
//    }

//    public static AudioSource TriggerSFX(AudioClip audioClip, Vector3 position, float pitchMin = 0.95f, float pitchMax = 1.05f)
//    {
//        GameObject sfxObject = PoolSystem.GetPoolObject("SFXObject", ObjectPoolingCategory.SFX);
//        sfxObject.transform.position = position;
//        AudioSource audiso = sfxObject.GetComponent<AudioSource>();
//        audiso.clip = audioClip;
//        audiso.pitch = Random.Range(pitchMin, pitchMax);
//        audiso.Play();

//        CoroutineRunner.Instance.StartCoroutine(CR_TriggerSFX(audiso, sfxObject, position));

//        return audiso;
//    }

//    static IEnumerator CR_TriggerSFX(AudioSource audiso, GameObject sfxObject, Vector3 position)
//    {
//        yield return new WaitUntil(() => audiso.isPlaying == false);

//        sfxObject.SetActive(false);
//    }
//}