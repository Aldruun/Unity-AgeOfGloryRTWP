using UnityEngine;

public class MountPoint : MonoBehaviour
{
    public Transform m_mountPoint;

    //public WeaponType weaponType = WeaponType.OneHanded;

    public void MountObject(Transform obj)
    {
        obj.SetParent(m_mountPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    public void TransferToOtherMountPoint(MountPoint otherMountPoint)
    {
        otherMountPoint.MountObject(m_mountPoint.GetChild(0));
    }

    public void DestroyMountedObject()
    {
        Destroy(m_mountPoint.GetChild(0).gameObject);
    }
}