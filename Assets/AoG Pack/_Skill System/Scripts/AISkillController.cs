using GenericFunctions;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

namespace AoG.AI
{
    public class AISkillController
    {
        private NPCInput agent;
        private Skill chosenSkill;
        private Vector3 skillTargetPosition;
        public float recoveryCountdown;
        private float skillCheckCountdown;
        Skill _playerCommand;
        //private List<Skill> _skills;
        private Transform _vfxPoint;
        public Action<float, float> RecoveryTimeCallback; // updated value, recovery time
        public Action<float> SkillCheckIntervalCallback;

        public Action<float> NextActionHook;

        public float recoveryTime { get; private set; }

        public float skillCheckInterval { get; } = 0.5f;

        private float makeMoveCooldown;

        public AISkillController(ActorInput agent)
        {
            this.agent = agent as NPCInput;

            var animator = agent.GetComponent<Animator>();
            if(animator.isHuman)
                _vfxPoint = animator.GetBoneTransform(HumanBodyBones.RightHand);
            else
                _vfxPoint = this.agent.Combat.GetAttackPoint();
        }

        public void UpdateAI()
        {
            if(agent == null || agent.dead)
            {
                return;
            }

            if(agent.Animation.Animator.GetCurrentAnimatorStateInfo(1).IsName("New State"))
            {
                if(agent.debugInput)
                {
                    Debug.Log($"{agent.GetName()}: <color=orange>SkillCTRL</color> - Unrooting");
                }
                agent.NavAgent.isStopped = false;
            }
            else
            {
                agent.Combat.Execute_BlockAggro(2);
                agent.NavAgent.isStopped = true;
            }
            if(agent.debugInput)
                Debug.Log($"{agent.GetName()}: <color=cyan>Updating SkillSystem</color>");

            if(_playerCommand != null)
            {
                if(UpdateSkill(_playerCommand) == false && _playerCommand.repeatIfNoAI == false
                   //FIXME:
                   /*_playerCommand.GetType() != typeof(Skill_DefaultAttack)*/)
                {
                    _playerCommand = null;
                }
            }
            else if(chosenSkill != null)
            {
                if(UpdateSkill(chosenSkill) == false)
                {
                    chosenSkill = null;
                }
            }
            else
            {
                if(agent.aiControlled && _playerCommand == null)
                {
                    skillCheckCountdown -= Time.deltaTime;
                    SkillCheckIntervalCallback?.Invoke(skillCheckCountdown);
                    if(skillCheckCountdown <= 0 && recoveryCountdown <= 0)
                    {
                        skillCheckCountdown = 0.5f;

                        chosenSkill = ChooseBestSkill();
                    }
                }

                if(agent.NavAgent.hasPath)
                {
                    HelperFunctions.RotateTo(agent.transform, agent.NavAgent.steeringTarget, 300);
                }
            }


            recoveryCountdown -= Time.deltaTime;
            RecoveryTimeCallback?.Invoke(recoveryCountdown, recoveryTime);
        }

        private bool UpdateSkill(Skill skill)
        {
            if(skill == null)
            {
                Debug.LogError("UpdateSkill() -> NULL");
            }
            bool hasValidTarget = skill.skillTarget != null && skill.skillTarget.dead == false;

            if(hasValidTarget == false)
            {
                chosenSkill = null;
                return false;
            }

            Vector3 targetPos = skill.skillTarget.transform.position;
            Vector3 targetDir = targetPos - agent.transform.position;
            float distToTarget = targetDir.magnitude;

            bool hasLOS = skill.skillTarget == agent ? true : HelperFunctions.LineOfSightBoxcast(agent.transform, skill.skillTarget.transform,
                new Vector3(0.2f, 0.1f, 0.1f), 1.6f, 1.6f);
            // bool hasLOS = HelperFunctions.LineOfSightH2H(agent.transform, targetPos, 1.6f, 1.6f);
            bool inFOV = Get.IsInFOV(agent.transform, targetPos, 10) == true;
            float requiredDistance = skill.GetActivationRange(agent) + skill.skillTarget.GetCharacterRadius();
            bool inRange = distToTarget <= requiredDistance;
            if(inFOV == false)
            {
                // Rotation happens in CombatState
                if(agent.debugInput)
                    Debug.Log(agent.GetName() + ":<color=green>#</color> Not in FOV");
                HelperFunctions.RotateTo(agent.transform, skill.skillTarget.transform.position, 200);
                hasLOS = false;
            }

            makeMoveCooldown -= Time.deltaTime;

            //if(_weapon.range > 10)
            //{
            if(inRange == false && makeMoveCooldown <= 0)
            {
                makeMoveCooldown = UnityEngine.Random.Range(1f, 4f);

                DoSideStep(skill.skillTarget);

                //Debug.Log($"{ctrl.agent.GetName()}:<color=orange>#</color> Attempting combat move");
            }


            if(hasLOS == false || inRange == false)
            {
                if(agent.debugInput)
                    Debug.Log(agent.GetName() + ":<color=green>#</color> InRange: " + inRange + " hasLOS: " + hasLOS);

                agent.SetDestination(targetPos, 0);
                return true;
            }

            agent.HoldPosition();

            if(hasLOS && inRange && recoveryCountdown <= 0)
            {
                skillTargetPosition = skill.skillTarget.transform.position;

                skillCheckCountdown = 0;

                if(agent.debugAnimation)
                {
                    Debug.Log($"{agent.GetName()}: <color=green>Updating SkillSystem with skill '<color=white>{skill.skillName}</color>'</color>");
                }

                Profiler.BeginSample("SkillCtrl: Steering");

                if(agent.debugInput)
                    Debug.Log($"{agent.GetName()}: <color=green>Activating skill '<color=white>{skill.skillName}</color>'</color>");

                switch(skill.DeliveryType)
                {
                    case DeliveryType.None:
                        break;
                    case DeliveryType.Contact:
                        skill.Activate(agent, skill.skillTarget, Vector3.zero);
                        break;
                    case DeliveryType.SeekActor:
                        skill.Activate(agent, skill.skillTarget, Vector3.zero);
                        break;
                    case DeliveryType.SeekLocation:
                        skill.Activate(agent, null, skillTargetPosition);
                        break;
                    case DeliveryType.InstantSelf:
                        skill.Activate(agent, agent, Vector3.zero);
                        break;
                    case DeliveryType.InstantActor:
                        skill.Activate(agent, skill.skillTarget, Vector3.zero);
                        break;
                    case DeliveryType.InstantLocation:
                        skill.Activate(agent, null, skillTargetPosition);
                        break;
                    case DeliveryType.Spray:
                        skill.Activate(agent, skill.skillTarget, Vector3.zero);
                        break;
                    case DeliveryType.Beam:
                        skill.Activate(agent, skill.skillTarget, Vector3.zero);
                        break;
                    default:
                        break;
                }

                recoveryCountdown = recoveryTime = skill.recoveryTime;

                skillTargetPosition = agent.transform.forward; //! Keep orientation
                Profiler.EndSample();
                return false; //! Stop updating skill
            }

            return true;
        }

