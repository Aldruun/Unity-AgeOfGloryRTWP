//using UnityEngine;

//public class ActorMonoObject : ScriptableMonoObject
//{
//    [SerializeField] ActorInput _actor;
//    //public NavMeshAgent navAgent;
//    //public Animator animator;
//    //TODO Good place to get and set mono components
   
//    AudioSource audiosource_misc;
//    AudioSource audiosource_chant;

//    internal override ScriptableMonoObject SetScriptable(Scriptable actor)
//    {
//        audiosource_misc = transform.Find("Audio/AS Misc").GetComponent<AudioSource>();
//        audiosource_chant = transform.Find("Audio/AS Voice").GetComponent<AudioSource>();


//        _actor = (ActorInput)actor;
//        VFXManager.RegisterVFXEvents(_actor);
//        RegisterSoundEvents(_actor);
//        //navAgent = GetComponent<NavMeshAgent>();
//        //animator = GetComponent<Animator>();
//        //_scriptable.SetScriptable(this);
//        return this;
//    }

//    internal override Scriptable GetScriptable()
//    {
//        return _actor;
//    }

//    //void OnCollisionEnter(Collision collision)
//    //{
//    //    if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Actors"))
//    //    {
//    //        ActorMonoObject actorObject = collision.collider.GetComponent<ActorMonoObject>();
//    //        ActorInput actor = (ActorInput)actorObject.GetScriptable();
//    //        //if(actor.navAgent.hasPath)
//    //        //{
//    //        Debug.Log("COLLIDING");
//    //            actor.SetDestination(transform.position + transform.right, 0.1f);
//    //        //}
//    //    }
//    //}

//    //! For animator
//    // This method is called by the attack animations
//    public virtual void Attack(int handIndex)
//    {
//        if(_actor.isCasting)
//        {
//            return;
//        }

//        //Debug.Log(gameObject.name + ": Attacking");

//        if(_actor.animator.GetCurrentAnimatorStateInfo(1).IsName("New State") == true)
//        {
//            return;
//        }

//        if(_actor.Combat.GetHostileTarget() == null)
//        {
//            Debug.LogError("Cannot attack: Attack target = null");
//            return;
//        }

//        if(_actor == _actor.Combat.GetHostileTarget())
//            Debug.Log($"<color=red>{_actor.GetName()}</color>: I'm attacking myself");

//        if(_actor.GetEquippedWeapon() == null)
//        {
//            if(_actor.debug)
//                Debug.Log(_actor.GetName() + ": Applying damage failed: Equipped right hand weapon = null");
//        }
//        else
//        {
//            Weapon weapon = _actor.GetEquippedWeapon();
//            if(weapon.projectileIdentifier != null)
//            {
                
//                GameObject vfxObj = PoolSystem.GetPoolObject(weapon.projectileIdentifier, ObjectPoolingCategory.VFX);
              
//                Projectile _projectile = vfxObj.GetComponent<Projectile>();

//                if(_projectile == null)
//                    Debug.LogError("ActorMonoObject: Projectile visual not found");

//                _projectile.Launch(null, _actor, _actor.Equipment.m_weaponHand.position, _actor.Combat.GetHostileTarget(), SpellTargetType.Foe, DeliveryType.SeekActor, ProjectileType.Lobber, 10, 5,0,0);;
//            }
//            else if(_actor.Combat.GetHostileTarget() is ActorInput a)
//            {
//                a.ApplyDamage(_actor, _actor.GetEquippedWeapon().damageType, ImpactType.Blade, _actor.GetEquippedWeapon().DamageRoll(), false);
//            }
//            SFXPlayer.PlaySound_WeaponSwing(_actor, _actor.Combat.GetEquippedWeapon().weaponCategory);
//            //Debug.Log(agentData.Name + ": Applying damage");
//        }
  

//        //Debug.Log("4. (Combat) " + agentData.Name + ": Hitting enemy");
//    }

//    //! For animator
//    public void DrawWeapon()
//    {
//        _actor.Equipment.DrawEquippedWeapon();
//    }

//    public void SheathWeapon()
//    {
//        _actor.Equipment.HolsterEquippedWeapon();
//    }

//    public void ReleaseSpell() // 0 = left, 1 = right
//    {
//        _actor.OnReleaseSkill?.Invoke();
//    }

//    public void WeaponEvent(int index)
//    {
//        switch(index)
//        {
//            // Bow
//            //case 1: // Hand on quiver
//            //case 2: // Nocking arrow
//            //case 3: // Fully drawn
//            case 4: // Release
//                _actor.OnHandleBow?.Invoke(index);
//                break;
//        }
//    }

