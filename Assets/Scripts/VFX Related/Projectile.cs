using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using Random = UnityEngine.Random;
using AoG.Core;

public enum ProjectileType
{
    Missile, // Fireball
    Lobber, // Bomb
    Beam, // Lightning Spell
    Spray,
    Arrow,
    MagicMissile
}

public enum ImpactType
{
    Blade
}

public class Projectile : MonoBehaviour
{
    private bool traveling;

    private bool _done;
    //public DamageType damageType { get; private set; }
    private DeliveryType DeliveryType;
    private ActorMeshEffectType ActorMeshEffectType;
    //private ImpactType impactType;
    public Action OnImpact;
    public ActorInput owner { get; private set; }
    public float damage { get; private set; }
    public float aoeRadius { get; private set; }
    public float beamDiameter { get; private set; }
    public float beamRange { get; private set; }
    public ProjectileType ProjectileType { get; private set; }
    public float lerpTime = 1;
    public float lifeTimeFactor = 1;
    
    private bool debugSkillAOEProjectile;
    private bool debugSkillBeamProjectile;

    private Collider[] _aoeEnemyList;
    private RaycastHit[] _beamEnemyList;
    private bool destroyOnImpact;
    private readonly float _evadeChance;
    private float _firingAngle = 45.0f;
    private readonly float _gravity = 9.8f;
    private bool _homing;
    private bool ignoreGroundCollision;
    private float _lifeTime = 5;
    private float _speed;
    private Vector3 _startPos;
    private ActorInput target;
    private Vector3 _targetPosition;
    private float lifeTime = 5;

    public void Init(ProjectileType projectileType)
    {
        this.ProjectileType = projectileType;
    }

    public void StopAndDisable(string debug = "")
    {
        //if(owner.debug && debug != "")
        //    Debug.Log("<color=orange>Disabling proj: </color>" + debug);
        //ResourceManager.

        gameObject.SetActive(false);
    }

    private void Awake()
    {
        _aoeEnemyList = new Collider[5];
        _beamEnemyList = new RaycastHit[5];

    }

    private void Start()
    {
        debugSkillAOEProjectile = GameInterface.Instance.DatabaseService.GameSettings.DebugSkillAOEProjectile;
        debugSkillBeamProjectile = GameInterface.Instance.DatabaseService.GameSettings.DebugSkillBeamProjectile;
    }

    private void Update()
    {
        //if(_projectileTargetLogic != null)
        //{
        //    if(_projectileTargetLogic.Done())
        //        StopAndDisable();
        //    return;
        //}

        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            StopAndDisable();

            return;
        }

        switch(DeliveryType)
        {
            case DeliveryType.None:
                //Debug.Log("Projectile: None");
                break;
            case DeliveryType.Contact:
                //Debug.Log("Projectile: Contact");
                break;
            case DeliveryType.SeekActor:
              
            case DeliveryType.SeekLocation:
                //Debug.Log("Projectile: Seek location");
                //if(Vector3.Distance(_targetPosition, transform.position) < 0.2f)
                //{
                //    OnImpact?.Invoke();
                //    if(aoeRadius > 0)
                //        HandleAOE(aoeRadius);

                //    if(destroyOnImpact)
                //        StopAndDisable("Instant Location -> Target reached");
                //}
                
                switch(ProjectileType)
                {
                    case ProjectileType.Arrow:
                    case ProjectileType.Missile:
                        HelperFunctions.RotateTo(transform, _targetPosition, 180);
                        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _speed);
                        break;
                    case ProjectileType.Lobber:
                        TravelArc(_targetPosition);
                        break;
                    case ProjectileType.Beam:
                        break;
                    case ProjectileType.Spray:
                        break;
                    case ProjectileType.MagicMissile:
                        TravelMM(_targetPosition);
                        break;
                }
                
