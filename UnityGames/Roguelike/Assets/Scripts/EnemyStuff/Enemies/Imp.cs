using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RB.velocity = Seek(player.RB) * Speed;

        if (RB.velocity.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
