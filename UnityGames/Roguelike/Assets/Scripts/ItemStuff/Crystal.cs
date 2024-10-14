using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Crystal : Collectable
{




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RandomForce();
    }

    public void FixedUpdate()
    {
        Attraction();
    }




    public void SetValue(int num)
    {
        Value = num;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Player>().IncreaseCrystals(Value);
            gameObject.SetActive(false);
        }
    }
}
