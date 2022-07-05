//using UnityEngine;

//[CreateAssetMenu(fileName = "Skill_ProjectileDamage", menuName = "ScriptableObjects/Skills/ProjectileDamageSkill")]
//public class Skill_Projectile_Damage : Skill
//{
//    public bool keepGrounded;
//    public bool destroyOnImpact = true;
//    //public ImpactType impactType;
//    public ProjectileType ProjectileType;
//    public Vector3 vfxStartOffset;
//    public Vector3 vfxTargetOffset;
//    [Tooltip("Make the projectile spawn at the height of the hand spell anchor.")]
//    public bool launchFromHandAnchor = true;
//    public bool isMagic;
//    public float baseSpeed = 5;
//    public float baseDamage = 10;
//    public float damagePerSecond;
//    public int dpsDuration;
//    public float aoeRadius;
//    public AudioClip[] sfxlist_impact;

//    private bool homing;
//    [SerializeField] private float beamDiameter;
//    [SerializeField] private float beamRange;

//    public override void Init()
//    {
//        base.Init();
//    }

//    public override bool ConditionsMetAI(NPCInput agent)
//    {
//        //if(agent.aiControlled)
//        //{
//        //    skillTarget = HelperFunctions.GetClosestActor_WithJobs(agent, 20, agent.GetEnemyFlags());
//        //}
//        //else
//            skillTarget = agent.Combat.GetHostileTarget();
//        return /*agent.weaponDrawn && */skillTarget != null;
//    }

//    public override bool ConditionsMetPlayer(Actor actor)
//    {
//        return true;
//    }

//    public override void IndividualSetup(Actor self)
//    {
//    }

//    public override void SpawnVFX(Actor self, Actor target, Vector3 targetPosition)
//    {
//        GameObject vfxObj = PoolSystem.GetPoolObject(vfxIdentifier, ObjectPoolingCategory.VFX);
//        vfxObj.transform.position = vfxPoint.position;
//        VFXPlayer.TriggerVFX(vfxObj, vfxPoint.position, Quaternion.identity);

//        Projectile projectile = vfxObj.GetComponent<Projectile>();

//        projectile.OnImpact = () =>
//        {
//            Debug.Log("__________________Impact");

//            if(sfxlist_impact.Length > 0) 
//                SFXPlayer.TriggerSFX(sfxlist_impact[UnityEngine.Random.Range(0, sfxlist_impact.Length)], target != null ? target.transform.position : targetPosition);
//            if(vfx_impact_identifier != "")
//            {
//                Debug.Log("______________Impact VFX");
//                GameObject startVFX = PoolSystem.GetPoolObject(vfx_impact_identifier, ObjectPoolingCategory.VFX);

//                VFXPlayer.TriggerVFX(startVFX, projectile.transform.position, Quaternion.identity, 1);
               
//            }
//        };

//        Debug.Log("________________Launching projectile");
//        Vector3 startTransformPoint = self.transform.TransformPoint(vfxStartOffset);
//        Vector3 startPoint = launchFromHandAnchor ? startTransformPoint + Vector3.up * vfxPoint.position.y : startTransformPoint;
//        Vector3 targetDir = (target != null ? target.Combat.GetAttackPoint().position : targetPosition) - startPoint;
//        Vector3 targetPoint = Vector3.zero;

//        projectile.transform.position = startPoint;
//        bool instant = false;
//        switch(DeliveryType)
//        {
//            case DeliveryType.None:
//                Debug.LogError("Skill '" + skillName + "' has no delivery type");
//                break;
//            case DeliveryType.Contact:
//                targetPoint = target.transform.position;
//                break;
//            case DeliveryType.SeekActor: // Always homing
//                homing = true;
//                targetPoint = startPoint + targetDir * 999;
//                break;
//            case DeliveryType.SeekLocation:
//                targetPoint = targetPosition;
//                break;
//            case DeliveryType.InstantSelf:
//                targetPoint = self.transform.position;
//                instant = true;
//                break;
//            case DeliveryType.InstantActor:
//                targetPoint = target.transform.position;
//                instant = true;
//                break;
//            case DeliveryType.InstantLocation:
//                targetPoint = targetPosition;
//                instant = true;
//                break;
//            case DeliveryType.Spray:
//                //startPoint = self.transform.position;
//                //projectile.transform.rotation = self.transform.rotation;
//                //break;
//            case DeliveryType.Beam:
//                //startPoint = self.transform.position;
//                targetPoint = startPoint + self.transform.forward;
//                projectile.transform.rotation = self.transform.rotation;
//                break;
//        }
//        if(targetPoint != Vector3.zero)
//        {
//            if(instant)
//            {
//                projectile.ActivateAtPoint(self, targetPoint, Quaternion.identity, ActorMeshEffectType, baseDamage, aoeRadius);
//            }
//            else
//            {
//                projectile.LaunchStraight(startPoint, self, targetPoint, ActorMeshEffectType, DeliveryType, destroyOnImpact, baseSpeed, baseDamage, aoeRadius, beamDiameter, activationRange, keepGrounded);
//            }
            
//        }
//        //else
//        //    projectile.Launch(null, self, startPoint, targetPoint, SpellTargetType, DeliveryType, ProjectileType, baseSpeed, aoeRadius, beamDiameter, activationRange);

//        //if(homing)
//        //    projectile.LaunchStraightHoming(self, target, damageType, impactType, destroyOnImpact, baseSpeed, baseDamage, beamDiameter, beamRange);
//        //else
//        //{
               
//        //    projectile.LaunchStraight(startPoint, self, targetPoint, damageType, impactType, deliveryType, destroyOnImpact, baseSpeed, baseDamage, aoeRadius, beamDiameter, beamRange);

//        //} 
      
//    }
//}