//    void TriggerSFX(AudioSource audioSource, AudioClip audioClip, float pitchMin = 0.95f, float pitchMax = 1.05f)
//    {
//        audioSource.clip = audioClip;
//        audioSource.pitch = Random.Range(pitchMin, pitchMax);
//        audioSource.Play();
//    }

//    public void RegisterSoundEvents(ActorInput actor)
//    {
//        actor.OnHit += (aggressor, damageAmount, effectType, successfulHit) =>
//        {
//            //if(actor.blocking)
//            //{
//            //    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_blocks_sword[Random.Range(0, ResourceManager.sfx_list_blocks_sword.Length)], actor.transform.position);
//            //    return;
//            //}

//            if(successfulHit == false)
//            {
//                TriggerSFX(audiosource_misc, ResourceManager.sfx_list_swings[Random.Range(0, ResourceManager.sfx_list_swings.Length)]);
//                return;
//            }

//            switch(effectType)
//            {
//                case DamageType.PIERCING:
//                case DamageType.SLASHING:
//                case DamageType.CRUSHING:
//                    if(successfulHit)
//                        TriggerSFX(audiosource_misc, ResourceManager.sfx_list_hits_creature[Random.Range(0, ResourceManager.sfx_list_hits_creature.Length)]);
//                    break;
//                //case WeaponImpactType.Blade:
//                //if(successfulHit)
//                //    TriggerSFX(ResourceManager.sfx_list_hits_blade[Random.Range(0, ResourceManager.sfx_list_hits_blade.Length)], agent.transform.position);
//                //break;

//                case DamageType.MISSILE:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_hits_arrow[Random.Range(0, ResourceManager.sfx_list_hits_arrow.Length)]);
//                    break;
//                case DamageType.MAGICFIRE:
//                case DamageType.FIRE:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_hits_fire[Random.Range(0, ResourceManager.sfx_list_hits_fire.Length)]);
//                    break;
//                case DamageType.MAGICCOLD:
//                case DamageType.COLD:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_hits_frost[Random.Range(0, ResourceManager.sfx_list_hits_frost.Length)]);
//                    break;

//                case DamageType.ELECTRICITY:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_hits_lightning[Random.Range(0, ResourceManager.sfx_list_hits_lightning.Length)]);
//                    break;
//                case DamageType.MAGIC:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_hits_magic[Random.Range(0, ResourceManager.sfx_list_hits_magic.Length)], 0.7f, 1.3f);
//                    break;
//                //case EffectType.Holy:
//                //    TriggerSFX(ResourceManager.sfx_list_hits_holy[Random.Range(0, ResourceManager.sfx_list_hits_holy.Length)], agent.transform.position);
//                //    break;

//                //case EffectType.Unholy:
//                //    TriggerSFX(ResourceManager.sfx_list_hits_unholy[Random.Range(0, ResourceManager.sfx_list_hits_unholy.Length)], agent.transform.position);
//                //    break;

//                case DamageType.POISON:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_hits_poison[Random.Range(0, ResourceManager.sfx_list_hits_poison.Length)]);
//                    break;

//                default:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_unarmedAttackSounds[Random.Range(0, ResourceManager.sfx_list_unarmedAttackSounds.Length)]);
//                    break;
//            }

//        };

//        //actor.OnHandleSpell += (spell, stage, motionIndex) =>
//        //{
//        //    if(stage == 0)
//        //    {
//        //        switch(spell.effectType)
//        //        {
//        //            //case DamageType.MAGIC:
//        //            //    break;
//        //            case DamageType.FIRE:
//        //                switch(actor.ActorRecord.gender)
//        //                {
//        //                    case Gender.Female:
//        //                        TriggerSFX(audiosource_chant, ResourceManager.sfx_list_spellchants_fire_f[Random.Range(0, ResourceManager.sfx_list_spellchants_fire_f.Length)]);
//        //                        break;
//        //                    case Gender.Male:
//        //                        TriggerSFX(audiosource_chant, ResourceManager.sfx_list_spellchants_gibberish_m[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_m.Length)]);
//        //                        break;
//        //                    case Gender.OTHER:
//        //                        break;
//        //                }
//        //                break;
//        //            //case DamageType.COLD:
//        //            //    break;
//        //            //case DamageType.ELECTRICITY:
//        //            //    break;
//        //            //case DamageType.MAGICFIRE:
//        //            //    break;
//        //            case DamageType.HEAL:
//        //                switch(actor.ActorRecord.gender)
//        //                {
//        //                    case Gender.Female:
//        //                        TriggerSFX(audiosource_chant, ResourceManager.sfx_list_spellchants_heal_f[Random.Range(0, ResourceManager.sfx_list_spellchants_heal_f.Length)]);
//        //                        break;
//        //                    case Gender.Male:
//        //                        TriggerSFX(audiosource_chant, ResourceManager.sfx_list_spellchants_gibberish_m[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_m.Length)]);
//        //                        break;
//        //                    case Gender.OTHER:
//        //                        break;
//        //                }
//        //                break;
//        //            //case DamageType.POISON:
//        //            //    break;
//        //            default:
//        //                switch(actor.ActorRecord.gender)
//        //                {
//        //                    case Gender.Female:
//        //                        TriggerSFX(audiosource_chant, ResourceManager.sfx_list_spellchants_gibberish_f[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_f.Length)]);
//        //                        break;
//        //                    case Gender.Male:
//        //                        TriggerSFX(audiosource_chant, ResourceManager.sfx_list_spellchants_gibberish_m[Random.Range(0, ResourceManager.sfx_list_spellchants_gibberish_m.Length)]);
//        //                        break;
//        //                    case Gender.OTHER:
//        //                        break;
//        //                }
//        //                break;
//        //        }
//        //    }
//        //};
//        //agent.OnDamageReceived += (source, dmgAmount) => {  };

