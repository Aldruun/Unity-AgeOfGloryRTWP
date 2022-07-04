using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LLPoolableObject : MonoBehaviour
{
    public LLObjectPool Parent;

    public virtual void OnDisable()
    {
        Parent.ReturnObjectToPool(this);
    }
}
