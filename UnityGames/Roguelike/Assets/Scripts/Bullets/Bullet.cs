using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Normal, Accelerating, Homing
}

public class Bullet : MonoBehaviour
{
    public int ContactDamage = 2;
    public float maxSpeed;
    Vector2 MoveDir;
    public BulletType type;

    public float lifeSpan;
    public float speed;
    public float acceleration;

    public Rigidbody2D RB;

    private void Start()
    {
        
    }

    void FixedUpdate()
    {

    }




    public void SetMoveDir(Vector2 Dir, float Angle = 0)
    {
        MoveDir = Quaternion.AngleAxis(Angle, Vector3.forward) * Dir;

    }

    public void SetVelocity()
    {
        RB.velocity = MoveDir * speed;
    }

    public void SetVelocity(Vector2 dir, float Angle = 0)
    {
        SetMoveDir(dir, Angle);
        RB.velocity = MoveDir * speed;
    }

    public void OnEnable()
    {
        //SetVelocity();
        StartCoroutine(DestroySelf());
    }

    public void Accelerate(float dt)
    {
        RB.velocity += acceleration * dt * MoveDir;

        RB.velocity = Vector2.ClampMagnitude(RB.velocity, maxSpeed);
    }

    public Vector2 Seek(Vector2 pos)
    {
        Vector2 dir = pos - RB.position;
        return dir.normalized;
    }



    public Vector2 Seek(Rigidbody2D rb)
    {
        Vector2 dir = rb.position - RB.position;
        return dir.normalized;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(ContactDamage);
        }
    }

    //Should use an object pool instead
    public IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(lifeSpan);
        gameObject.SetActive(false);
    }
}
