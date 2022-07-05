using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionDamage : MonoBehaviour
{
    private Projectile _projectile;
    public List<ParticleCollisionEvent> collisionEvents;
    public ParticleSystem part;

    private void Start()
    {
        _projectile = GetComponentInParent<Projectile>();
        part = GetComponentInChildren<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("<color=orange>ParticleCollision called</color>");
        if (_projectile.owner.transform.root != other.transform.root &&
            other.GetComponentInParent<Actor>())
        {
            var target = other.GetComponentInParent<Actor>();
            var source = _projectile.owner;

            var numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            //Rigidbody rb = other.GetComponent<Rigidbody>();
            var i = 0;

            while (i < numCollisionEvents)
            {
                //if(rb)
                //{
                //    Vector3 pos = collisionEvents[i].intersection;
                //    Vector3 force = collisionEvents[i].velocity * 10;
                //    rb.AddForce(force);
                //}
                i++;
                Debug.Log("<color=orange>Hit</color>");
                //target.ApplyDamage(source, _projectile.effectType, true, source.isSpellCaster, _projectile.damage,
                //    false, true);
            }
        }
    }
}