using GenericFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RagdollBuilder : Editor
{
    //static Animator animator;

    private static Transform root;

    private static Transform leftHips;
    private static Transform leftKnee;
    private static Transform leftFoot;

    private static Transform rightHips;
    private static Transform rightKnee;
    private static Transform rightFoot;

    private static Transform leftArm;
    private static Transform leftElbow;

    private static Transform rightArm;
    private static Transform rightElbow;

    private static Transform middleSpine;
    private static Transform head;


    private static readonly float totalMass = 20;
    private static readonly float strength = 0.0F;

    private static Vector3 right = Vector3.right;
    private static Vector3 up = Vector3.up;
    private static Vector3 forward = Vector3.forward;

    private static Vector3 worldRight = Vector3.right;
    private static Vector3 worldUp = Vector3.up;
    private static Vector3 worldForward = Vector3.forward;
    private static readonly bool flipForward = false;

    private static List<BoneInfo> bones;

    //ArrayList bones;
    private static BoneInfo rootBone;

    private static Animator _animator;

    private static List<Transform> _bonesToProcess;

    private string CheckConsistency()
    {
        //PrepareBones();
        Hashtable map = new Hashtable();
        foreach(BoneInfo bone in bones)
            if(bone.anchor)
            {
                if(map[bone.anchor] != null)
                {
                    BoneInfo oldBone = (BoneInfo)map[bone.anchor];
                    return string.Format("{0} and {1} may not be assigned to the same bone.", bone.name, oldBone.name);
                }

                map[bone.anchor] = bone;
            }

        foreach(BoneInfo bone in bones)
            if(bone.anchor == null)
                return string.Format("{0} not assigned yet.\n", bone.name);

        return "";
    }

    private void OnDrawGizmos()
    {
        if(root)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(root.position, root.TransformDirection(right));
            Gizmos.color = Color.green;
            Gizmos.DrawRay(root.position, root.TransformDirection(up));
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(root.position, root.TransformDirection(forward));
        }
    }

    [MenuItem("AoG Utilities/Create Ragdoll/Generic Humanoid")]
    private static void CreateGenericHumanoidWizard()
    {
        CreateRagdoll(Selection.activeTransform);
    }
    [MenuItem("AoG Utilities/Create Ragdoll/Generic Creature")]
    private static void CreateGenericCreatureWizard()
    {
        //CreateRagdoll(Selection.activeTransform);
        Transform transform = Selection.activeTransform;
        _animator = transform.GetComponentInChildren<Animator>();

        _bonesToProcess = new List<Transform>();
        //Transform[] children = transform.GetComponentsInChildren<Transform>();
        //foreach(Transform t in children)
        //{
        //    if(t.childCount == 0)
        //        continue;

        //    if(t.name.IndexOf("head", StringComparison.OrdinalIgnoreCase) >= 0)
        //    {
        //        _bonesToProcess.Add(t);
        //    }
        //    else if(t.name.IndexOf("spine", StringComparison.OrdinalIgnoreCase) >= 0)
        //    {
        //        _bonesToProcess.Add(t);
        //    }
        //    else if(t.name.IndexOf("hips", StringComparison.OrdinalIgnoreCase) >= 0)
        //    {
        //        _bonesToProcess.Add(t);
        //    }
        //    else if(t.name.IndexOf("wing", StringComparison.OrdinalIgnoreCase) >= 0)
        //    {
        //        _bonesToProcess.Add(t);
        //    }
        //    else if(t.name.IndexOf("leg", StringComparison.OrdinalIgnoreCase) >= 0)
        //    {
        //        _bonesToProcess.Add(t);
        //    }
        //    else if(t.name.IndexOf("tail", StringComparison.OrdinalIgnoreCase) >= 0)
        //    {
        //        _bonesToProcess.Add(t);
        //    }
        //}

        CreateRagdoll(transform, true);

        //foreach(Transform bone in _bonesToProcess)
        //{

            //bone.EnsureHasComponent<Rigidbody>();
            //bone.gameObject.layer = LayerMask.NameToLayer("Bodyparts");
            //BoxCollider bc = bone.ForceGetComponent<BoxCollider>();
            //bc.size = Vector3.one * 0.1f;
            //CharacterJoint joint = bone.ForceGetComponent<CharacterJoint>();

            //joint.autoConfigureConnectedAnchor = true;
            //joint.axis = new Vector3(1, 0, 0);
            //joint.swingAxis = new Vector3(0, 0, 1);
            //joint.enableCollision = false;
            //joint.enablePreprocessing = true;
            //joint.enableProjection = true;
            //joint.projectionDistance = 0.1f;
            //joint.projectionAngle = 180;
            //joint.massScale = 1;
            //joint.connectedMassScale = 1;
            //joint.breakForce = Mathf.Infinity;
            //joint.breakTorque = Mathf.Infinity;

            //joint.axis = CalculateDirectionAxis(bone.InverseTransformDirection(joint.axis));
            ////joint.swingAxis = CalculateDirectionAxis(bone.InverseTransformDirection(joint.normalAxis));
            //joint.anchor = Vector3.zero;
            //joint.connectedBody = bone.parent.ForceGetComponent<Rigidbody>();

            //// Setup limits			
            //SoftJointLimit limit = new SoftJointLimit();

            //limit.limit = -20;
            //joint.lowTwistLimit = limit;

            //limit.limit = 20;
            //joint.highTwistLimit = limit;

            //limit.limit = 10;
            //joint.swing1Limit = limit;

            //limit.limit = 0;
            //joint.swing2Limit = limit;

        //}
    }

    private void AddBone(Transform bone, List<Transform> bones)
    {
        if(bone != null)
            bones.Add(bone);
    }

    private static void SetupBones(Transform transform)
    {
        root = transform.FindDeepChild("hips");
        if(root == null)
            Debug.LogError("root = null");

        head = transform.FindDeepChild("head");
        if(head == null)
            Debug.LogError("head = null");

        middleSpine = transform.FindDeepChild("chest");
        if(middleSpine == null)
        {
            //middleSpine = transform.FindDeepChild("spine");
            Debug.LogError("chest = null"); 
        }

        leftArm = transform.Child("upper_arm.L", true, false);
        if(leftArm == null)
            Debug.LogError("upper_arm = null");

        leftElbow = transform.Child("forearm.L", true, false);
        if(leftElbow == null)
            Debug.LogError("l forearm = null");

        leftHips = transform.Child("thigh.L", true, false);
        if(leftHips == null)
            Debug.LogError("l thigh = null");

        leftKnee = transform.Child("shin.L", true, false);
        if(leftKnee == null)
            Debug.LogError("l shin = null");

        leftFoot = transform.Child("foot.L", true, false);
        if(leftFoot == null)
            Debug.LogError("l foot = null");

        rightArm = transform.Child("upper_arm.R", true, false);
        if(rightArm == null)
            Debug.LogError("r upper_arm = null");

        rightElbow = transform.Child("forearm.R", true, false);
        if(rightElbow == null)
            Debug.LogError("r forearm = null");

        rightHips = transform.Child("thigh.R", true, false);
        if(rightHips == null)
            Debug.LogError("r thigh = null");

        rightKnee = transform.Child("shin.R", true, false);
        if(rightKnee == null)
            Debug.LogError("r shin = null");

        rightFoot = transform.Child("foot.L", true, false);
        if(rightFoot == null)
            Debug.LogError("r foot = null");

    }

    private static void SetupBonesGeneric(Transform transform)
    {
        List<Transform> genericBones = new List<Transform>();
        Debug.Log("Setting up generic ragdoll");
        root = transform.FindDeepChild("hips");
        if(root == null)
            Debug.LogError("root = null");
        else
        {
            genericBones.Add(root);
        }

        head = transform.FindDeepChild("head");
        if(head == null)
            Debug.LogError("head = null");
        else
        {
            genericBones.Add(head);
        }

        middleSpine = transform.FindDeepChild("chest");
        
        if(middleSpine == null)
        {
            middleSpine = transform.Child("spine", true, false);
            //middleSpine = transform.FindDeepChild("spine");
            if(middleSpine == null)
                Debug.LogError("chest = null");
            else
                genericBones.Add(middleSpine);
        }
        else
            genericBones.Add(middleSpine);
        

        leftArm = transform.Child("front_thigh.L", true, false);
        if(leftArm == null)
            Debug.LogError("upper_arm = null");
        else
            genericBones.Add(leftArm);

        leftElbow = transform.Child("front_shin.L", true, false);
        if(leftElbow == null)
            Debug.LogError("l forearm = null");
        else
            genericBones.Add(leftElbow);

        leftHips = transform.Child("thigh.L", true, false);
        if(leftHips == null)
            Debug.LogError("l thigh = null");
        else
            genericBones.Add(leftHips);

        leftKnee = transform.Child("shin.L", true, false);
        if(leftKnee == null)
            Debug.LogError("l shin = null");
        else
            genericBones.Add(leftKnee);
        leftFoot = transform.Child("foot.L", true, false);
        if(leftFoot == null)
            Debug.LogError("l foot = null");
        else
            genericBones.Add(leftFoot);
        rightArm = transform.Child("front_thigh.R", true, false);
        if(rightArm == null)
            Debug.LogError("r upper_arm = null");
        else
            genericBones.Add(rightArm);
        rightElbow = transform.Child("front_shin.R", true, false);
        if(rightElbow == null)
            Debug.LogError("r forearm = null");
        else
            genericBones.Add(rightElbow);
        rightHips = transform.Child("thigh.R", true, false);
        if(rightHips == null)
            Debug.LogError("r thigh = null");
        else
            genericBones.Add(rightHips);
        rightKnee = transform.Child("shin.R", true, false);
        if(rightKnee == null)
            Debug.LogError("r shin = null");
        else
            genericBones.Add(rightKnee);
        rightFoot = transform.Child("foot.L", true, false);
        if(rightFoot == null)
            Debug.LogError("r foot = null");
        else
            genericBones.Add(rightFoot);

        foreach(Transform bone in genericBones)
        {
            Rigidbody rb = bone.GetComponent<Rigidbody>();
            if(rb == null)
                rb = bone.gameObject.AddComponent<Rigidbody>();

            rb.isKinematic = true;

            CharacterJoint joint = bone.GetComponent<CharacterJoint>();
            if(joint == null)
                joint = bone.gameObject.AddComponent<CharacterJoint>();

            BoxCollider col = bone.GetComponent<BoxCollider>();
            if(col == null)
                col = bone.gameObject.AddComponent<BoxCollider>();

            col.size = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }

    private void DecomposeVector(out Vector3 normalCompo, out Vector3 tangentCompo, Vector3 outwardDir,
        Vector3 outwardNormal)
    {
        outwardNormal = outwardNormal.normalized;
        normalCompo = outwardNormal * Vector3.Dot(outwardDir, outwardNormal);
        tangentCompo = outwardDir - normalCompo;
    }

    private void CalculateAxes()
    {
        if(head != null && root != null)
            up = CalculateDirectionAxis(root.InverseTransformPoint(head.position));
        if(rightElbow != null && root != null)
        {
            Vector3 removed, temp;
            DecomposeVector(out temp, out removed, root.InverseTransformPoint(rightElbow.position), up);
            right = CalculateDirectionAxis(removed);
        }

        forward = Vector3.Cross(right, up);
        if(flipForward)
            forward = -forward;
    }

    private void OnWizardUpdate()
    {
        //errorString = CheckConsistency();
        CalculateAxes();

        //if(errorString.Length != 0) {
        //    helpString = "Drag all bones from the hierarchy into their slots.\nMake sure your character is in T-Stand.\n";
        //}
        //else {
        //    helpString = "Make sure your character is in T-Stand.\nMake sure the blue axis faces in the same direction the chracter is looking.\nUse flipForward to flip the direction";
        //}

        //isValid = true;
    }

    private static void PrepareBones(Transform transform, bool generic)
    {
        Debug.Log("Preparing bones");
        if(generic == false)
            SetupBones(transform);
        else
        {
            SetupBonesGeneric(transform);
            return;
        }

        bones = new List<BoneInfo>();

        if(root != null)
        {
            worldRight = root.TransformDirection(right);
            worldUp = root.TransformDirection(up);
            worldForward = root.TransformDirection(forward);
            rootBone = new BoneInfo();
            rootBone.name = "Root";
            rootBone.anchor = root;
            rootBone.parent = null;
            rootBone.density = 2.5F;
            bones.Add(rootBone);
        }
        else
        {
            Debug.LogError("No root found");
        }


        if(generic)
        {
            foreach(var t in _bonesToProcess)
            {
                AddJoint("", t, t.parent != null ? t.parent.name : "", worldRight, worldForward, -20, 20, 10, typeof(CapsuleCollider), 1, 2.5F);
            }
            return;
        }

        AddMirroredJoint("Hips", leftHips, rightHips, "Root", worldRight, worldForward, -20, 70, 30,
            typeof(CapsuleCollider), 0.3F, 1.5F);
        AddMirroredJoint("Knee", leftKnee, rightKnee, "Hips", worldRight, worldForward, -80, 0, 0,
            typeof(CapsuleCollider), 0.25F, 1.5F);
        //		AddMirroredJoint ("Hips", leftHips, rightHips, "Root", worldRight, worldForward, -0, -70, 30, typeof(CapsuleCollider), 0.3F, 1.5F);
        //		AddMirroredJoint ("Knee", leftKnee, rightKnee, "Hips", worldRight, worldForward, -0, -50, 0, typeof(CapsuleCollider), .25F, 1.5F);

        AddJoint("Middle Spine", middleSpine, "Root", worldRight, worldForward, -20, 20, 10, null, 1, 2.5F);

        AddMirroredJoint("Arm", leftArm, rightArm, "Middle Spine", worldUp, worldForward, -70, 10, 50,
            typeof(CapsuleCollider), 0.25F, 1.0F);
        AddMirroredJoint("Elbow", leftElbow, rightElbow, "Arm", worldForward, worldUp, -90, 0, 0,
            typeof(CapsuleCollider), 0.20F, 1.0F);

        AddJoint("Head", head, "Middle Spine", worldRight, worldForward, -40, 25, 25, null, 1, 1.0F);

    }

    private static void OnWizardCreate(bool generic)
    {
        Cleanup();
        BuildCapsules();

        if(generic == false)
        {
            AddBreastColliders();
            AddHeadCollider();
        }

        BuildBodies();
        BuildJoints(bones);

        if(generic == false)
        {
            CalculateMass();
        }

        CalculateSpringDampers();
    }

    public static void CreateRagdoll(Transform transform, bool generic = false)
    {
        Debug.Log("Creating ragdoll");
        PrepareBones(transform, generic);
        OnWizardCreate(generic);
    }

    private static BoneInfo FindBone(string name)
    {
        foreach(BoneInfo bone in bones)
            if(bone.name == name)
                return bone;
        return null;
    }

    private static void AddMirroredJoint(string name, Transform leftAnchor, Transform rightAnchor, string parent,
        Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit,
        Type colliderType, float radiusScale, float density)
    {
        AddJoint("Left " + name, leftAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit,
            colliderType, radiusScale, density);
        AddJoint("Right " + name, rightAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit,
            colliderType, radiusScale, density);
    }


    private static void AddJoint(string name, Transform anchor, string parent, Vector3 worldTwistAxis,
        Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale,
        float density)
    {
        if(anchor == null)
        {
            return;
        }

        anchor.gameObject.layer = LayerMask.NameToLayer("Bodyparts");
        BoneInfo bone = new BoneInfo();
        bone.name = name;
        bone.anchor = anchor;
        bone.axis = worldTwistAxis;
        bone.normalAxis = worldSwingAxis;
        bone.minLimit = minLimit;
        bone.maxLimit = maxLimit;
        bone.swingLimit = swingLimit;
        bone.density = density;
        bone.colliderType = colliderType;
        bone.radiusScale = radiusScale;


        if(FindBone(parent) != null)
            bone.parent = FindBone(parent);
        else if(name.StartsWith("Left"))
            bone.parent = FindBone("Left " + parent);
        else if(name.StartsWith("Right"))
            bone.parent = FindBone("Right " + parent);

        if(bone.parent != null)
            bone.parent.children.Add(bone);
        bones.Add(bone);
    }

    private static void BuildCapsules()
    {
        foreach(BoneInfo bone in bones)
        {
            if(bone.colliderType != typeof(CapsuleCollider) || bone.parent == null)
                continue;

            int direction;
            float distance;
            if(bone.children.Count == 1)
            {
                BoneInfo childBone = (BoneInfo)bone.children[0];
                Vector3 endPoint = childBone.anchor.position;
                CalculateDirection(bone.anchor.InverseTransformPoint(endPoint), out direction, out distance);
            }
            else
            {
                Vector3 endPoint = bone.anchor.position - bone.parent.anchor.position + bone.anchor.position;
                CalculateDirection(bone.anchor.InverseTransformPoint(endPoint), out direction, out distance);

                if(bone.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
                {
                    Bounds bounds = new Bounds();
                    foreach(Transform child in bone.anchor.GetComponentsInChildren(typeof(Transform)))
                        bounds.Encapsulate(bone.anchor.InverseTransformPoint(child.position));

                    if(distance > 0)
                        distance = bounds.max[direction];
                    else
                        distance = bounds.min[direction];
                }
            }

            CapsuleCollider collider = bone.anchor.gameObject.AddComponent<CapsuleCollider>();
            collider.direction = direction;

            Vector3 center = Vector3.zero;
            center[direction] = distance * 0.5F;
            collider.center = center;
            collider.height = Mathf.Abs(distance);
            collider.radius = Mathf.Abs(distance * bone.radiusScale);
        }
    }

    private static void Cleanup()
    {
        foreach(BoneInfo bone in bones)
        {
            if(!bone.anchor)
                continue;

            Component[] joints = bone.anchor.GetComponentsInChildren(typeof(Joint));
            foreach(Joint joint in joints)
                DestroyImmediate(joint);

            Component[] bodies = bone.anchor.GetComponentsInChildren(typeof(Rigidbody));
            foreach(Rigidbody body in bodies)
                DestroyImmediate(body);

            Component[] colliders = bone.anchor.GetComponentsInChildren(typeof(Collider));
            foreach(Collider collider in colliders)
                DestroyImmediate(collider);
        }
    }

    private static void BuildBodies()
    {
        foreach(BoneInfo bone in bones)
        {
            Rigidbody rb = bone.anchor.GetComponent<Rigidbody>();

            if(rb == null)
            {
                rb = bone.anchor.gameObject.AddComponent<Rigidbody>();
            }
            //			bone.anchor.rigidbody.SetDensity (bone.density);
            rb.mass = bone.density;
            rb.isKinematic = true;
        }
    }

    private static void BuildJoints(List<BoneInfo> bones)
    {
        foreach(BoneInfo bone in bones)
        {
            if(bone.parent == null)
                continue;

            CharacterJoint joint = bone.anchor.gameObject.AddComponent<CharacterJoint>();
            joint.enableProjection = true;
            bone.joint = joint;

            // Setup connection and axis
            joint.axis = CalculateDirectionAxis(bone.anchor.InverseTransformDirection(bone.axis));
            joint.swingAxis = CalculateDirectionAxis(bone.anchor.InverseTransformDirection(bone.normalAxis));
            joint.anchor = Vector3.zero;
            joint.connectedBody = bone.parent.anchor.GetComponent<Rigidbody>();

            // Setup limits			
            SoftJointLimit limit = new SoftJointLimit();

            limit.limit = bone.minLimit;
            joint.lowTwistLimit = limit;

            limit.limit = bone.maxLimit;
            joint.highTwistLimit = limit;

            limit.limit = bone.swingLimit;
            joint.swing1Limit = limit;

            limit.limit = 0;
            joint.swing2Limit = limit;
        }
    }

    private static void CalculateMassRecurse(BoneInfo bone)
    {
        float mass = bone.anchor.GetComponent<Rigidbody>().mass;
        foreach(BoneInfo child in bone.children)
        {
            CalculateMassRecurse(child);
            mass += child.summedMass;
        }

        bone.summedMass = mass;
    }

    private static void CalculateMass()
    {
        // Calculate allChildMass by summing all bodies
        CalculateMassRecurse(rootBone);

        // Rescale the mass so that the whole character weights totalMass
        float massScale = totalMass / rootBone.summedMass;
        foreach(BoneInfo bone in bones)
            bone.anchor.GetComponent<Rigidbody>().mass *= massScale;

        // Recalculate allChildMass by summing all bodies
        CalculateMassRecurse(rootBone);
    }

    //TO DO: This should take into account the inertia tensor.
    private static JointDrive CalculateSpringDamper(float frequency, float damping, float mass)
    {
        JointDrive drive = new JointDrive();
        drive.positionSpring = 9 * frequency * frequency * mass;
        drive.positionDamper = 4.5F * frequency * damping * mass;
        return drive;
    }

    private static void CalculateSpringDampers()
    {
        // Calculate the rotation drive based on the strength and how much mass the character needs to pull around.
        foreach(BoneInfo bone in bones)
        {
//            if(bone.joint)
//#pragma warning disable CS0618 // 'CharacterJoint.rotationDrive' is obsolete: 'RotationDrive not in use for Unity 5 and assumed disabled.'
//                bone.joint.rotationDrive = CalculateSpringDamper(strength / 100.0F, 1, bone.summedMass);
//#pragma warning restore CS0618 // 'CharacterJoint.rotationDrive' is obsolete: 'RotationDrive not in use for Unity 5 and assumed disabled.'
        }
    }
    /*	

        void AddJoint (string name, Complexity complexity, Transform anchor, Transform connectTo, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, float mass)
        {
            if (!connectTo.rigidbody)
                connectTo.gameObject.AddComponent("Rigidbody");

            CharacterJoint joint = (CharacterJoint)anchor.gameObject.AddComponent ("CharacterJoint");

            joint.axis = CalculateDirectionAxis (anchor.InverseTransformDirection(worldTwistAxis));
            joint.swingAxis = CalculateDirectionAxis (anchor.InverseTransformDirection(worldSwingAxis));
            joint.anchor = Vector3.zero;
            joint.connectedBody = connectTo.rigidbody;

            SoftJointLimit limit = new SoftJointLimit ();

            limit.limit = minLimit;
            joint.lowTwistLimit = limit;

            limit.limit = maxLimit;
            joint.highTwistLimit = limit;

            limit.limit = swingLimit;
            joint.swing1Limit = limit;

            limit.limit = 0;
            joint.swing2Limit = limit;

            JointDrive drive = new JointDrive ();
            drive.spring = 0.2F;
            drive.damper = .1F;
            drive.force = 10.0F;
            joint.rotationDrive = drive;

            connectTo.rigidbody.mass = 2;
            anchor.rigidbody.mass = 2;
        }
        /*
        void BuildCapsule (BoneInfo bone)
        {
            CapsuleCollider collider = (CapsuleCollider)bone.body.gameObject.AddComponent ("CapsuleCollider");

            Bounds bounds;
            if (Editor.CalculateSkinnedAABB (bone.body, bone.body, out bounds))
            {
                int direction;
                float distance;
                CalculateDirection (bounds.max, out direction, out distance);

                collider.direction = direction;
                collider.height = distance;
                collider.radius = SecondLargestComponent ();
            }
            else
            {

            }

            if (bone.children.Count == 1)
            {

            }
        }
        */
    /*	
 
	void AddCapsule (Transform anchor, Transform parent, Transform next, float directionScale, float radiusScale)
	{
		if (anchor.collider)
			Destroy (anchor.collider);
 
		Vector3 endPoint;
 
		if (next)
			endPoint = next.position;
		else
			endPoint = directionScale * (anchor.position - parent.position) + anchor.position;
 
		int direction;
		float distance;
		CalculateDirection (anchor.InverseTransformPoint(endPoint), out direction, out distance);
		distance = distance / anchor.lossyScale[direction];
 
		CapsuleCollider collider = (CapsuleCollider)anchor.gameObject.AddComponent ("CapsuleCollider");
		collider.direction = direction;
 
		Vector3 center = Vector3.zero;
		center[direction] = distance * 0.5F;
		collider.center = center;
		collider.height = Mathf.Abs (distance);
		collider.radius = Mathf.Abs (distance * radiusScale);
	}*/

    private static void CalculateDirection(Vector3 point, out int direction, out float distance)
    {
        // Calculate longest axis
        direction = 0;
        if(Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
            direction = 1;
        if(Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
            direction = 2;

        distance = point[direction];
    }

    private static Vector3 CalculateDirectionAxis(Vector3 point)
    {
        int direction = 0;
        float distance;
        CalculateDirection(point, out direction, out distance);
        Vector3 axis = Vector3.zero;
        if(distance > 0)
            axis[direction] = 1.0F;
        else
            axis[direction] = -1.0F;
        return axis;
    }

    private static int SmallestComponent(Vector3 point)
    {
        int direction = 0;
        if(Mathf.Abs(point[1]) < Mathf.Abs(point[0]))
            direction = 1;
        if(Mathf.Abs(point[2]) < Mathf.Abs(point[direction]))
            direction = 2;
        return direction;
    }

    private static int LargestComponent(Vector3 point)
    {
        int direction = 0;
        if(Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
            direction = 1;
        if(Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
            direction = 2;
        return direction;
    }

    private static int SecondLargestComponent(Vector3 point)
    {
        int smallest = SmallestComponent(point);
        int largest = LargestComponent(point);
        if(smallest < largest)
        {
            int temp = largest;
            largest = smallest;
            smallest = temp;
        }

        if(smallest == 0 && largest == 1)
            return 2;
        if(smallest == 0 && largest == 2)
            return 1;
        return 0;
    }

    private static Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
    {
        int axis = LargestComponent(bounds.size);

        if(Vector3.Dot(worldUp, relativeTo.TransformPoint(bounds.max)) >
            Vector3.Dot(worldUp, relativeTo.TransformPoint(bounds.min)) == below)
        {
            Vector3 min = bounds.min;
            min[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
            bounds.min = min;
        }
        else
        {
            Vector3 max = bounds.max;
            max[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
            bounds.max = max;
        }

        return bounds;
    }

    private static Bounds GetBreastBounds(Transform relativeTo)
    {
        // Root bounds
        Bounds bounds = new Bounds();
        bounds.Encapsulate(relativeTo.InverseTransformPoint(leftHips.position));
        bounds.Encapsulate(relativeTo.InverseTransformPoint(rightHips.position));
        bounds.Encapsulate(relativeTo.InverseTransformPoint(leftArm.position));
        bounds.Encapsulate(relativeTo.InverseTransformPoint(rightArm.position));
        Vector3 size = bounds.size;
        size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2.0F;
        bounds.size = size;
        return bounds;
    }

    private static void AddBreastColliders()
    {
        // Middle spine and root
        if(middleSpine != null && root != null)
        {
            Bounds bounds;
            BoxCollider box;

            // Middle spine bounds
            bounds = Clip(GetBreastBounds(root), root, middleSpine, false);
            box = root.gameObject.AddComponent<BoxCollider>();
            box.center = bounds.center;
            box.size = bounds.size;

            bounds = Clip(GetBreastBounds(middleSpine), middleSpine, middleSpine, true);
            box = middleSpine.gameObject.AddComponent<BoxCollider>();
            box.center = bounds.center;
            box.size = bounds.size;
        }
        // Only root
        else if(root != null)
        {
            Bounds bounds = new Bounds();
            bounds.Encapsulate(root.InverseTransformPoint(leftHips.position));
            bounds.Encapsulate(root.InverseTransformPoint(rightHips.position));
            bounds.Encapsulate(root.InverseTransformPoint(leftArm.position));
            bounds.Encapsulate(root.InverseTransformPoint(rightArm.position));

            Vector3 size = bounds.size;
            size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2.0F;

            BoxCollider box = root.gameObject.AddComponent<BoxCollider>();
            box.center = bounds.center;
            box.size = size;
        }
    }

    private static void AddHeadCollider()
    {
        if(head.GetComponent<Collider>())
            Destroy(head.GetComponent<Collider>());

        float radius = Vector3.Distance(leftArm.transform.position, rightArm.transform.position);
        radius /= 4;

        SphereCollider sphere = head.gameObject.AddComponent<SphereCollider>();
        sphere.radius = radius;
        Vector3 center = Vector3.zero;

        int direction;
        float distance;
        CalculateDirection(head.InverseTransformPoint(root.position), out direction, out distance);
        if(distance > 0)
            center[direction] = -radius;
        else
            center[direction] = radius;
        sphere.center = center;
    }

    private class BoneInfo
    {
        public Transform anchor;

        public Vector3 axis;

        public readonly ArrayList children = new ArrayList();
        public Type colliderType;
        public float density;
        public CharacterJoint joint;
        public float maxLimit;

        public float minLimit;

        public string name;
        public Vector3 normalAxis;
        public BoneInfo parent;

        public float radiusScale;
        public float summedMass; // The mass of this and all children bodies
        public float swingLimit;
    }
}