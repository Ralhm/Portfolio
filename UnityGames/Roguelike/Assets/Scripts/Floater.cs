using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{

    public float frequency = 1;
    public float Distance = 0.1f;
    public float MinDistance = 3;
    protected float startingY;

    // Start is called before the first frame update
    void Start()
    {
        startingY = transform.localPosition.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Sin(Time.time * frequency) * Distance + startingY, transform.position.z);
    }
}
