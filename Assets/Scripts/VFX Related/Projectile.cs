using System;
using UnityEngine;

public enum ProjectileType
{
    Clamp,
    Missile, // Fireball
    Lobber, // Bomb
    Beam, // Lightning Spell
    Spray,
    Arrow,
    MagicMissile
}

public enum ProjectilePathType
{
    Straight,
    Parabolic,
    ParabolicStatic
}

public class Projectile : MonoBehaviour
{
    public AnimationCurve yPath;
    public AnimationCurve xPath;
    public AnimationCurve speedCurve;
    public float lerpTime = 1;
    public float lifeTimeFactor = 1;

    float _lifeTime = 0;
    float _timeToDeath = 10;

    Collider[] _aoeEnemyList;
    RaycastHit[] _beamEnemyList;
    //bool _destroyOnImpact;
    float _arcHeight = 1.3f;
    float _speed;
    Vector3 _startPos;
    Actor target;
    Quaternion _startRotation;

    internal bool ReachedTarget(Vector3 facingPoint, Vector3 targetPosition, float speed)
    {
        transform.rotation = Quaternion.LookRotation(facingPoint - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);

        if(Vector3.Distance(targetPosition, transform.position) < 0.2f)
        {
            return true;
        }

        return false;
    }

    Vector3 _targetPosition;
    public bool traveling;
    public DamageType effectType { get; private set; }
    public SavingThrowType savingThrowType { get; private set; }
    ProjectilePathType _projectilePathType;
    ProjectileType _projectileType;
    DeliveryType _deliveryType;

    public Action OnImpact;
    public Actor owner { get; private set; }
    public Dice damageDice { get; private set; }
    public float aoeRadius { get; private set; }
    public float beamDiameter { get; private set; }
    public float beamRange { get; private set; }

    public bool targetIsGround;
    bool _done;

    public void Launch(Actor owner, Vector3 startPosition, Actor target,
        DeliveryType deliveryType,
        ProjectileType projectileType,
        DamageType effectType,
        float speed, float aoeRadius, float beamDiameter, float beamRange)
    {
        //transform.LookAt(target.transform);

        this.owner = owner;
        transform.position = _startPos = startPosition;

        _lifeTime = 0;
        this.target = target;
        _deliveryType = deliveryType;
        _projectileType = projectileType;
        this.effectType = effectType;
        this.damageDice = damageDice;
        _speed = speed;
        this.aoeRadius = aoeRadius;
        this.beamDiameter = beamDiameter;
        this.beamRange = beamRange;
        _done = false;
        _timeToDeath = 10;

    }

    public void Launch(Actor owner, Vector3 startPosition, Vector3 targetPosition,
        DeliveryType deliveryType,
        ProjectileType projectileType,
        DamageType effectType,
        float speed, float aoeRadius, float beamDiameter, float beamRange)
    {
        this.owner = owner;
        transform.position = _startPos = startPosition;
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
        _targetPosition = targetPosition;
        _lifeTime = 0;
        _deliveryType = deliveryType;
        _projectileType = projectileType;
        this.effectType = effectType;
        this.damageDice = damageDice;
        _speed = speed;
        this.aoeRadius = aoeRadius;
        this.beamDiameter = beamDiameter;
        this.beamRange = beamRange;
        _done = false;
        _timeToDeath = 10;

        //transform.rotation = _startRotation = Quaternion.LookRotation(Vector3.up + owner.transform.right * UnityEngine.Random.Range(0.5f, -0.5f));
    }

    public void StopAndDisable(string debug = "")
    {
        //if(owner.debug && debug != "")
        //    Debug.Log("<color=orange>Disabling proj: </color>" + debug);
        //ResourceManager.

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        _aoeEnemyList = new Collider[5];
        _beamEnemyList = new RaycastHit[5];
    }


