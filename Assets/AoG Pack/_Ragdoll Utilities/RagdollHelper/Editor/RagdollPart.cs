using UnityEngine;

namespace BzKovSoft.RagdollHelper.Editor
{
    /// <summary>
    /// Abstract class. Represents ragdoll part. For example: legUpper, head, pelvis or something else
    /// </summary>
    internal abstract class RagdollPartBase
	{
		public readonly Transform transform;
		public Rigidbody rigidbody;
		public CharacterJoint joint;

		protected RagdollPartBase(Transform transform)
		{
			this.transform = transform;
		}
	}
	/// <summary>
	/// Ragoll part for Box collider
	/// </summary>
	internal sealed class RagdollPartBox : RagdollPartBase
	{
		public BoxCollider collider;

		public RagdollPartBox(Transform transform) : base(transform) { }
	}
	/// <summary>
	/// Ragoll part for Capsule collider
	/// </summary>
	internal sealed class RagdollPartCapsule : RagdollPartBase
	{
		public CapsuleCollider collider;

		public RagdollPartCapsule(Transform transform) : base(transform) { }
	}
	/// <summary>
	/// Ragoll part for Sphere collider
	/// </summary>
	internal sealed class RagdollPartSphere : RagdollPartBase
	{
		public SphereCollider collider;

		public RagdollPartSphere(Transform transform) : base(transform) { }
	}
}