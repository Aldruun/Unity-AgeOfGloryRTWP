//using LuxURPEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellTargetType
{
    All,
    Friend,
    Foe
}

public class SpellTargetProjector : MonoBehaviour
{
    private SpellTargetType _spellTargetType;
    private Actor _caster;
    public float radius { get; private set; }
    //SphereCollider _sphereCollider;
    private List<Actor> _targetMap; // target, distance
    private Transform _projectorDecal;

    public void Init()
    {
           _projectorDecal = transform.Find("Projector");
        //_sphereCollider = _projectorDecal.GetComponent<SphereCollider>();
        _targetMap = new List<Actor>();
        Toggle(false);
    }

    private void OnTriggerStay(Collider other)
    {
        Actor actorHit = other.GetComponent<Actor>();
        if(actorHit == null || actorHit.dead)
        {
            Debug.Log("<color=orange>Nothing hit</color>");
            return;
        }

        switch(_spellTargetType)
        {
            case SpellTargetType.All:
                if(actorHit is NPCInput)
                {
                    if(_targetMap.Contains(actorHit) == false)
                    {
                        actorHit.ActorUI.Highlight();
                        _targetMap.Add(actorHit);
                    }

                }
                break;
            case SpellTargetType.Friend:
                if(actorHit is NPCInput nc)
                {
                    if(_targetMap.Contains(nc) == false)
                    {
                        nc.ActorUI.Highlight();
                        _targetMap.Add(nc);
                    }

                }
                break;
            case SpellTargetType.Foe:
                if(actorHit is NPCInput mc)
                {
                    if(_targetMap.Contains(mc) == false)
                    {
                        mc.ActorUI.Highlight();
                        _targetMap.Add(mc);
                    }

                }
                break;
            //case SpellTargetType.Self:
            //    if(actorHit == _caster)
            //    {
            //        _caster.Highlight();
            //        _targetMap.Add(_caster);
            //    }
            //        break;
            default:
                break;
        }
        Debug.Log("<color=orange>Target indicator hit</color>");

    }

    private void OnTriggerExit(Collider other)
    {
        Actor actorHit = other.GetComponent<Actor>();
        if(_targetMap.Contains(actorHit))
        {
            actorHit.ActorUI.Unhighlight();
            _targetMap.Remove(actorHit);
        }
    }

    private void Update()
    {
        foreach(var actor in _targetMap)
        {
            if(actor == null)
            {
                _targetMap.Remove(actor);
            }
            else if(actor.dead)
            {
                actor.ActorUI.Unhighlight();
                _targetMap.Remove(actor);
            }
        }
    }

    private void TargetFriends()
    {

    }

    private void TargetFoes()
    {

    }

    private void TargetAll()
    {

    }
    public void Toggle(bool on)
    {
        _projectorDecal.gameObject.SetActive(on);
    }
    public void SetAoE(Actor caster, float radius, SpellTargetType spellTargetType)
    {
        _caster = caster;
        _spellTargetType = spellTargetType;
        //this.radius = _sphereCollider.radius = radius;
        _targetMap = new List<Actor>();
        _projectorDecal.transform.localScale = new Vector3(radius, 1, radius);
        _projectorDecal.gameObject.SetActive(true);
    }
    public void SetBeam(Actor caster, float length, float outerRadius, SpellTargetType spellTargetType)
    {
        _caster = caster;
        _spellTargetType = spellTargetType;
        //CapsuleCollider capsColl = transform.Find("Collider").GetComponent<CapsuleCollider>();
        //capsColl.radius = radius;
        //capsColl.height = length;

        _targetMap = new List<Actor>();
        _projectorDecal.transform.localScale = new Vector3(outerRadius, 1, length);
        _projectorDecal.gameObject.SetActive(true);
    }
}