                break;
            case DeliveryType.InstantSelf:
            case DeliveryType.InstantActor:
            case DeliveryType.InstantLocation:
                break;
            case DeliveryType.Spray:
                //Debug.Log("Projectile: Beam");

                //if(_lifeTime > 0)
                //{
                if(_done == false)
                {
                    _done = true;
                    Vector3 beamDir = owner.transform.forward;
                    beamDir.y = 0;
                    HandleSprayEffect(beamDir, beamDiameter, beamRange);
                }
                if(owner != null)
                {
                    transform.position = owner.Equipment.spellAnchor.position;
                    transform.rotation = Quaternion.LookRotation(/*_spellAnchor.position + */owner.transform.forward); 
                }
                //}
                //else
                //{
                //    StopAndDisable();
                //}
                break;
            case DeliveryType.Beam:
                //Debug.Log("Projectile: Beam");


                //}
                //else
                //{
                //    StopAndDisable();
                //}
                break;

            default:
                Debug.LogError("Projectile DeliveryType '<color=white>" + DeliveryType + "</color>' not recognized");
                break;
        }
    }

    public void UpdateStatic(Vector3 startPosition, Vector3 direction)
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void ActivateAtPoint(ActorInput owner,
        Vector3 targetPosition, Quaternion targetRotation, ActorMeshEffectType effectType,
        float damage, float aoeRadius)
    {
        transform.rotation = targetRotation;
        transform.position = targetPosition;
        this.owner = owner;
        this.damage = damage;
        this.aoeRadius = aoeRadius;
        ActorMeshEffectType = effectType;
        DeliveryType = DeliveryType.InstantLocation;
        _homing = false;
        //_targetPosition = targetPosition;
        //_startPos = transform.position;
        lifeTime = 10;

        Impact();
        if(aoeRadius > 0)
            HandleAOE(aoeRadius);
        //else
        //    aoeTarget.Combat.ApplyDamage(owner, null, null, false);

        if(destroyOnImpact)
            StopAndDisable("Instant Location");
    }

    public void LaunchStraight(Vector3 startPosition, ActorInput owner,
        Vector3 targetPosition, ActorMeshEffectType effectType, DeliveryType deliveryType, bool destroyOnImpact, float speed,
        float damage, float aoeRadius, float beamDiameter, float beamRange, bool ignoreGroundCollision)
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
        //this.damageType = damageType;
        this.destroyOnImpact = destroyOnImpact;
        this.owner = owner;
        _speed = speed;
        this.damage = damage;
        this.aoeRadius = aoeRadius;
        ActorMeshEffectType = effectType;
        ProjectileType = ProjectileType.Missile;
        DeliveryType = deliveryType;
        _homing = false;
        _targetPosition = targetPosition;
        //_startPos = transform.position;
        lifeTime = 10;
        this.beamDiameter = beamDiameter;
        this.beamRange = beamRange;
        this.ignoreGroundCollision = ignoreGroundCollision;
    }

    
    public void LaunchWithArc(Vector3 startPosition, ActorInput owner,
        Vector3 targetPosition, ActorMeshEffectType effectType, DeliveryType deliveryType, bool destroyOnImpact, float speed,
        float damage, float aoeRadius, float launchAngle)
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
        //this.damageType = damageType;
        this.destroyOnImpact = destroyOnImpact;
        this.owner = owner;
        _speed = speed;
        this.damage = damage;
        this.aoeRadius = aoeRadius;
        ActorMeshEffectType = effectType;
        ProjectileType = ProjectileType.Lobber;
        DeliveryType = deliveryType;
        _homing = false;
        _targetPosition = targetPosition;
        //_startPos = transform.position;
        lifeTime = 10;
    }
    
    private float _arcHeight = 1.3f;
    private Vector3 nextPos;

    private void TravelArc(Vector3 targetPoint)
    {
        float dist = (targetPoint - _startPos).magnitude;
        Vector3 nextZ = Vector3.MoveTowards(transform.position, targetPoint, _speed * Time.deltaTime);
        float baseY = Mathf.Lerp(_startPos.y, targetPoint.y, (nextZ - _startPos).magnitude / dist);
        float arc = (_arcHeight) * ((nextZ.z - _startPos.z) * (nextZ.z - _targetPosition.z) + (nextZ.x - _startPos.x) * (nextZ.x - _targetPosition.x)) / (-0.2f * dist * dist);
        nextPos = new Vector3(nextZ.x, baseY + arc, nextZ.z);

        transform.LookAt(nextPos - transform.position);
        transform.position = nextPos;
    }

    private void TravelMM(Vector3 targetPoint)
    {
        Vector3 desiredProjDir = (targetPoint - transform.position);
        //float startTgtDist = (targetPoint - _startPos).magnitude;
        float tgtDist = (targetPoint - transform.position).magnitude;

        _lifeTime += Time.deltaTime * lifeTimeFactor;
        float linearT = (_lifeTime / lerpTime) * 4f;
        Quaternion targetRot = Quaternion.LookRotation(desiredProjDir);

        if(tgtDist > 1)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, linearT);
        }
        else
        {
            transform.LookAt(targetPoint);
        }
        transform.position += transform.forward * Time.deltaTime * _speed;

    }

    private void OnTriggerEnter(Collider collision)
    {
        //bool targetAlive = _target != null && _target.dead == false;
        if(collision.gameObject.layer == LayerMask.NameToLayer("Actors") && (owner != null))
        {
            //Debug.Log("### hit");
            target = collision.gameObject.GetComponent<ActorInput>();

            if(target.gameObject == owner.gameObject)
            {
                return;
            }

            if(aoeRadius > 0)
                HandleAOE(aoeRadius);
            else
                target.Combat.ApplyDamage(owner, null, null, false);
         
            Impact();
            if(GameInterface.Instance.DatabaseService.GameSettings.DebugProjectileOnTrigger)
                Debug.Log(owner.GetName() + ":<color=orange>__PROJECTILE (" + gameObject.name + ") HIT 1</color> " + collision.gameObject.name);
            if(destroyOnImpact)
                StopAndDisable();
        }
        else if(/*(collision.gameObject.layer == LayerMask.NameToLayer("Actors") && collision.GetComponent<ActorInput>().gameObject != owner.gameObject) ||*/
            collision.gameObject.layer == LayerMask.NameToLayer("Obstacles") ||
            (ignoreGroundCollision == false && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            /*|| collision.gameObject.layer == LayerMask.NameToLayer("Projectiles")*/)
        {
            //Debug.Log("### hit object");

            if(aoeRadius > 0)
                HandleAOE(aoeRadius);
            if(GameInterface.Instance.DatabaseService.GameSettings.DebugProjectileOnTrigger)
                Debug.Log(owner.GetName() + ": <color=orange>__PROJECTILE (" + gameObject.name + ") HIT 2</color> " + collision.gameObject.name);

            //GameObject _fakeArrowObject = ItemDatabase.InstantiatePhysicalItem("iron arrow", collision.transform);
            //_fakeArrowObject.transform.position = transform.position;
            //_fakeArrowObject.transform.rotation = transform.rotation;
            //_fakeArrowObject.AddComponent<GarbageFader>();

            Impact();

            StopAndDisable();
        }
    }

    private void HandleSprayEffect(Vector3 direction, float diameter, float range)
    {
        Debug.Log(owner.GetName() + ": Handling Beam Projectile (" + gameObject.name + ")");
        int numHits = Physics.SphereCastNonAlloc(transform.position, diameter, direction.normalized, _beamEnemyList, range, 1 << LayerMask.NameToLayer("Actors"));

        for(int i = 0; i < numHits; i++)
        {
            ActorInput beamTarget = _beamEnemyList[i].collider.GetComponent<ActorInput>();

            if(beamTarget == owner) // A mage casting burnings hands shall never ignite himself
                continue;

            if(beamTarget == target)
            {
                continue;
            }

            if(ActorMeshEffectType != ActorMeshEffectType.None)
                VFXPlayer.TriggerActorVFX(beamTarget, ActorMeshEffectType, 3);

            beamTarget.Combat.ApplyDamage(owner, null, null, false);
            //if(_statusEffect.rounds > 0)
            //{
            //    beamTarget.Execute_ApplyStatusEffect(_statusEffect.statusType, _statusEffect.rounds);
            //}
        }
    }

    private void HandleAOE(float aoeRadius)
    {
        Debug.Log(owner.GetName() + ": <color=orange>Handling AOE Projectile (" + gameObject.name + ") with radius</color>: " + aoeRadius);
        int numHits = Physics.OverlapSphereNonAlloc(transform.position, aoeRadius, _aoeEnemyList, 1 << LayerMask.NameToLayer("Actors"));

        for(int i = 0; i < numHits; i++)
        {
            ActorInput aoeTarget = _aoeEnemyList[i].GetComponent<ActorInput>();

            if(aoeTarget == owner)
            {
                continue;
            }

            if(ActorMeshEffectType != ActorMeshEffectType.None)
                VFXPlayer.TriggerActorVFX(aoeTarget, ActorMeshEffectType, 3);

            aoeTarget.Combat.ApplyDamage(owner, null, null, false);
            //if(_statusEffect.rounds > 0)
            //{
            //    aoeTarget.Execute_ApplyStatusEffect(_statusEffect.statusType, _statusEffect.rounds);
            //}
        }
    }

    private void Impact()
    {
        OnImpact?.Invoke();
    }

    //Vector3 CircularMod()
    //{
    //    int circleSpeed = 5;
    //    int forwardSpeed = 1; // Assuming negative Z is towards the camera
    //    float circleSize = 1.5f;
    //    //var circleGrowSpeed = 0.1;
    //    float xPos = Mathf.Sin(Time.time * circleSpeed) * circleSize;
    //    float yPos = Mathf.Cos(Time.time * circleSpeed) * circleSize;

    //    //circleSize += circleGrowSpeed;
    //    return new Vector3(xPos, yPos, forwardSpeed);

    //    //var circleSpeed = 1;
    //    //var forwardSpeed = -1; // Assuming negative Z is towards the camera
    //    //var circleSize = 1;
    //    //var circleGrowSpeed = 0.1;

    //    //var xPos = Mathf.Sin(Time.time * circleSpeed) * circleSize;
    //    //var yPos = Mathf.Cos(Time.time * circleSpeed) * circleSize;
    //    //var zPos += forwardSpeed * Time.deltaTime;

    //    //circleSize += circleGrowSpeed;
    //}

    //float SinMod()
    //{
    //    return Mathf.Sin(Time.time * 1) * 2;
    //}

    //IEnumerator SimulateProjectile()
    //{
    //    // Calculate distance to target
    //    float target_Distance = Vector3.Distance(transform.position, _targetPosition);

    //    // Calculate the velocity needed to throw the object to the target at specified angle.
    //    float projectile_Velocity = target_Distance / (Mathf.Sin(2 * _firingAngle * Mathf.Deg2Rad) / _gravity);

    //    // Extract the X  Y componenent of the velocity
    //    float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(_firingAngle * Mathf.Deg2Rad);
    //    float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(_firingAngle * Mathf.Deg2Rad);

    //    // Calculate flight time.
    //    float flightDuration = target_Distance / Vx;

    //    // Rotate projectile to face the target.
    //    transform.rotation = Quaternion.LookRotation(_targetPosition - transform.position);

    //    float elapse_time = 0;

    //    while(_lifeTime > 0)
    //    {
    //        transform.Translate(0, (Vy - _gravity * elapse_time) * Time.deltaTime, Vx * Time.deltaTime);

    //        elapse_time += Time.deltaTime;

    //        yield return null;
    //    }
    //}
}