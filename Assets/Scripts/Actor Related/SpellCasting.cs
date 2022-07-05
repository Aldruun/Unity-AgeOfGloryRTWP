//using System;
//using UnityEngine;
//using UnityEngine.Profiling;

//public static class SpellCasting
//{
//    /// <summary>
//    /// NOT an "Every Frame" function
//    /// </summary>
//    /// <param name="spell"></param>
//    /// <param name="caster"></param>
//    /// <param name="target"></param>
//    /// <returns></returns>
//    public static bool InflictMagicEffectsAtActor(Spell spell, Actor caster, Actor target)
//    {
//        if(caster == null || caster.dead)
//        {
//            return false;
//        }

//        if(target == null || target.dead)
//        {
//            return false;
//        }

//        ApplyVFXToActor(caster, target, spell.magicEffectsData[0], 0.5f);
//        foreach(EffectData eff in spell.magicEffectsData)
//        {
//            target.ApplyStatusEffect(eff.statusEffect, eff.Rounds);
//        }

//        //ActorUtility.ModifyActorMagicka(caster.ActorRecord, -spell.cost, ModType.ADDITIVE);

//        return true;
//    }

//    public static void CastSpraySpellInDirection(Spell spell, Actor caster, Vector3 targetPoint, float duration)
//    {
//        if(caster == null || caster.dead)
//        {
//            return;
//        }
//        Vector3 dir = targetPoint - caster.Equipment.spellAnchor.position;
//        //if(_projectile == null)
//        //{
//        GameObject vfxObj = PoolSystem.GetPoolObject(spell.magicEffectsData[0].vfxid_projectile, ObjectPoolingCategory.VFX);
//        //Debug.Log(spellAnchor.name);
//        VFXPlayer.TriggerVFX(vfxObj, caster.Equipment.spellAnchor, Vector3.zero, Quaternion.LookRotation(dir), false, duration);

//        Projectile projectile = vfxObj.GetComponent<Projectile>();
//        EffectData effectData = spell.magicEffectsData[0];
//        if(projectile != null)
//        {
//            //projectile.Init(effectData, ProjectileType.Missile);
//            projectile.OnImpact = () =>
//            {
//                if(effectData.sfxOnHit != null)
//                    SFXPlayer.TriggerSFX(effectData.sfxOnHit[UnityEngine.Random.Range(0, effectData.sfxOnHit.Length)], projectile.transform.position);
//                //if(id_VFXOnHit != "")
//                //{
//                //    VFXManager.TriggerVFX(id_VFXOnHit);
//                //}
//                //if(duration > 1)
//                //    target.Execute_ApplyStatusEffect(new StatusEffect_Damage(1, false, 1, duration, 1));
//            };
//            if(projectile == null)
//            {
//                if(caster.debugAnimation)
//                    Debug.LogError(caster.GetName() + ": '<color=yellow>" + ProjectileType.Missile + "</color>' Projectile\n" +
//                        "Delivery Type:  '<color=yellow>" + effectData.deliveryType + "</color>'\n" +
//                        "Casting Type:  '<color=yellow>" + effectData.castingType + "</color>'");
//            }
//            //Debug.LogError("No projectile component on '" + vfxObj.name + "'");
//        }
//        //ActorUtility.ModifyActorMagicka(caster.ActorRecord, -spell.cost, ModType.ADDITIVE);
//    }

//    public static void LaunchMagicProjectile(Spell spell, Actor caster, Vector3 targetLocation)
//    {
//        if(caster == null || caster.dead)
//        {
//            return;
//        }

//        //if(_projectile == null)
//        //{
//        GameObject vfxObj = PoolSystem.GetPoolObject(spell.magicEffectsData[0].vfxid_projectile, ObjectPoolingCategory.VFX);
//        //Debug.Log(spellAnchor.name);
//        VFXPlayer.TriggerVFX(vfxObj, caster.Equipment.spellAnchor.position, Quaternion.LookRotation(caster.transform.forward));

