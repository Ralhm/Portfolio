using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knight : Enemy
{
    //only move in 4 cardinal directions
    //if hit a wall (raytrace) change movement to other direction

    
    public float AttackRange;
    public float ChangeDirectionRange;
    public float AttackForce;
    public bool MovingVertically;
    public bool IsAttacking;
    bool InAttack;


    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;

        ChooseDirection();
        traceOffset = GetComponent<Collider2D>().offset;

        collider = GetComponent<Collider2D>();
        //DisableCollider();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Debug.Log("Current Velocity : " + RB.velocity + "and test: " + test);



        if (!IsAttacking)
        {
            RaycastHit2D check = CheckForWall(MoveDir);
           // Debug.DrawRay(RB.position, MoveDir, Color.black);
            if (check.collider != null)
            {
                HitAWall(MoveDir);
            }

            if (GetDistanceToPlayer() < AttackRange)
            {
                Attack();
            }
            CheckDistanceToPlayer();

        }
        //Debug.Log(MoveDir);
        //Debug.Log(MoveDir);


    }

    public void ChooseDirection()
    {
        Vector2 vec = GetVectorToPlayer();

        if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y))
        {
            MoveHorizontally();
        }
        else if (Mathf.Abs(vec.y) > Mathf.Abs(vec.x))
        {
            MoveVertically();
        }
    }

    public void CheckDistanceToPlayer()
    {
        Vector2 vec = GetVectorToPlayer();
        if (MovingVertically)
        {

            if (vec.y > -ChangeDirectionRange && vec.y < ChangeDirectionRange)
            {
                MoveHorizontally();
            }
        }
        else
        {
            if (vec.x > -ChangeDirectionRange && vec.x < ChangeDirectionRange)
            {
                MoveVertically();
            }
        }
    }

    public override void HitAWall(Vector2 Dir)
    {
        base.HitAWall(Dir);
        //Debug.Log("HitAWall: " +  Dir);


        ChangeMovement(Dir);


    }

    public void ChangeDirection(Vector2 Dir)
    {
        //Don't change direction if there's a wall blocking the way.


        RaycastHit2D raycastHit = CheckForWall(Dir);
        if (raycastHit.collider == null)
        {
            MoveDir = Dir;
            RB.velocity = Dir * Speed;
            //Debug.Log("Changed Move Dir: " + MoveDir);
            //Debug.Log("Changed Move Speed: " + RB.velocity);

            if (Dir == Vector2.up || Dir == Vector2.down)
            {
                MovingVertically = true;
            }
            else
            {
                MovingVertically = false;
            }


        }
        else
        {
            //failsafe
            if (MoveDir == Vector2.zero)
            {
                MoveDir = Dir;
                RB.velocity = Dir * Speed;
            }

            //Debug.Log("HIT SOMETHING: " + raycastHit.collider.gameObject.name);
        }


    }

    public void ChangeMovement(Vector2 Dir)
    {
        //Debug.Log("Changing Movment!");
        if (Dir == Vector2.up || Dir == Vector2.down)
        {
            MoveHorizontally();
        }
        else if (Dir == Vector2.left || Dir == Vector2.right)
        {
            MoveVertically();
        }
    }

    public void MoveVertically()
    {
        Vector2 vec = GetVectorToPlayer();
        


        if (vec.y < 0)
        {
            //Debug.Log("Moving Up!");
            ChangeDirection(Vector2.up);
        }
        else
        {
            //Debug.Log("Moving down!");
            ChangeDirection(Vector2.down);
        }


    }

    public void MoveHorizontally()
    {
        Vector2 vec = GetVectorToPlayer();


        if (vec.x < 0)
        {
            //Debug.Log("Moving right!");
            ChangeDirection(Vector2.right);
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            //Debug.Log("Moving left!");
            ChangeDirection(Vector2.left);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

    }

    public override void Attack()
    {
        base.Attack();
        IsAttacking = true;
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        MoveDir = Vector2.zero;
        RB.velocity = Vector2.zero;

        Anim.SetTrigger("Ready");

        yield return new WaitForSeconds(0.5f);

        InAttack = true;
        RB.AddForce(Seek(player.RB) * AttackForce);
        Anim.SetTrigger("Attack");


        if (Seek(player.RB).x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }



        yield return new WaitForSeconds(0.5f);

        InAttack = false;
        RB.velocity = Vector2.zero;
        yield return new WaitForSeconds(1);

        
        ChooseDirection();
        Anim.SetTrigger("Walk");
        yield return new WaitForSeconds(1);
        IsAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(ContactDamage, InAttack);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(RB.position + traceOffset, traceRadius);


    }
#endif
}
