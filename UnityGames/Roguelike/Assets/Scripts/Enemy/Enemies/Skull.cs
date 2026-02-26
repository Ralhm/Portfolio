using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Enemy
{
    public bool MoveTowardsPlayer;
    public float MoveForce;



    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
        RB = GetComponent<Rigidbody2D>();
        RB.velocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        MoveDir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveDir = Wander();
        Accelerate(Time.deltaTime);
        if (MoveTowardsPlayer)
        {
            RB.AddForce(Seek(player.RB) * MoveForce);
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //Debug.DrawLine(RB.position, collision.gameObject.transform.position, Color.green, 5);
        MoveDir *= -MoveDir;
        RB.AddForce(-MoveDir * 100);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(ContactDamage);
        }

    }
}