//        actor.Combat.OnDeath += (s, a) =>
//        {

//            //Debug.Log("Disabling audio on corpse");
//            audiosource_misc.Stop();
//            audiosource_chant.Stop();

//        };

//        //actor.OnPickUpItem += (item, playAnimation, playSound) =>
//        //{
//        //    if(playSound == false || actor == null)
//        //        return;

//        //    TriggerSFX(ResourceManager.sfx_list_dropitem[Random.Range(0, ResourceManager.sfx_list_dropitem.Length)], transform.position);
//        //};

//        //actor.OnDropItem += (item, playAnimation, playSound) =>
//        //{
//        //    if(playSound == false || actor == null)
//        //        return;

//        //    TriggerSFX(ResourceManager.sfx_list_dropitem[Random.Range(0, ResourceManager.sfx_list_dropitem.Length)], transform.position);
//        //};

//        actor.OnRequestWeaponEquipEffects += (weaponCategory, equipping, playAnimation, playSound) =>
//        {
//            if(playSound == false)
//                return;

//            switch(weaponCategory)
//            {
//                case WeaponCategory.Longbow:
//                case WeaponCategory.Shortbow:
//                    if(equipping)
//                    {
//                        TriggerSFX(audiosource_misc, ResourceManager.sfx_list_bow_draw[Random.Range(0, ResourceManager.sfx_list_bow_draw.Length)]);
//                    }
//                    else
//                    {
//                        TriggerSFX(audiosource_misc, ResourceManager.sfx_list_bow_holster[Random.Range(0, ResourceManager.sfx_list_bow_holster.Length)]);
//                    }

//                    break;

//                //case WeaponType.SwordAndShield:
//                case WeaponCategory.ShortSword:
//                case WeaponCategory.LongSword:
//                case WeaponCategory.BastardSword:
//                case WeaponCategory.GreatSword:
//                    if(equipping)
//                    {
//                        TriggerSFX(audiosource_misc, ResourceManager.sfx_sword_draw);
//                    }
//                    else
//                    {
//                        TriggerSFX(audiosource_misc, ResourceManager.sfx_sword_sheath);
//                    }
//                    break;
//            }
//        };

//        actor.OnArmorUnequipped += (armor, playAnimation, playSound) =>
//        {
//            if(playSound == false || actor == null)
//                return;

//            switch(armor.armorType)
//            {
//                case ArmorType.Leather:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_equiplightarmor[Random.Range(0, ResourceManager.sfx_list_equiplightarmor.Length)]);

//                    break;

//                case ArmorType.Hide:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_equiplightarmor[Random.Range(0, ResourceManager.sfx_list_equiplightarmor.Length)]);

//                    break;

//                case ArmorType.Chain:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_equipclothing[Random.Range(0, ResourceManager.sfx_list_equipclothing.Length)]);

//                    break;
//            }
//        };

//        actor.OnArmorEquipped += (armor, playAnimation, playSound) =>
//        {
//            if(playSound == false || actor == null)
//                return;

//            switch(armor.armorType)
//            {
//                case ArmorType.Leather:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_equiplightarmor[Random.Range(0, ResourceManager.sfx_list_equiplightarmor.Length)]);

//                    break;

//                case ArmorType.Hide:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_equiplightarmor[Random.Range(0, ResourceManager.sfx_list_equiplightarmor.Length)]);

//                    break;

//                case ArmorType.Chain:
//                    TriggerSFX(audiosource_misc, ResourceManager.sfx_list_equipclothing[Random.Range(0, ResourceManager.sfx_list_equipclothing.Length)]);

//                    break;
//            }
//        };

//        //actor.OnHandleSpell += (spell, stage, motionIndex) =>
//        //{
//        //    if(stage == 0)
//        //    {

//        //    }
//        //};

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

//}