//        Projectile projectile = vfxObj.GetComponent<Projectile>();
//        EffectData effectData = spell.magicEffectsData[0];
//        if(projectile != null)
//        {
//            //projectile.Init(effectData, ProjectileType.Missile);
//            projectile.OnImpact = () =>
//            {
//                if(effectData.sfxOnHit != null)
//                    SFXPlayer.TriggerSFX(effectData.sfxOnHit[UnityEngine.Random.Range(0, effectData.sfxOnHit.Length)], projectile.transform.position);
//                //if(id_VFXOnHit != "")
//                //{
//                //    VFXManager.TriggerVFX(id_VFXOnHit);
//                //}
//                //if(duration > 1)
//                //    target.Execute_ApplyStatusEffect(new StatusEffect_Damage(1, false, 1, duration, 1));
//            };
//            if(projectile == null)
//            {
//                if(caster.debugAnimation)
//                    Debug.LogError(caster.GetName() + ": '<color=yellow>" + ProjectileType.Missile + "</color>' Projectile\n" +
//                        "Delivery Type:  '<color=yellow>" + effectData.deliveryType + "</color>'\n" +
//                        "Casting Type:  '<color=yellow>" + effectData.castingType + "</color>'");
//            }
//            //Debug.LogError("No projectile component on '" + vfxObj.name + "'");
//        }
//        //ActorUtility.ModifyActorMagicka(caster.ActorRecord, -spell.cost, ModType.ADDITIVE);
//        projectile.LaunchStraight(caster.Equipment.spellAnchor.position, caster, targetLocation, ActorMeshEffectType.None, spell.deliveryType, true, 12, spell.magicEffectsData[0].magnitude, effectData.aoeRadius, spell.GetEffectDiameter(), spell.activationRange, false);
//    }

//    //static void ApplyEffectToActor(Actor caster, Actor target, EffectData effectData)
//    //{
//    //    target.ApplyEffect(effectData);
//    //}

//    private static void ApplyVFXToActor(Actor caster, Actor target, EffectData effectData, float delay)
//    {
//        GameObject vfxObj = PoolSystem.GetPoolObject(effectData.vfxid_projectile, ObjectPoolingCategory.VFX);
//        //Debug.Log(spellAnchor.name);
//        VFXPlayer.TriggerVFX(vfxObj, target.transform, Vector3.zero, Quaternion.identity, false, effectData.Rounds);
//    }

//    public static bool SpellOnCooldown(Spell spell)
//    {
//        if(spell.cooldownTimer > 0)
//        {
//            return true;
//        }

//        return false;
//    }

//    public static bool SpellCostCovered(Spell spell, Actor caster)
//    {

//        //if(ActorUtility.GetModdedAttribute(caster.ActorRecord, ActorStats.MAGICKA) < spell.cost)
//        //{
//        //    return false;
//        //}

//        return true;
//    }

//    public static bool CanCastSpell(Spell spell, Actor caster)
//    {
//        if(SpellCostCovered(spell, caster) == false)
//        {
//            if(caster.debugAnimation)
//                Debug.Log(caster.GetName() + ": Spell '<color=white>" + spell.Name + "</color>' activation failed -> Not enough mana");
//            return false;
//        }

//        if(SpellOnCooldown(spell))
//        {
//            if(caster.debugAnimation)
//                Debug.Log(caster.GetName() + ": Spell '<color=white>" + spell.Name + "</color>' activation failed -> On cooldown");
//            return false;
//        }

//        return true;
//    }

//    public static Spell ChooseBestSpell(Actor caster, Actor target)
//    {
//        if(caster == null || caster.dead)
//        {
//            return null;
//        }

//        if(target == null || target.dead)
//        {
//            return null;
//        }

//        bool hostile = target.ActorStats.IsEnemy(caster.ActorStats);

//        Spell highestPrioritySpell = null;
//        float highestRating = 0;

//        for(int i = 0; i < caster.GetSpells().Count; i++)
//        {
//            Spell currSpell = caster.GetSpells()[i];

//            if(caster.debugAnimation)
//                Debug.Log(caster.GetName() + ":<color=cyan>ITERATING SPELL </color>'" + currSpell.Name + "'");

//            if(SpellOnCooldown(currSpell))
//            {
//                continue;
//            }
//            if(SpellCostCovered(currSpell, caster) == false)
//            {
//                continue;
//            }

//            float currSpellRating = GetTotalEffectsRating(currSpell, caster, target, hostile);

