using GenericFunctions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Actions
{
    //public static void RotateTo(Transform t, Vector3 targetPosition)
    //{
    //	Vector3 dir = targetPosition - t.position;

    //	if(dir == Vector3.zero)
    //		return;

    //	Quaternion targetRot = Quaternion.LookRotation(dir);
    //	targetRot.x = 0;
    //	targetRot.z = 0;

    //	t.rotation = targetRot;
    //}

    public static bool MoveIntoRange(NPCInput self, Vector3 point, float range, bool requiresInFOV = true)
    {
        Debug.Assert(self != null);
        Debug.Assert(point != Vector3.zero);
        //Debug.Assert(range > 0);

        Vector3 selfPos = self.transform.position;
        Vector3 targetPos = point;
        Vector3 targetDir = targetPos - selfPos;

        bool hasLOS = HelperFunctions.LineOfSightH2H(self.transform, targetPos, 1.5f);
        bool inFOV = requiresInFOV ? Get.IsInFOV(self.transform, targetPos, 5) : true;
        bool inRange = targetDir.magnitude <= range;

        self.Unroot();

        if(hasLOS == false)
        {
            Debug.DrawLine(self.transform.position + Vector3.up * 0.3f, point, Color.red);
            HelperFunctions.RotateTo(self.transform, self.NavAgent.steeringTarget, 300/*, self.GetName() + ": MoveIntoRange: <color=orange>Rotating to path (NoLOS)</color>"*/);

            self.SetDestination(targetPos, 1f);
        }
        else if(inRange == false)
        {
            Debug.DrawLine(self.transform.position + Vector3.up * 0.3f, point, Color.yellow);
            HelperFunctions.RotateTo(self.transform, self.NavAgent.steeringTarget, 300/*, self.GetName() + ": MoveIntoRange: <color=orange>Rotating to path (NotInRange)</color>"*/);

            self.SetDestination(targetPos, Mathf.Clamp(range - 0.1f, 0, 100));
        }
        else
        {
            Debug.DrawLine(self.transform.position + Vector3.up * 0.3f, point, Color.green);
            self.HoldPosition();
            if(inFOV == false)
            {
                HelperFunctions.RotateTo(self.transform, targetPos, 300/*, self.GetName() + ": MoveIntoRange: <color=orange>Rotating to target (InFOV)</color>"*/);
            }

            if(self.debugAnimation)
                Debug.Log(self.GetName() + "<color=cyan>: # Can attack</color>");
        }
        return hasLOS && inRange && inFOV;
    }

    public static bool MoveIntoRange(NPCInput self, ActorInput target, float range)
    {
        Debug.Assert(self != null);
        Debug.Assert(target != null);
        Debug.Assert(range > 0);

        Vector3 selfPos = self.transform.position;
        Vector3 targetPos = target.transform.position;
        Vector3 targetDir = targetPos - selfPos;

        bool hasLOS = HelperFunctions.LineOfSightH2H(self.transform, targetPos, 1.5f);
        bool inFOV = Get.IsInFOV(self.transform, targetPos, 5);
        float desiredDistToTarget = (range) + (target.GetCharacterRadius() / 2) /*+ ctrl.agent.GetCharacterRadius()*/;
        bool inWeaponRange = targetDir.magnitude <= desiredDistToTarget;

        self.Unroot();

        if(hasLOS == false)
        {
            Debug.DrawLine(self.transform.position + Vector3.up * 0.3f, target.transform.position, Color.red);
            HelperFunctions.RotateTo(self.transform, self.NavAgent.steeringTarget, 300);

            self.SetDestination(targetPos, 1f);
        }
        else if(inWeaponRange == false)
        {
            Debug.DrawLine(self.transform.position + Vector3.up * 0.3f, target.transform.position, Color.yellow);
            HelperFunctions.RotateTo(self.transform, self.NavAgent.steeringTarget, 300);

            self.SetDestination(targetPos, desiredDistToTarget - 0.1f, "");
        }
        else
        {
            Debug.DrawLine(self.transform.position + Vector3.up * 0.3f, target.transform.position, Color.green);
            self.HoldPosition();
            if(inFOV == false)
            {
                HelperFunctions.RotateTo(self.transform, targetPos, 300);
            }

            if(self.debugAnimation)
                Debug.Log(self.GetName() + "<color=cyan>: # Can attack</color>");
        }
        return hasLOS && inWeaponRange && inFOV;
    }

    //public static bool MoveIntoSpellRange(ActorInput self, ActorInput target, Spell spell)
    //{
    //	Debug.Assert(self != null);
    //	Debug.Assert(target != null);
    //	Debug.Assert(spell != null);

    //	Vector3 selfPos = self.transform.position;
    //	Vector3 targetPos = target.transform.position;
    //	Vector3 targetDir = targetPos - selfPos;

    //	bool hasLOS = HelperFunctions.LineOfSightH2H(self.transform, targetPos, 1.5f);
    //	bool inFOV = Get.IsInFOV(self.transform, targetPos, 5);
    //	float desiredDistToTarget = (spell.activationRange) + (target.GetCharacterRadius() / 2) /*+ ctrl.agent.GetCharacterRadius()*/;
    //	bool inSpellRange = targetDir.magnitude <= desiredDistToTarget;

    //	self.Unroot();

    //	if(hasLOS == false)
    //	{
    //		DebugExtension.DebugPoint(self.transform.position + Vector3.up * 2.6f, Colors.Red);
    //		//Debug.DrawLine(self.transform.position + Vector3.up * 1.5f, target.transform.position, Color.red);
    //		ActorMotion.Rotate(self, self.navAgent.steeringTarget, 500);

    //		self.SetDestination(targetPos, 1f);
    //	}
    //	else if(inSpellRange == false)
    //	{
    //		DebugExtension.DebugWireSphere(self.transform.position + Vector3.up * 2.6f, Colors.Red, .2f);
    //		//Debug.DrawLine(self.transform.position + Vector3.up * 1.5f, target.transform.position, Color.yellow);
    //		ActorMotion.Rotate(self, self.navAgent.steeringTarget, 500);

    //		self.SetDestination(targetPos, /*desiredDistToTarget - */0.1f);
    //	}
    //	else
    //	{
    //		DebugExtension.DebugWireSphere(self.transform.position + Vector3.up * 2.6f, Colors.Green, .2f);
    //		Debug.DrawLine(self.transform.position + Vector3.up * 1.5f, target.transform.position, Color.green);
    //		self.Hold();
    //		if(inFOV == false)
    //		{
    //			DebugExtension.DebugCone(self.transform.position + Vector3.up * 2.6f, Vector3.up, Colors.Red, 25f);
    //			ActorMotion.RotateTo(self, targetPos);
    //			return false;
    //		}

    //		if(self.debug)
    //			Debug.Log(self.ActorRecord.Name + "<color=cyan>: # Can cast</color>");
    //	}
    //	return hasLOS && inSpellRange && inFOV;
    //}

    public static Vector3 GetBestPositionOnFoe(ActorInput self, Vector3 foePosition, int posCount, Collider[] _attackPosHits)
    {
        List<Vector3> availablePositions = new List<Vector3>();
        int radius = 4;
        for(int i = 0; i < posCount; i++)
        {

            //summon the enemies around this central GameObject
            float radian = i * Mathf.PI / (posCount / 2);
            Vector3 standPosition = foePosition + new Vector3(radius * Mathf.Cos(radian), foePosition.y, radius * Mathf.Sin(radian));

            int hits = Physics.OverlapSphereNonAlloc(standPosition, 2, _attackPosHits, 1 << LayerMask.NameToLayer("Actors"));

            if(hits == 0)
            {
                availablePositions.Add(standPosition);
            }
            else
            {
                for(int h = 0; h < hits; h++)
                {
                    ActorInput actor = _attackPosHits[h].GetComponent<ActorInput>();
                    if(actor == self || actor.dead)
                    {
                        availablePositions.Add(standPosition);
                    }
                }
            }
        }

        //for(int i = 0; i < availablePositions.Count; i++)
        //{
        //    if(true)
        //    {

        //    }
        //}
        if(availablePositions.Count == 0)
        {
            return Vector3.zero;
        }
        return availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
    }

    public static List<ActorInput> GetWoundedAlliesInRangeNonAlloc(NPCInput caller, float range, float hpThreshold, Collider[] populatedArray)
    {
        int numHit = Physics.OverlapSphereNonAlloc(caller.transform.position, range, populatedArray,
            1 << LayerMask.NameToLayer("Actors"));

        if(numHit == 0)
            return null;
        List<ActorInput> agentList = new List<ActorInput>();

        for(int i = 0; i < numHit; i++)
        {
            ActorInput agent = populatedArray[i].GetComponent<ActorInput>();
            //Debug.Log(agent.hpPercentage);

            if( /*agent != _agent &&*/
                //agent.CheckStatusEffect(Status.BEINGHEALED) == 0 &&
                agent.isDowned == false &&
                agent.dead == false &&
                agent.hpPercentage <= hpThreshold &&
                agent.ActorStats.IsFriend(caller.ActorStats) &&
                agent.ActorStats.isBeingBuffed == false)
                agentList.Add(agent);
        }
        return agentList.OrderBy(a => a.hpPercentage).ToList();
        //return agentList.OrderBy(a => (a.attackTarget.transform.position - a.transform.position).sqrMagnitude)
        //	.FirstOrDefault();
    }

    public static int GetNumAlliesAtLocation(ActorInput caller, Vector3 location, float range, Collider[] populatedArray)
    {
        int numHit = Physics.OverlapSphereNonAlloc(location, range, populatedArray,
            1 << LayerMask.NameToLayer("Actors"));

        if(numHit == 0)
            return 0;

        int hits = 0;

        for(int i = 0; i < numHit; i++)
        {
            ActorInput agent = populatedArray[i].GetComponent<ActorInput>();

            if( /*agent != _agent &&*/
                agent.dead == false && agent.ActorStats.IsFriend(caller.ActorStats))
                hits++;
        }

        return hits;
        //return agentList.OrderBy(a => (a.attackTarget.transform.position - a.transform.position).sqrMagnitude)
        //	.FirstOrDefault();
    }

    public static List<ActorInput> GetActorsOfClassAtLocation(ActorInput caller, Vector3 location, Class actorClass, float range, SpellTargetType targetType, Collider[] populatedArray)
    {
        int numHit = Physics.OverlapSphereNonAlloc(location, range, populatedArray,
            1 << LayerMask.NameToLayer("Actors"));

        if(numHit == 0)
            return null;

        List<ActorInput> agentList = new List<ActorInput>();

        for(int i = 0; i < numHit; i++)
        {
            ActorInput agent = populatedArray[i].GetComponent<ActorInput>();

            if(agent != caller && agent.ActorStats.Class == actorClass &&
                agent.dead == false &&
                (targetType == SpellTargetType.Foe ? agent.ActorStats.IsEnemy(caller.ActorStats)
                : targetType == SpellTargetType.Friend ? agent.ActorStats.IsFriend(caller.ActorStats)
                : true))
                agentList.Add(agent);
        }

        return agentList;
        //return agentList.OrderBy(a => (a.attackTarget.transform.position - a.transform.position).sqrMagnitude)
        //	.FirstOrDefault();
    }

    public static float GetDistanceToNearestActor(ActorInput caller, Vector3 point, Collider[] populatedArray, bool onlyHostile)
    {
        int numHit = Physics.OverlapSphereNonAlloc(point, 20, populatedArray,
            1 << LayerMask.NameToLayer("Actors"));

        if(numHit == 0)
            return 0;

        float shortestDist = 9999;

        for(int i = 0; i < numHit; i++)
        {
            ActorInput agent = populatedArray[i].GetComponent<ActorInput>();
            float newDist = Vector3.Distance(agent.transform.position, point);

            if(newDist < shortestDist && agent.dead == false && (onlyHostile ? agent.ActorStats.IsEnemy(caller.ActorStats) : agent.ActorStats.IsFriend(caller.ActorStats)))
                shortestDist = newDist;
        }

        return shortestDist;
    }
}
