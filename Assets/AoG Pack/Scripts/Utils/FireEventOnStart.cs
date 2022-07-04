using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireEventOnStart : MonoBehaviour
{
    public UnityEvent[] events;

    // Start is called before the first frame update
    private void Start()
    {
        foreach(var ev in events)
        {
            ev?.Invoke();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
