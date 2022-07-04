using System;
using UnityEditor;
using UnityEngine;

namespace BzKovSoft.RagdollHelper.Editor
{
	internal class RagdollController
	{
		private GameObject _go;
		private Func<Vector3> _getPlayerDirection;

		private RagdollProperties _ragdollProperties = new RagdollProperties
		{
			asTrigger = true,
			isKinematic = true,
			rigidAngularDrag = 0.3f,
			rigidDrag = 0.3f
		};

		private int _ragdollTotalWeight = 60;           // weight of character (by default 60)

		public RagdollController(GameObject go, Func<Vector3> getPlayerDirection)
		{
			_go = go;
			_getPlayerDirection = getPlayerDirection;
		}

		public void DrawRagdollPanel(bool humanoidSelected)
		{
			GUILayout.BeginVertical("box");
			
			if (humanoidSelected)
			{
				GUILayout.Label("Ragdoll:");
				if (GUILayout.Button("Create"))
					CreateRagdoll();
				if (GUILayout.Button("Remove"))
					RemoveRagdoll();
				//if (GUILayout.Button("Make Profi"))
				//	ConvertToSmartRagdoll();

				_ragdollTotalWeight = EditorGUILayout.IntField("Total Weight:", _ragdollTotalWeight);

				_ragdollProperties.Draw();
			}
			else
			{
				GUILayout.Label("Ragdoll creator supported only for humanoids");
			}

			GUILayout.EndVertical();
		}

		/// <summary>
		/// Remove all colliders, joints, and rigids from "_go" object
		/// </summary>
		private void RemoveRagdoll()
		{
			Ragdoller ragdoller = new Ragdoller(_go.transform, _getPlayerDirection());
			ragdoller.ClearRagdoll();
		}

		/// <summary>
		/// Create Ragdoll components on _go object
		/// </summary>
		private void CreateRagdoll()
		{
			Ragdoller ragdoller = new Ragdoller(_go.transform, _getPlayerDirection());
			ragdoller.ApplyRagdoll(_ragdollTotalWeight, _ragdollProperties);
		}

		//void ConvertToSmartRagdoll()
		//{
		//	Ragdoller ragdoller = new Ragdoller(_go.transform, _getPlayerDirection());
		//	ragdoller.ClearRagdoll();
		//}
	}
}