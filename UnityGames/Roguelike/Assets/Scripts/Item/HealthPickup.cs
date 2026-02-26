using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Collectable
{
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RandomForce();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Player>().RestoreHealth(Value);
            gameObject.SetActive(false);
        }
    }
}