//            if(highestPrioritySpell == null || currSpellRating > highestRating)
//            {
//                highestRating = currSpellRating;
//                highestPrioritySpell = currSpell;
//            }
//        }

//        if(highestPrioritySpell == null)
//        {
//            if(caster.debugAnimation)
//                Debug.Log(caster.GetName() + ": No spell found");
//        }
//        else
//        {
//            if(caster.debugAnimation)
//                Debug.Log(caster.GetName() + ": Chose " + highestPrioritySpell.spellType.ToString() + " '<color=white>" + highestPrioritySpell.Name + "</color>'\n" +
//                "Delivery Type:  '<color=white>" + highestPrioritySpell.deliveryType.ToString() + "</color>'\n" +
//                "Casting Type:  '<color=white>" + highestPrioritySpell.castingType.ToString() + "</color>'\n" +
//                "Equip Type:  '<color=white>" + highestPrioritySpell.equipType.ToString() + "</color>'");
//        }

//        Profiler.EndSample();

//        if(highestPrioritySpell != null)
//        {
//            return highestPrioritySpell;
//        }

//        return null;
//    }

//    public static Spell ChooseBestSpellFromSchool(Actor caster, Actor target, MagicSchool[] targetSchools)
//    {
//        if(caster == null || caster.dead)
//        {
//            return null;
//        }

//        if(target == null || target.dead)
//        {
//            return null;
//        }

//        Spell highestPrioritySpell = null;
//        float highestRating = 0;

//        for(int i = 0; i < caster.GetSpells().Count; i++)
//        {
//            Spell currSpell = caster.GetSpells()[i];

//            if(Array.IndexOf(targetSchools, currSpell.magicSchool) == -1)
//            {
//                continue;
//            }

//            if(caster.debugAnimation)
//                Debug.Log(caster.GetName() + ":<color=cyan>ITERATING SPELL </color>'" + currSpell.Name + "'");
//            if(SpellCostCovered(currSpell, caster) == false || SpellOnCooldown(currSpell))
//            {
//                continue;
//            }

//            float currSpellRating = GetTotalEffectsRating(currSpell, caster, target, false);
//            if(currSpellRating == 0)
//                continue;

//            if(highestPrioritySpell == null || currSpellRating > highestRating)
//            {
//                highestRating = currSpellRating;
//                highestPrioritySpell = currSpell;
//            }
//        }

//        if(highestPrioritySpell == null)
//        {
//            if(caster.debugAnimation)
//                Debug.Log(caster.GetName() + ": No spell found");
//        }
//        else
//        {
//            if(caster.debugAnimation)
//                Debug.Log(caster.GetName() + ": Chose " + highestPrioritySpell.spellType.ToString() + " '<color=white>" + highestPrioritySpell.Name + "</color>'\n" +
//                "Delivery Type:  '<color=white>" + highestPrioritySpell.deliveryType.ToString() + "</color>'\n" +
//                "Casting Type:  '<color=white>" + highestPrioritySpell.castingType.ToString() + "</color>'\n" +
//                "Equip Type:  '<color=white>" + highestPrioritySpell.equipType.ToString() + "</color>'");
//        }

//        Profiler.EndSample();

//        if(highestPrioritySpell != null)
//        {
//            return highestPrioritySpell;
//        }

//        return null;
//    }

//    public static Spell ChooseAnySpellWithKeyword(Actor caster, Keyword keywords)
//    {
//        if(caster == null || caster.dead)
//        {
//            return null;
//        }

//        if(caster.debugAnimation)
//            Debug.Log(caster.GetName() + ":<color=cyan> Searching for any spell with keywords </color>'" + keywords + "'");

//        for(int i = 0; i < caster.GetSpells().Count; i++)
//        {
//            Spell currSpell = caster.GetSpells()[i];

//            foreach(EffectData effect in currSpell.magicEffectsData)
//            {
//                if(effect.keywordFlags.HasFlag(keywords))
//                {

//                }
//            }

//            if(SpellCostCovered(currSpell, caster) == false || SpellOnCooldown(currSpell))
//            {
//                continue;
//            }


//        }

//        return null;
//    }