        internal void CancelSkill()
        {
            chosenSkill = null;
        }

        private Skill ChooseBestSkill()
        {
            Profiler.BeginSample("SkillCtrl: Pick Skill");
            Skill highestPrioritySkill = null;

            if(agent.debugInput && agent.GetSkills(false).Count == 0)
                Debug.Log(agent.GetName() + ": Skillbook empty");

            for(var i = 0; i < agent.GetSkills(false).Count; i++)
            {
                var currSkill = agent.GetSkills(false)[i];

                if(agent.debugInput)
                    Debug.Log(agent.GetName() + ": ITERATING SKILL '" + currSkill.skillName + "'");

                if(currSkill.CanActivate(agent) == false)
                {
                    if(agent.debugInput)
                        Debug.Log(agent.GetName() + ":<color=yellow>*</color> Can't activate skill '" + currSkill.skillName + "'");
                }
                else
                {
                    if(highestPrioritySkill == null)
                        highestPrioritySkill = currSkill;
                    else if(currSkill.priority < highestPrioritySkill.priority)
                        //Debug.Log($"{_agent.name}: <color=white>Curr skill '{currSkill.skillName}[{currSkill.priority}]' is more urgent than '{highestPrioritySkill.skillName}[{highestPrioritySkill.priority}]'</color>");
                        //Debug.Log(gameObject.name + "2: Chose skill '" + currSkill.skillName + "'");
                        highestPrioritySkill = currSkill;
                }
            }

            if(agent.debugInput)
            {
                if(highestPrioritySkill == null)
                    Debug.Log(agent.GetName() + ": No skill found");
                else
                    Debug.Log(agent.GetName() + ": Chose skill '<color=white>" + highestPrioritySkill.skillName +
                              "</color>'");
            }
            Profiler.EndSample();
            return highestPrioritySkill;
        }

