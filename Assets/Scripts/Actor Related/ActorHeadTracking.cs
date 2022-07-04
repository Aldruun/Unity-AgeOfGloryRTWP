using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ActorHeadTracking : MonoBehaviour
{
    private Animator _animator;

    public Vector3 lookAtPosition;
    public float lookAtWeight = 1f;
    public float bodyWeight = 0f;
    public float headWeight = 0.8f;
    public float eyesWeight = 1f;

    public float clampWeight = 0.5f;
    
    public float lookSpeed = 7f;
    public float GotTargetLookSpeed = 0.4f;
    public float lostTargetLookSpeed = 0.4f;

    private bool isAimingRangedWeapon;

    private Vector3 _currentLookAtPosition;

    private Transform head;
    //Transform _upperBodyTarget;

    //public bool useBodyIKTarget;

    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
        head = _animator.GetBoneTransform(HumanBodyBones.Head);
        //_upperBodyTarget = transform.Find("bodyIKTarget");
    }

    // private void LateUpdate()
    // {
    //     head.rotation = Quaternion.Euler(0, head.rotation.y, 0);
    // }

    private void OnAnimatorIK(int layerIndex)
    {
        //if(useBodyIKTarget && _upperBodyTarget != null)
        //{
        //    _animator.ik( AvatarIKHint.);
        //}

        if (lookAtPosition == Vector3.zero) return;
        lookAtPosition.y = head.position.y;
        
        if(isAimingRangedWeapon)
        {
            _currentLookAtPosition = Vector3.Lerp(_currentLookAtPosition, lookAtPosition, Time.deltaTime * 5);
            _animator.SetLookAtPosition(_currentLookAtPosition);
            _animator.SetLookAtWeight(1, 1, headWeight, eyesWeight, clampWeight);

        }
        else
        {
            _currentLookAtPosition = Vector3.Lerp(_currentLookAtPosition, lookAtPosition, Time.deltaTime * lookSpeed);
            _animator.SetLookAtPosition(_currentLookAtPosition);
            _animator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyesWeight, clampWeight);

            //_animator.SetIKPosition();

        }

        Vector3 dir = lookAtPosition - transform.position;
        dir.y = head.position.y;
        float dist = dir.magnitude;
        if(GenericFunctions.Get.IsInFOV(transform, lookAtPosition, 90) && dist > 1f)
        {
            clampWeight = Mathf.MoveTowards(clampWeight, 0.7f, GotTargetLookSpeed * Time.deltaTime);
            //clampWeight = Mathf.Clamp01(clampWeight + lostTargetTurnSpeed * Time.deltaTime);
        }
        else
        {
            clampWeight = Mathf.MoveTowards(clampWeight, 1f, lostTargetLookSpeed * Time.deltaTime);
            //clampWeight = Mathf.Clamp(clampWeight - lostTargetTurnSpeed * Time.deltaTime, 0, 0.5f);
        }
    }

    public void SetLooAtWeight(float weight)
    {
        if(lookAtWeight != weight)
            lookAtWeight = weight;
    }

    internal void SetIsAimingRangedWeapon(bool on)
    {
        isAimingRangedWeapon = on;
    }
}