//    private static float GetTotalEffectsRating(Spell spell, Actor caster, Actor target, bool hostile)
//    {
//        //foreach(MagicEffect effect in spell.magicEffects)
//        //{

           
//        //        switch(effect.statusEffect)
//        //        {
//        //            case Effects.Soultrap:
//        //            case Effects.AlmsiviIntervention:
//        //            case Effects.DivineIntervention:
//        //            case Effects.CalmHumanoid:
//        //            case Effects.CalmCreature:
//        //            case Effects.FrenzyHumanoid:
//        //            case Effects.FrenzyCreature:
//        //            case Effects.DemoralizeHumanoid:
//        //            case Effects.DemoralizeCreature:
//        //            case Effects.RallyHumanoid:
//        //            case Effects.RallyCreature:
//        //            case Effects.Charm:
//        //            case Effects.DetectAnimal:
//        //            case Effects.DetectEnchantment:
//        //            case Effects.DetectKey:
//        //            case Effects.Telekinesis:
//        //            case Effects.Mark:
//        //            case Effects.Recall:
//        //            case Effects.Jump:
//        //            case Effects.WaterBreathing:
//        //            case Effects.SwiftSwim:
//        //            case Effects.WaterWalking:
//        //            case Effects.SlowFall:
//        //            case Effects.Lock:
//        //            case Effects.Open:
//        //            case Effects.TurnUndead:
//        //            case Effects.WeaknessToCommonDisease:
//        //            case Effects.WeaknessToBlightDisease:
//        //            case Effects.WeaknessToCorprusDisease:
//        //            case Effects.CureCommonDisease:
//        //            case Effects.CureBlightDisease:
//        //            case Effects.CureCorprusDisease:
//        //            case Effects.ResistBlightDisease:
//        //            case Effects.ResistCommonDisease:
//        //            case Effects.ResistCorprusDisease:
//        //            case Effects.Invisibility:
//        //            case Effects.Chameleon:
//        //            case Effects.NightEye:
//        //            case Effects.Vampirism:
//        //            case Effects.StuntedMagicka:
//        //            case Effects.ExtraSpell:
//        //            case Effects.RemoveCurse:
//        //            case Effects.CommandCreature:
//        //            case Effects.CommandHumanoid:
//        //                return 0f;

//        //            case Effects.Light:
//        //                if(HelperFunctions.GetCurrentLightLevel() < 0.2f)
//        //                {
//        //                    return 1000;
//        //                }
//        //                return 0;

//        //            case Effects.RestoreHealth:

//        //                if(effect.deliveryType == DeliveryType.InstantSelf)
//        //                {
//        //                    if(ActorUtility.GetHealthPercentage(caster.ActorStats) < 0.4f)
//        //                    {
//        //                        return 500;
//        //                    } 
//        //                }

//        //                return 0;

//        //                //case Effects.
//        //        }
//        //    //}
//        //    //else
//        //    //{
//        //        switch(effect.statusEffect)
//        //        {
//        //            case Effects.FrostDamage:
//        //            case Effects.FireDamage:
//        //            case Effects.ShockDamage:
//        //            case Effects.DamageHealth:
//        //            case Effects.DamageAttribute:
//        //            case Effects.DamageFatigue:
//        //                return 1;
//        //        }
//        //        //TODO
//        //        //return 0;
//        //    //}
//        //}

//        return 0;
//    }

//    public static bool CanCast(Actor input)
//    {
//        if(input.dead)
//        {
//            if(input.debugAnimation)
//                Debug.Log(input.GetName() + ":<color=yellow> Can't cast -> dead</color>");
//            return false;
//        }
//        if(input.Animation.Animator.GetCurrentAnimatorStateInfo(2).IsName("New State") == false)
//        {
//            if(input.debugAnimation)
//                Debug.Log(input.GetName() + ":<color=yellow> Can't cast -> Still in cast animation</color>");
//            return false;
//        }
//        if(input.isCasting)
//        {
//            if(input.debugAnimation)
//                Debug.Log(input.GetName() + ":<color=yellow> Can't cast -> Already casting</color>");
//            return false;
//        }
//        if(input.isDowned)
//        {
//            if(input.debugAnimation)
//                Debug.Log(input.GetName() + ":<color=yellow> Can't cast -> Is downed</color>");
//            return false;
//        }
//        return true;
//    }

//}
