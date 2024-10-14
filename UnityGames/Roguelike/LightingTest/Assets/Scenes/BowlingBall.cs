using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingBall : MonoBehaviour
{

    public Rigidbody RB;

    public float Force;

    //Update is callled once per frame
    void Update()
    {
        //Debug.Log("Bowling Ball Is Updating!!");

        Vector3 DirectionVector = new Vector3(0, 0.1f, 0);

        //Moves the player from side to side so you can aim
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime, 0, 0));



        //
        if (Input.GetKeyDown(KeyCode.Space)) //Throws the ball
        {
            Debug.Log("Holding SPace Down!!!");

            RB.AddForce(DirectionVector * Force);

        }
        //





    }







}
