using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{

    public float FleeDistance;
    public float ShootRate;
    public float WallForce;



    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
        RB = GetComponent<Rigidbody2D>();

        
        Pool = GameObject.Find("BulletPool").GetComponent<BulletPool>();
        StartCoroutine(ShootCycle());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
 
        if (GetDistanceToPlayer() < FleeDistance)
        {
            MoveDir = Flee(player.RB);
            Accelerate(Time.deltaTime);
        }
        else
        {
            MoveDir = Vector2.zero;
        }
        CheckAllSides();
    }

    public override void Shoot(Vector2 Dir, float Angle = 0)
    {
        Bullet bullet = Pool.GetObjectInPool(bulletClass, transform);
        //bullet.SetMoveDir(Dir, Angle);
        bullet.SetVelocity(Dir, Angle);
    }

    public override void Attack()
    {
        base.Attack();
        //ShootXPattern();
        //ShootPlusPattern();
        Shoot(Seek(player.RB));
    }

    public override void HitAWall(Vector2 Dir)
    {
        base.HitAWall(Dir);
        //Debug.Log("HitAWall!");
        RB.AddForce(-Dir * WallForce);
    }



    IEnumerator ShootCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(ShootRate);
            Attack();
        }
    }

}
