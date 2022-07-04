using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAt : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        //if(isShip) {

        //transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, transform.position - Camera.main.transform.position);
        //Vector3 v = Camera.main.transform.position - transform.position;
        //v.x = v.z = 0.0f;
        //transform.LookAt(Camera.main.transform.position - v);
        transform.LookAt(
            transform.position + _camera.transform.rotation * Vector3.forward, /*Camera.main.transform.rotation * */
            _camera.transform.up);
        //}
        //else {

        //    transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
        //    Camera.main.transform.rotation * Vector3.up);
        //}
    }
}
