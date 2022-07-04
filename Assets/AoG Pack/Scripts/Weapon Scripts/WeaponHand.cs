using System;
using UnityEngine;

public class WeaponHand : MonoBehaviour
{
    public bool drawn;
    internal GameObject handVFX;
    public Action OnActivateSpell;
    public Weapon weapon { get; set; }
    public Spell spell { get; set; }
    public int handIndex { get; set; }

    public Transform handVFXAnchor { get; private set; }

    //public float recoveryCountdown { get; set; }
    public Transform attackVFXPoint { get; private set; }

    public void OnAttack(ActorInput owner, ActorInput target)
    {
        //Debug.Log(owner.agentData.Name + ": OnAttack");
        OnActivateSpell?.Invoke();
    }


    public void OnDraw()
    {
        ShowHandVFX();
    }

    public void OnHolster()
    {
        HideHandVFX();
    }

    public void SetHandVFX(string vfxID)
    {
        if (handVFX != null)
        {
            if (vfxID == handVFX.name)
                return;
            RemoveHandVFX();
        }

        handVFX = PoolSystem.GetPoolObject(vfxID, ObjectPoolingCategory.VFX);
        handVFX.transform.SetParent(handVFXAnchor);
        handVFX.transform.localPosition = Vector3.zero;
        handVFX.transform.localRotation = Quaternion.identity;
        //handVFX.SetActive(true);
    }

    public void RemoveHandVFX()
    {
        if (handVFX != null) // Reason: Could be called while NPC had no spell
            PoolSystem.StorePoolObject(handVFX.transform, ObjectPoolingCategory.VFX);
    }

    public void ShowHandVFX()
    {
        if (handVFX == null)
        {
            Debug.LogError("Can not show HandVFX -> null");
            return;
        }

        handVFX.SetActive(true);
    }

    public void HideHandVFX()
    {
        if (handVFX == null)
        {
            Debug.LogError("Can not hide HandVFX -> null");
            return;
        }

        handVFX.SetActive(false);
    }

    internal void SetAnchor(Transform transform)
    {
        handVFXAnchor = attackVFXPoint = transform;
        //handVFXAnchor.position = transform.position;
    }
}