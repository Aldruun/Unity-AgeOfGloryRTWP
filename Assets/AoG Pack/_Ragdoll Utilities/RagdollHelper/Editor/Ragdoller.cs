using UnityEngine;
using System;

namespace BzKovSoft.RagdollHelper.Editor
{
    /// <summary>
    /// Class responsible for regdoll and unregdoll character
    /// </summary>
    internal sealed class Ragdoller
	{
		private const string _colliderNodeSufix = "_ColliderRotator";
		private readonly bool _readyToGenerate;
		private readonly Vector3 _playerDirection;
		private readonly Transform _rootNode;

		private readonly RagdollPartBox _pelvis;
		private readonly RagdollPartCapsule _leftHip;
		private readonly RagdollPartCapsule _leftKnee;
		private readonly RagdollPartCapsule _rightHip;
		private readonly RagdollPartCapsule _rightKnee;
		private readonly RagdollPartCapsule _leftArm;
		private readonly RagdollPartCapsule _leftElbow;
		private readonly RagdollPartCapsule _rightArm;
		private readonly RagdollPartCapsule _rightElbow;
		private readonly RagdollPartBox _chest;
		private readonly RagdollPartBox _head;

		private readonly RagdollPartBox _leftFoot;
		private readonly RagdollPartBox _rightFoot;
		private readonly RagdollPartBox _leftHand;
		private readonly RagdollPartBox _rightHand;

