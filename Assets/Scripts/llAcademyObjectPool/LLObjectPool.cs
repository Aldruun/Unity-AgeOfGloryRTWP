using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LLObjectPool : MonoBehaviour
{
    private LLPoolableObject Prefab;
    private int Size;
    private List<LLPoolableObject> AvailableObjectsPool;

    private LLObjectPool(LLPoolableObject Prefab, int Size)
    {
        this.Prefab = Prefab;
        this.Size = Size;
        AvailableObjectsPool = new List<LLPoolableObject>(Size);
    }

    public static LLObjectPool CreateInstance(LLPoolableObject Prefab, int Size)
    {
        LLObjectPool pool = new LLObjectPool(Prefab, Size);

        GameObject poolGameObject = new GameObject(Prefab + " Pool");
        pool.CreateObjects(poolGameObject);

        return pool;
    }

    private void CreateObjects(GameObject parent)
    {
        for(int i = 0; i < Size; i++)
        {
            LLPoolableObject poolableObject = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity, parent.transform);
            poolableObject.Parent = this;
            poolableObject.gameObject.SetActive(false); // PoolableObject handles re-adding the object to the AvailableObjects
        }
    }

    public LLPoolableObject GetObject()
    {
        LLPoolableObject instance = AvailableObjectsPool[0];

        AvailableObjectsPool.RemoveAt(0);

        instance.gameObject.SetActive(true);

        return instance;
    }

    public void ReturnObjectToPool(LLPoolableObject Object)
    {
        AvailableObjectsPool.Add(Object);
    }
}
