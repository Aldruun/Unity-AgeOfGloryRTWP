using UnityEditor;
using UnityEngine;

namespace BzKovSoft.RagdollHelper.Editor
{
    public class RagdollProperties
	{
		public bool asTrigger = true;
		public bool isKinematic = true;
		public bool useGravity = true;
		public bool includeHandsAndFeet;
		public float rigidDrag;
		public float rigidAngularDrag;
		public float jointsLimitMult = 1f;
		public float headJointLimitMult = 1f;
		public float chestJointLimitMult = 1f;
		public float shoulderJointLimitMult = 1f;
		public float elbowJointLimitMult = 1f;
		public float hipsJointLimitMult = 1f;
		public float kneeJointLimitMult = 1f;
		public float handFootJointLimitMult = 1f;
		public CollisionDetectionMode cdMode;

		internal void Draw()
		{
			cdMode = (CollisionDetectionMode)EditorGUILayout.EnumPopup("Collision detection:", cdMode);

			rigidDrag = EditorGUILayout.FloatField("Rigid Drag:", rigidDrag);

			rigidAngularDrag = EditorGUILayout.FloatField("Rigid Angular Drag:", rigidAngularDrag);

			EditorGUI.BeginChangeCheck();
			jointsLimitMult = EditorGUILayout.Slider("Joints Limit Multiplier:", jointsLimitMult, 0f, 1f);
			if(EditorGUI.EndChangeCheck())
			{
				headJointLimitMult     = jointsLimitMult;
				chestJointLimitMult    = jointsLimitMult;
				shoulderJointLimitMult = jointsLimitMult;
				elbowJointLimitMult    = jointsLimitMult;
				hipsJointLimitMult     = jointsLimitMult;
				kneeJointLimitMult     = jointsLimitMult;
				handFootJointLimitMult = jointsLimitMult;
			}

			GUILayout.Space(10);
			headJointLimitMult     = EditorGUILayout.FloatField("Head Limit Multiplier:", headJointLimitMult);
			chestJointLimitMult    = EditorGUILayout.FloatField("Chest Limit Multiplier:", chestJointLimitMult);
			shoulderJointLimitMult = EditorGUILayout.FloatField("Shoulders Limit Multiplier:", shoulderJointLimitMult);
			elbowJointLimitMult    = EditorGUILayout.FloatField("Elbows Limit Multiplier:", elbowJointLimitMult);
			hipsJointLimitMult     = EditorGUILayout.FloatField("Hips Limit Multiplier:", hipsJointLimitMult);
			kneeJointLimitMult     = EditorGUILayout.FloatField("Knee Limit Multiplier:", kneeJointLimitMult);

			asTrigger = EditorGUILayout.Toggle("Trigger colliders:", asTrigger);

			isKinematic = EditorGUILayout.Toggle("Is kinematic:", isKinematic);

			useGravity = EditorGUILayout.Toggle("Use gravity:", useGravity);
			includeHandsAndFeet = EditorGUILayout.Toggle("Include Hands And Feet:", includeHandsAndFeet);

            if(includeHandsAndFeet)
            {
				handFootJointLimitMult = EditorGUILayout.FloatField("Hand Foot Limit Multiplier:", handFootJointLimitMult);
			}
		}
	}
}