using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowling2 : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {

        Vector3 DirectionVector = new Vector3(0, 0.1f, 0);

        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime, 0, 0));
    }
}
