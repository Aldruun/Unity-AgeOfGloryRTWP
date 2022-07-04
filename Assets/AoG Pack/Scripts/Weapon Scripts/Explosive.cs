//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GenericFunctions;

//public class Explosive : Weapon {

//    float damageRadius = 4;
//    float detonationTimer = 3;

//    public float h = 25;
//    public float gravity = -18;

//    public bool debugPath;

//    Rigidbody _rigidbody;

//    void Awake() {

//        _rigidbody = this.ForceGetComponent<Rigidbody>();
//    }

//    public void Throw(Agent agent, Vector3 targetPosition) {

//        base.OnDraw();

//        Physics.gravity = Vector3.up * gravity;
//        _rigidbody.isKinematic = false;
//        _rigidbody.useGravity = true;
//        _rigidbody.velocity = CalculateLaunchData(targetPosition).initialVelocity;

//        StartCoroutine(DetonationCountdown(agent));

//        if(debugPath) {

//            DrawPath(targetPosition);
//        }
//    }

//    IEnumerator DetonationCountdown(Agent agent) {

//        yield return new WaitForSeconds(detonationTimer);

//        ApplyAreaDamage(agent);

//        Create.OneShotSFX(attackSounds[Random.Range(0, attackSounds.Length)], transform.position, attackSoundVolume, Random.Range(0.85f, 1.15f));
//        TriggerAttackVFX(transform.position, transform.rotation, null);

//        Destroy(gameObject);
//    }

//    public void SetDetonationTimer(float timer) {

//        detonationTimer = timer;
//    }

//    void ApplyAreaDamage(Agent agent) {

//        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, damageRadius);

//        foreach(Collider col in objectsInRange) {

//            IDestroyable target = col.GetComponent<IDestroyable>();

//            if(target != null) {

//                float proximity = (transform.position - col.transform.position).magnitude;
//                float effect = 1 - (proximity / damageRadius);

//                target.ApplyDamage(agent, m_rawDamageOutput * effect);

//                Rigidbody rb = col.GetComponent<Rigidbody>();

//                if(rb != null) {

//                    if(target.m_destroyed) {

//                        rb.isKinematic = false;
//                        rb.AddExplosionForce(effect, transform.position, damageRadius);
//                    }

//                }
//            }
//        }


//    }

//    LaunchData CalculateLaunchData(Vector3 targetPosition) {

//        float displacementY = targetPosition.y - _rigidbody.position.y;
//        Vector3 displacementXZ = new Vector3(targetPosition.x - _rigidbody.position.x, 0, targetPosition.z - _rigidbody.position.z);
//        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);
//        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
//        Vector3 velocityXZ = displacementXZ / time;

//        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
//    }

//    void DrawPath(Vector3 targetPosition) {

//        LaunchData launchData = CalculateLaunchData(targetPosition);
//        Vector3 previousDrawPoint = _rigidbody.position;

//        int resolution = 30;
//        for(int i = 1; i <= resolution; i++) {
//            float simulationTime = i / (float)resolution * launchData.timeToTarget;
//            Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
//            Vector3 drawPoint = _rigidbody.position + displacement;
//            UnityEngine.Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
//            previousDrawPoint = drawPoint;
//        }
//    }

//    struct LaunchData {
//        public readonly Vector3 initialVelocity;
//        public readonly float timeToTarget;

//        public LaunchData(Vector3 initialVelocity, float timeToTarget) {
//            this.initialVelocity = initialVelocity;
//            this.timeToTarget = timeToTarget;
//        }

//    }
//}