    void Update()
    {
        //if(projectileType == ProjectileType.Beam || projectileType == ProjectileType.Spray)
        //    return;

        bool targetAlive = true;

        //if(_projectileTargetLogic != null)
        //{
        //    if(_projectileTargetLogic.Done())
        //        StopAndDisable();
        //    return;
        //}

        _timeToDeath -= Time.deltaTime;
        if(_timeToDeath <= 0)
        {
            StopAndDisable();

            return;
        }


        switch(_deliveryType)
        {
            case DeliveryType.None:
                //Debug.Log("Projectile: None");
                break;
            case DeliveryType.Contact:
                //Debug.Log("Projectile: Contact");
                break;
            case DeliveryType.SeekActor:
                //Debug.Log("Projectile: Seek actor");
                Actor trgt = target;
                targetAlive = trgt != null && trgt.dead == false;
                if(targetAlive)
                {
                    _targetPosition = trgt.Combat.GetAttackVector() /* + Vector3.up * 1*/;

                    if(Vector3.Distance(_targetPosition, transform.position) < 0.2f)
                    {
                        trgt.Combat.ApplyDamage(owner, effectType, damageDice.Roll(), false);
                        //if(_statusEffect.rounds > 0)
                        //{
                        //    trgt.ApplyStatusEffect(_statusEffect, _statusEffect.rounds);
                        //}

                        OnImpact?.Invoke();
                        StopAndDisable("Seek actor -> Target reached");
                    }
                }
                else
                {
                    StopAndDisable("Seek actor -> Target null");
                    //Debug.LogError("Target null");
                }

                switch(_projectileType)
                {
                    case ProjectileType.Clamp:
                        break;
                    case ProjectileType.Missile:
                        HelperFunctions.RotateTo(transform, _targetPosition, 180);
                        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _speed);
                        break;
                    case ProjectileType.Lobber:
                        //TravelArc(_targetPosition);
                        TravelArc(_targetPosition);
                        //transform.position = LaunchVelocity(_targetPosition) * Time.deltaTime * _speed;//BallisticVel(_target.transform.position, 30) * Time.deltaTime * _speed;
                        break;
                    case ProjectileType.Beam:
                        break;
                    case ProjectileType.Spray:
                        break;
                    case ProjectileType.MagicMissile:
                        TravelMM(_targetPosition);
                        break;
                    default:
                        break;
                }


                //

                break;
            case DeliveryType.InstantSelf:
            case DeliveryType.InstantActor:
                if(_done == false)
                {
                    _done = true;
                    //Debug.Log("Projectile: Instant Actor");
                    trgt = (Actor)target;
                    _targetPosition = trgt.transform.position;

                    if(targetAlive)
                    {
                        transform.position = _targetPosition;
                        if(target != null)
                        {
                            trgt.Combat.ApplyDamage(owner, effectType, damageDice.Roll(), false);
                            //if(_statusEffect.rounds > 0)
                            //{
                            //    trgt.ApplyStatusEffect(_statusEffect.statusType, _statusEffect.rounds);
                            //}
                        }
                        OnImpact?.Invoke();

                    }

                }
                //StopAndDisable();

                break;
            case DeliveryType.InstantLocation:
                //Debug.Log("Projectile: Instant location");
                transform.position = _targetPosition;
                OnImpact?.Invoke();
                if(aoeRadius > 0)
                    HandleAOE(aoeRadius);

                StopAndDisable("Instant Location");

                break;
            case DeliveryType.SeekLocation:
                //Debug.Log("Projectile: Seek location");
                if(Vector3.Distance(_targetPosition, transform.position) < 0.2f)
                {
                    OnImpact?.Invoke();
                    if(aoeRadius > 0)
                        HandleAOE(aoeRadius);
                    StopAndDisable("Instant Location -> Target reached");
                }
                HelperFunctions.RotateTo(transform, _targetPosition, 180);
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, Time.deltaTime * _speed);
                //TravelArc(_targetPosition, 0);
                break;
            case DeliveryType.Spray:
                //Debug.Log("Projectile: Beam");

                //if(_lifeTime > 0)
                //{
                if(_done == false)
                {
                    _done = true;
                    Vector3 beamDir = owner.Equipment.spellAnchor.forward;
                    beamDir.y = 0;
                    HandleSprayEffect(beamDir, beamDiameter, beamRange);
                }
                transform.position = owner.Equipment.spellAnchor.position;
                transform.rotation = Quaternion.LookRotation(/*_spellAnchor.position + */owner.transform.forward);
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
                Debug.LogError("Projectile DeliveryType not recognized");
                break;
        }
    }

    void HandleAOE(float aoeRadius)
    {
        Debug.Log(owner.GetName() + ": Handling AOE");
        int numHits = Physics.OverlapSphereNonAlloc(transform.position, aoeRadius, _aoeEnemyList, 1 << LayerMask.NameToLayer("Actors"));

        for(int i = 0; i < numHits; i++)
        {
            Actor aoeTarget = _aoeEnemyList[i].GetComponent<Actor>();

            if(aoeTarget == target)
            {
                continue;
            }

            aoeTarget.Combat.ApplyDamage(owner, effectType, damageDice.Roll(), false);
            //if(_statusEffect.rounds > 0)
            //{
            //    aoeTarget.ApplyStatusEffect(_statusEffect.statusType, _statusEffect.rounds);
            //}
        }
    }

    void HandleSprayEffect(Vector3 direction, float diameter, float range)
    {
        Debug.Log(owner.GetName() + ": Handling Beam");
        int numHits = Physics.SphereCastNonAlloc(transform.position, diameter, direction.normalized, _beamEnemyList, range, 1 << LayerMask.NameToLayer("Actors"));

        for(int i = 0; i < numHits; i++)
        {
            Actor beamTarget = _beamEnemyList[i].collider.GetComponent<Actor>();

            if(beamTarget == owner) // A mage casting burnings hands shall never ignite himself
                continue;

            if(beamTarget == target)
            {
                continue;
            }

            beamTarget.Combat.ApplyDamage(owner, effectType, damageDice.Roll(), false);
            //if(_statusEffect.rounds > 0)
            //{
            //    beamTarget.ApplyStatusEffect(_statusEffect.statusType, _statusEffect.rounds);
            //}
        }
    }

    Vector3 CircularMod()
    {
        float circleSpeed = 1f;
        float forwardSpeed = .1f; // Assuming negative Z is towards the camera
        float circleSize = 2f;
        //var circleGrowSpeed = 0.1;
        float xPos = Mathf.Sin(Time.time * circleSpeed) * circleSize;
        float yPos = Mathf.Cos(Time.time * circleSpeed) * circleSize;
        return new Vector3(xPos, yPos, forwardSpeed);
    }

    Vector3 nextPos;
    void TravelArc(Vector3 targetPoint)
    {
        float dist = (targetPoint - _startPos).magnitude;
        Vector3 nextZ = Vector3.MoveTowards(transform.position, targetPoint, _speed * Time.deltaTime);
        float baseY = Mathf.Lerp(_startPos.y, targetPoint.y, (nextZ - _startPos).magnitude / dist);
        float arc = (_arcHeight) * ((nextZ.z - _startPos.z) * (nextZ.z - _targetPosition.z) + (nextZ.x - _startPos.x) * (nextZ.x - _targetPosition.x)) / (-0.2f * dist * dist);
        nextPos = new Vector3(nextZ.x, baseY + arc, nextZ.z);

        transform.LookAt(nextPos - transform.position);
        transform.position = nextPos;
    }

    void TravelMM(Vector3 targetPoint)
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
}