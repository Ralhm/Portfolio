using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform focus;
    public float smoothTime = 2;

    Vector3 offset;

    void Start()
    {
        focus = GameObject.Find("Player").GetComponent<Transform>();
        transform.position = focus.transform.position + new Vector3(0, 0, -10f);


        offset = focus.position - transform.position;

        
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, focus.position - offset, Time.deltaTime * smoothTime);
    }
}