        private void DoSideStep(ActorInput target)
        {
            Vector3 pointOnGround = Vector3.zero;

            Vector3 newTacticalPosition = agent.transform.position;
            Vector3 targetPos = target.transform.position;
            Vector3 fleeDir = (agent.transform.position - targetPos).normalized;
            bool isRanged = agent.ActorStats.isSpellCaster || agent.Equipment.equippedWeapon.Weapon != null && agent.Equipment.equippedWeapon.Weapon.range > 10;

            //blocked = true;
            //if(agent.attackTarget != null && inRange)
            //{

            //if(agent.m_isSpellCaster)
            //{
            //    AgentMonoController friendly = AgentFunctions.GetStrongestFriendlyInRangeNonAlloc(agent, 50, _friends);

            //    newTacticalPosition = fleeDir
            //        //* UnityEngine.Random.Range(4, 8)
            //        + (UnityEngine.Random.insideUnitSphere
            //        * UnityEngine.Random.Range(0, 3))
            //        + (friendly != null ? (friendly.transform.position - agent.transform.position) : HelperFunctions.GetSampledNavMeshPosition(agent.transform.position - agent.transform.forward));

            //    if(Vector3.Distance(newTacticalPosition, agent.transform.position) < 1.5f)
            //    {
            //        newTacticalPosition = agent.transform.position + -fleeDir + UnityEngine.Random.insideUnitSphere + fleeDir.normalized * 0.2f;
            //    }
            //}
            //else
            //{


            if(isRanged)
            {
                float fleeStrength = 1 - HelperFunctions.GetLinearDistanceAttenuation(agent.transform.position, targetPos, 2, 10);
                //float centerStrength = (HelperFunctions.GetLinearDistanceAttenuation(agent.transform.position, CoverGrid.mapCenter, 2, 10));
                newTacticalPosition += fleeDir.normalized * UnityEngine.Random.Range(3, 15) * fleeStrength;
                //newTacticalPosition += (CoverGrid.mapCenter - agent.transform.position).normalized * centerStrength * (fleeStrength);
            }

            //bool leftRight = UnityEngine.Random.value > 0.5f;
            else
            {
                float stepRange = 0;
                //if(HelperFunctions.LineOfSight(HelperFunctions.GetSampledNavMeshPosition(newTacticalPosition), targetPos) == false)
                //{
                for(int i = 0; i < 6; i++)
                {
                    stepRange += 1;

                    newTacticalPosition = agent.transform.position + agent.transform.right * stepRange;
                    if(HelperFunctions.LineOfSight(HelperFunctions.GetSampledNavMeshPosition(newTacticalPosition), targetPos))
                    {
                        //Debug.DrawLine(agent.transform.position, newTacticalPosition, Color.green, 0.2f);
                        //Debug.DrawLine(newTacticalPosition, targetPos, Color.green, 0.2f);
                        break;
                    }
                    else
                    {
                        newTacticalPosition = agent.transform.position + -agent.transform.right * stepRange;
                        if(HelperFunctions.LineOfSight(HelperFunctions.GetSampledNavMeshPosition(newTacticalPosition), targetPos))
                        {
                            //Debug.DrawLine(agent.transform.position, newTacticalPosition, Color.green, 0.2f);
                            //Debug.DrawLine(newTacticalPosition, targetPos, Color.green, 0.2f);
                            break;
                        }
                        else
                        {
                            newTacticalPosition = agent.transform.position + -agent.transform.forward * stepRange;
                            if(HelperFunctions.LineOfSight(HelperFunctions.GetSampledNavMeshPosition(newTacticalPosition), targetPos))
                            {
                                //Debug.DrawLine(agent.transform.position, newTacticalPosition, Color.green, 0.2f);
                                //Debug.DrawLine(newTacticalPosition, targetPos, Color.green, 0.2f);
                                break;
                            }

                        }
                    }
                }
            }
            //}

            //Vector3 inNormalPos = (targetPos - newTacticalPosition).normalized;
            //newTacticalPosition = Vector3.Reflect(targetPos - agent.transform.position, inNormalPos); 

            NavMeshHit navHit;
            if(NavMesh.SamplePosition(newTacticalPosition, out navHit, float.PositiveInfinity, NavMesh.AllAreas))
            {
                newTacticalPosition = navHit.position;
            }

            Debug.DrawLine(agent.transform.position + Vector3.up * 1, newTacticalPosition + Vector3.up * 1, Color.red, 1);

            agent.SetDestination(newTacticalPosition, 0.1f, "", 1);
            //}
            //else
            //{
            //    return;
            //}
            //}

            //if(agent.isBeast                                                                                              == false && Random.value < 0.1f * (1 - HelperFunctions.GetLinearDistanceAttenuation(targetPos, agent.transform.position, 1, 20)))
            //{
            //    NPCAdvancedMotionProtocol.JumpNonLink(agent, agent.navAgent.velocity.normalized, 0.5f);
            //}


        }

        internal void SetSkill(Skill skill, ActorInput target, Vector3 targetPosition)
        {
            if(skill.PlayerCanActivate(agent) == false)
            {
                Debug.LogError("Player 'Use Skill' command failed");
                return;
            }
            CancelSkill();
            agent.CancelActions();
            agent.hasMovementOrder = false;
            _playerCommand = skill;
            _playerCommand.SetPlayerSkillTarget(target);

            skillTargetPosition = targetPosition;

            //if(agent.debugActions)
            //    Debug.Log("<color=cyan>Setting skill</color>");
        }

        internal void SetDefaultAttackSkill(ActorInput target)
        {

            Skill skill = agent.GetSkills(false)[0];
            if(skill != null && skill.GetType() == typeof(Skill_DefaultAttack) && _playerCommand != null && _playerCommand.GetType() == typeof(Skill_DefaultAttack))
            {
                return;
            }
            agent.hasMovementOrder = false;
            agent.CancelActions();
            CancelSkill();
            _playerCommand = skill;
            skill.SetPlayerSkillTarget(target);
            if(skill.PlayerCanActivate(agent))
            {
                //if(agent.debugActions)
                //    Debug.Log("<color=cyan>Setting skill</color>");
                chosenSkill = skill;
            }
            else
            {
                Debug.LogError("Player 'Use Default Attack Skill' command failed");
            }
        }
    }
}