		public Ragdoller(Transform player, Vector3 playerDirection)
		{
			_playerDirection = playerDirection;
			_readyToGenerate = false;

			// find Animator
			Animator animator = FindAnimator(player);
			if (animator == null)
				return;
			_rootNode = animator.transform;

			// specify all parts of ragdoll
			_pelvis = new RagdollPartBox(animator.GetBoneTransform(HumanBodyBones.Hips));
			_leftHip = new RagdollPartCapsule(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
			_leftKnee = new RagdollPartCapsule(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
			_rightHip = new RagdollPartCapsule(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg));
			_rightKnee = new RagdollPartCapsule(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
			_leftArm = new RagdollPartCapsule(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
			_leftElbow = new RagdollPartCapsule(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
			_rightArm = new RagdollPartCapsule(animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
			_rightElbow = new RagdollPartCapsule(animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
			_chest = new RagdollPartBox(animator.GetBoneTransform(HumanBodyBones.Chest));
			_head = new RagdollPartBox(animator.GetBoneTransform(HumanBodyBones.Head));

			_leftFoot = new RagdollPartBox(animator.GetBoneTransform(HumanBodyBones.LeftFoot));
			_rightFoot = new RagdollPartBox(animator.GetBoneTransform(HumanBodyBones.RightFoot));
			_leftHand = new RagdollPartBox(animator.GetBoneTransform(HumanBodyBones.LeftHand));
			_rightHand = new RagdollPartBox(animator.GetBoneTransform(HumanBodyBones.RightHand));

			if (_chest.transform == null)
				_chest = new RagdollPartBox(animator.GetBoneTransform(HumanBodyBones.Spine));

			if (!CheckFields())
			{
				Debug.LogError("Not all nodes was found!");
				return;
			}

			_readyToGenerate = true;
		}
		/// <summary>
		/// Finds animator component in "player" and in parents till it find Animator component. Otherwise returns null
		/// </summary>
		private static Animator FindAnimator(Transform player)
		{
			Animator animator;
			do
			{
				animator = player.GetComponent<Animator>();
				if (animator != null && animator.enabled)
					break;

				player = player.parent;
			}
			while (player != null);

			if (animator == null | player == null)
			{
				Debug.LogError("An Animator must be attached to find bones!");
				return null;
			}
			if (!animator.isHuman)
			{
				Debug.LogError("To auto detect bones, there are has to be humanoid Animator!");
				return null;
			}
			return animator;
		}
		/// <summary>
		/// Some checks before Applying ragdoll
		/// </summary>
		private bool CheckFields()
		{
			if (_rootNode == null |
				_pelvis == null |
				_leftHip == null |
				_leftKnee == null |
				_rightHip == null |
				_rightKnee == null |
				_leftArm == null |
				_leftElbow == null |
				_rightArm == null |
				_rightElbow == null |
				_chest == null |
				_head == null)
				return false;

			return true;
		}


		/// <summary>
		/// Create all ragdoll's components and set their proterties
		/// </summary>
		public void ApplyRagdoll(float totalMass, RagdollProperties ragdollProperties)
		{
			if (!_readyToGenerate)
			{
				Debug.LogError("Initialization failed. Reinstance object!");
				return;
			}

			var weight = new WeightCalculator(totalMass, ragdollProperties.includeHandsAndFeet);

			bool alreadyRagdolled = _pelvis.transform.gameObject.GetComponent<Rigidbody>() != null;

			AddComponentesTo(_pelvis,     ragdollProperties, weight.Pelvis, false);
			AddComponentesTo(_leftHip,    ragdollProperties, weight.Hip,    true);
			AddComponentesTo(_leftKnee,   ragdollProperties, weight.Knee,   true);
			AddComponentesTo(_rightHip,   ragdollProperties, weight.Hip,    true);
			AddComponentesTo(_rightKnee,  ragdollProperties, weight.Knee,   true);
			AddComponentesTo(_leftArm,    ragdollProperties, weight.Arm,    true);
			AddComponentesTo(_leftElbow,  ragdollProperties, weight.Elbow,  true);
			AddComponentesTo(_rightArm,   ragdollProperties, weight.Arm,    true);
			AddComponentesTo(_rightElbow, ragdollProperties, weight.Elbow,  true);
			AddComponentesTo(_chest,      ragdollProperties, weight.Chest,  true);
			AddComponentesTo(_head,       ragdollProperties, weight.Head,   true);

			if (ragdollProperties.includeHandsAndFeet)
			{
				AddComponentesTo(_leftFoot,   ragdollProperties, weight.Foot,   true);
				AddComponentesTo(_rightFoot,  ragdollProperties, weight.Foot,   true);
				AddComponentesTo(_leftHand,   ragdollProperties, weight.Hand,   true);
				AddComponentesTo(_rightHand,  ragdollProperties, weight.Hand,   true);
			}

			if (alreadyRagdolled)
				return;

			// Pelvis
			//Vector3 pelvisSize = new Vector3(0.32f, 0.31f, 0.3f);
			//Vector3 pelvisCenter = new Vector3(00f, 0.06f, -0.01f);
			//_pelvis.collider.size = Abs(_pelvis.transform.InverseTransformVector(pelvisSize));
			_pelvis.collider.size = new Vector3(0.5f, 0.5f, 0.3f);
			_pelvis.collider.center = _pelvis.transform.InverseTransformVector(new Vector3(00f, 0.06f, -0.01f));

			ApplySide(ragdollProperties, true, ragdollProperties.includeHandsAndFeet);
			ApplySide(ragdollProperties, false, ragdollProperties.includeHandsAndFeet);

			// Chest collider
			Vector3 chestSize = new Vector3(0.34f, 0.34f, 0.28f);

			//float y = (pelvisSize.y + chestSize.y) / 2f + pelvisCenter.y;
			//y -= _chest.transform.position.y - _pelvis.transform.position.y;
			//_chest.collider.size = Abs(_chest.transform.InverseTransformVector(chestSize));
			_chest.collider.size = new Vector3(0.6f, 0.6f, 0.3f);
			_chest.collider.center = _chest.transform.InverseTransformVector(new Vector3(0f, 0.03f, -0.03f));

			// Chest joint
			var chestJoint = _chest.joint;
			ConfigureJointParams(_chest, _pelvis.rigidbody, _rootNode.right, _rootNode.forward);
			ConfigureJointLimits(chestJoint,
				-45f * ragdollProperties.chestJointLimitMult,
				20f * ragdollProperties.chestJointLimitMult,
				20f * ragdollProperties.chestJointLimitMult,
				20f * ragdollProperties.chestJointLimitMult);

			// head
			_head.collider.size = Vector3.one * 0.35f;
			_head.collider.center = _head.transform.InverseTransformVector(new Vector3(0f, 0.09f, 0.03f));
			var headJoint = _head.joint;
			ConfigureJointParams(_head, _chest.rigidbody, _rootNode.right, _rootNode.forward);
			ConfigureJointLimits(headJoint,
				-45f * ragdollProperties.headJointLimitMult,
				20f * ragdollProperties.headJointLimitMult,
				20f * ragdollProperties.headJointLimitMult,
				20f * ragdollProperties.headJointLimitMult);
		}

		private Vector3 Abs(Vector3 v)
		{
			return new Vector3(
				Mathf.Abs(v.x),
				Mathf.Abs(v.y),
				Mathf.Abs(v.z)
				);
		}

		private static void ConfigureJointParams(RagdollPartBase part, Rigidbody anchor, Vector3 axis, Vector3 swingAxis)
		{
			part.joint.connectedBody = anchor;
			part.joint.axis = part.transform.InverseTransformDirection(axis);
			part.joint.swingAxis = part.transform.InverseTransformDirection(swingAxis);
		}

		private static void ConfigureJointLimits(CharacterJoint joint, float lowTwist, float highTwist, float swing1, float swing2)
		{
			if (lowTwist > highTwist)
				throw new ArgumentException("wrong limitation: lowTwist > highTwist");

			var twistLimitSpring = joint.twistLimitSpring;
			joint.twistLimitSpring = twistLimitSpring;

			var swingLimitSpring = joint.swingLimitSpring;
			joint.swingLimitSpring = swingLimitSpring;

			// configure limits
			var lowTwistLimit = joint.lowTwistLimit;
			lowTwistLimit.limit = lowTwist;
			joint.lowTwistLimit = lowTwistLimit;
			var highTwistLimit = joint.highTwistLimit;
			highTwistLimit.limit = highTwist;
			joint.highTwistLimit = highTwistLimit;

			var swing1Limit = joint.swing1Limit;
			swing1Limit.limit = swing1;
			joint.swing1Limit = swing1Limit;
			var swing2Limit = joint.swing2Limit;
			swing2Limit.limit = swing2;
			joint.swing2Limit = swing2Limit;
		}

		/// <summary>
		/// Configure one hand and one leg
		/// </summary>
		/// <param name="leftSide">If true, configuration apply to left hand and left leg, otherwise right hand and right leg</param>
		private void ApplySide(RagdollProperties rp, bool leftSide, bool createTips)
		{
			RagdollPartCapsule hip = (leftSide ? _leftHip : _rightHip);
			RagdollPartCapsule knee = (leftSide ? _leftKnee : _rightKnee);
			RagdollPartBox foot = (leftSide ? _leftFoot : _rightFoot);
			
			RagdollPartCapsule arm = (leftSide ? _leftArm : _rightArm);
			RagdollPartCapsule elbow = (leftSide ? _leftElbow : _rightElbow);
			RagdollPartBox hand = (leftSide ? _leftHand : _rightHand);

			ConfigureRagdollForLimb(hip, knee, foot, createTips);
			ConfigureLegsJoints(rp, hip, knee, foot, createTips);

			ConfigureRagdollForLimb(arm, elbow, hand, createTips);
			ConfigureHandJoints(rp, arm, elbow, hand, leftSide, createTips);
		}
		/// <summary>
		/// Configer one of 4 body parts: right leg, left leg, right hand or left hand
		/// </summary>
		private static void ConfigureRagdollForLimb(RagdollPartCapsule limbUpper, RagdollPartCapsule limbLower, RagdollPartBox tip, bool createTips)
		{
			float totalLength = limbUpper.transform.InverseTransformPoint(tip.transform.position).magnitude;

			// limbUpper
			CapsuleCollider upperCapsule = limbUpper.collider;
			var boneEndPos = limbUpper.transform.InverseTransformPoint(limbLower.transform.position);
			upperCapsule.direction = GetXyzDirection(limbLower.transform.localPosition);
			upperCapsule.radius = totalLength * 0.12f;
			upperCapsule.height = boneEndPos.magnitude;
			upperCapsule.center = Vector3.Scale(boneEndPos, Vector3.one * 0.5f);

			// limbLower
			CapsuleCollider endCapsule = limbLower.collider;
			boneEndPos = limbLower.transform.InverseTransformPoint(tip.transform.position);
			endCapsule.direction = GetXyzDirection(boneEndPos);
			endCapsule.radius = totalLength * 0.12f;
			endCapsule.height = boneEndPos.magnitude;
			endCapsule.center = Vector3.Scale(boneEndPos, Vector3.one * 0.5f);

			// tip
			if (createTips)
			{
				boneEndPos = GetLongestTransform(tip.transform).position;
				boneEndPos = tip.transform.InverseTransformPoint(boneEndPos);

				Vector3 tipDir = GetXyzDirectionV(boneEndPos);
				Vector3 tipSides = (tipDir - Vector3.one) * -1;
				Vector3 boxSize = tipDir * boneEndPos.magnitude * 1.3f + tipSides * totalLength * 0.2f;

				BoxCollider tipBox = tip.collider;
				tipBox.size = boxSize;

				float halfTipLength = boneEndPos.magnitude / 2f;
				tipBox.center = Vector3.Scale(boneEndPos.normalized, Vector3.one * halfTipLength);
			}
		}

		private static Transform GetLongestTransform(Transform limb)
		{
			float longestF = -1;
			Transform longestT = null;

			// find the farest object that attached to 'limb'
			foreach (Transform t in limb.GetComponentsInChildren<Transform>())
			{
				float length = (limb.position - t.position).sqrMagnitude;
				if (length > longestF)
				{
					longestF = length;
					longestT = t;
				}
			}

			return longestT;
		}

		private static Vector3 GetXyzDirectionV(Vector3 node)
		{
			var d = GetXyzDirection(node);

			switch (d)
			{
				case 0: return Vector3.right;
				case 1: return Vector3.up;
				case 2: return Vector3.forward;
			}

			throw new InvalidOperationException();
		}
		
		/// <summary>
		/// Get the most appropriate direction in terms of PhysX (0,1,2 directions)
		/// </summary>
		private static int GetXyzDirection(Vector3 node)
		{
			float x = Mathf.Abs(node.x);
			float y = Mathf.Abs(node.y);
			float z = Mathf.Abs(node.z);

			if (x > y & x > z)		// x is the bigest
				return 0;
			if (y > x & y > z)		// y is the bigest
				return 1;

			// z is the bigest
			return 2;
		}

		private void ConfigureHandJoints(RagdollProperties rp, RagdollPartCapsule arm, RagdollPartCapsule elbow, RagdollPartBox hand, bool leftHand, bool createTips)
		{
			var dirUpper = elbow.transform.position - arm.transform.position;
			var dirLower = hand.transform.position - elbow.transform.position;
			var dirHand = GetLongestTransform(hand.transform).position - hand.transform.position; // TODO: need to find the most longest bone

			if (leftHand)
			{
				ConfigureJointLimits(arm.joint,
					-100f * rp.shoulderJointLimitMult,
					30f * rp.shoulderJointLimitMult,
					100f * rp.shoulderJointLimitMult,
					45f * rp.shoulderJointLimitMult);
				ConfigureJointLimits(elbow.joint,
					-120f * rp.elbowJointLimitMult,
					0f * rp.elbowJointLimitMult,
					10f * rp.elbowJointLimitMult,
					90f * rp.elbowJointLimitMult);
				if (createTips)
				{
					ConfigureJointLimits(hand.joint, -90f * rp.handFootJointLimitMult, 90f * rp.handFootJointLimitMult, 90f * rp.handFootJointLimitMult, 45f * rp.handFootJointLimitMult);
				}
				dirUpper = -dirUpper;
				dirLower = -dirLower;
				dirHand = -dirHand;
			}
			else
			{
				ConfigureJointLimits(arm.joint, -30f * rp.shoulderJointLimitMult, 100f * rp.shoulderJointLimitMult, 100f * rp.shoulderJointLimitMult, 45f * rp.shoulderJointLimitMult);
				ConfigureJointLimits(elbow.joint, 0f * rp.elbowJointLimitMult, 120f * rp.elbowJointLimitMult, 10f * rp.elbowJointLimitMult, 90f * rp.elbowJointLimitMult);
				if (createTips)
				{
					ConfigureJointLimits(hand.joint, -90f * rp.handFootJointLimitMult, 90f * rp.handFootJointLimitMult, 90f * rp.handFootJointLimitMult, 45f * rp.handFootJointLimitMult);
				}
			}

			var upU = Vector3.Cross(_playerDirection, dirUpper);
			var upL = Vector3.Cross(_playerDirection, dirLower);
			var upH = Vector3.Cross(_playerDirection, dirHand);
			ConfigureJointParams(arm, _chest.rigidbody, upU, _playerDirection);
			ConfigureJointParams(elbow, arm.rigidbody, upL, _playerDirection);
			if (createTips)
			{
				ConfigureJointParams(hand, elbow.rigidbody, upH, _playerDirection);
			}
		}

		private void ConfigureLegsJoints(RagdollProperties rp, RagdollPartCapsule hip, RagdollPartCapsule knee, RagdollPartBox foot, bool createTips)
		{
			var hipJoint = hip.joint;
			var kneeJoint = knee.joint;
			var footJoint = foot.joint;

			ConfigureJointParams(hip, _pelvis.rigidbody, _rootNode.right, _rootNode.forward);
			ConfigureJointParams(knee, hip.rigidbody, _rootNode.right, _rootNode.forward);

			ConfigureJointLimits(hipJoint, -10f * rp.hipsJointLimitMult, 80f * rp.hipsJointLimitMult, 80f * rp.hipsJointLimitMult, 20f * rp.hipsJointLimitMult);
			ConfigureJointLimits(kneeJoint, -90f * rp.kneeJointLimitMult, 0f * rp.kneeJointLimitMult, 10f * rp.kneeJointLimitMult, 20f * rp.kneeJointLimitMult);

			if (createTips)
			{
				ConfigureJointParams(foot, knee.rigidbody, _rootNode.right, _rootNode.forward);
				ConfigureJointLimits(footJoint, -70f * rp.handFootJointLimitMult, 70f * rp.handFootJointLimitMult, 45f * rp.handFootJointLimitMult, 20f * rp.handFootJointLimitMult);
			}
		}

		private static void AddComponentesTo(RagdollPartBox part, RagdollProperties ragdollProperties, float mass, bool addJoint)
		{
			AddComponentesToBase(part, ragdollProperties, mass, addJoint);
			GameObject go = part.transform.gameObject;
			go.layer = LayerMask.NameToLayer("Bodyparts");
			part.collider = GetCollider<BoxCollider>(go.transform);
			if (part.collider == null)
				part.collider = go.AddComponent<BoxCollider>();
			part.collider.isTrigger = ragdollProperties.asTrigger;
		}

		private static void AddComponentesTo(RagdollPartCapsule part, RagdollProperties ragdollProperties, float mass, bool addJoint)
		{
			AddComponentesToBase(part, ragdollProperties, mass, addJoint);
			GameObject go = part.transform.gameObject;
			go.layer = LayerMask.NameToLayer("Bodyparts");
			part.collider = GetCollider<CapsuleCollider>(go.transform);
			if (part.collider == null)
				part.collider = go.AddComponent<CapsuleCollider>();
			part.collider.isTrigger = ragdollProperties.asTrigger;
		}

		private static void AddComponentesTo(RagdollPartSphere part, RagdollProperties ragdollProperties, float mass, bool addJoint)
		{
			AddComponentesToBase(part, ragdollProperties, mass, addJoint);
			GameObject go = part.transform.gameObject;
			go.layer = LayerMask.NameToLayer("Bodyparts");
			part.collider = GetCollider<SphereCollider>(go.transform);
			if (part.collider == null)
				part.collider = go.AddComponent<SphereCollider>();
			part.collider.isTrigger = ragdollProperties.asTrigger;
		}

		private static void AddComponentesToBase(RagdollPartBase part, RagdollProperties ragdollProperties, float mass, bool addJoint)
		{
			GameObject go = part.transform.gameObject;
			go.layer = LayerMask.NameToLayer("Bodyparts");
			part.rigidbody = go.GetComponent<Rigidbody>();
			if (part.rigidbody == null)
				part.rigidbody = go.AddComponent<Rigidbody>();
			part.rigidbody.mass = mass;
			part.rigidbody.drag = ragdollProperties.rigidDrag;
			part.rigidbody.angularDrag = ragdollProperties.rigidAngularDrag;
			part.rigidbody.collisionDetectionMode = ragdollProperties.cdMode;
			part.rigidbody.isKinematic = ragdollProperties.isKinematic;
			part.rigidbody.useGravity = ragdollProperties.useGravity;

			if (addJoint)
			{
				part.joint = go.GetComponent<CharacterJoint>();
				if (part.joint == null)
					part.joint = go.AddComponent<CharacterJoint>();

				part.joint.enablePreprocessing = false;
				part.joint.enableProjection = true;
			}
		}

		private static T GetCollider<T>(Transform transform)
			where T : Collider
		{
			for (int i = 0; i < transform.childCount; ++i)
			{
				Transform child = transform.GetChild(i);

				if (child.name.EndsWith(_colliderNodeSufix))
				{
					transform = child;
					break;
				}
			}

			return transform.GetComponent<T>();
		}

		/// <summary>
		/// Remove all colliders, joints, and rigids
		/// </summary>
		public void ClearRagdoll()
		{
			foreach (var component in _pelvis.transform.GetComponentsInChildren<Collider>())
				GameObject.DestroyImmediate(component);
			foreach (var component in _pelvis.transform.GetComponentsInChildren<CharacterJoint>())
				GameObject.DestroyImmediate(component);
			foreach (var component in _pelvis.transform.GetComponentsInChildren<Rigidbody>())
				GameObject.DestroyImmediate(component);

			DeleteColliderNodes(_pelvis.transform);
		}
		/// <summary>
		/// Correct deleting collider with collider's separate nodes
		/// </summary>
		/// <param name="node"></param>
		private static void DeleteColliderNodes(Transform node)
		{
			for (int i = 0; i < node.childCount; ++i)
			{
				Transform child = node.GetChild(i);

				if (child.name.EndsWith(_colliderNodeSufix))
					GameObject.DestroyImmediate(child.gameObject);
				else
					DeleteColliderNodes(child);
			}
		}
	}
}