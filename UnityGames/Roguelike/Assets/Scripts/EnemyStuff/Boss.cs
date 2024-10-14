using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackState
{
    Spinning, Moving, Spawning
}

public class Boss : Enemy
{

    [Header("Spawning")]
    public float SpawnDistance;
    public Enemy EnemyRef;

    [Header("Spinning")]
    public float SpinRate; //How quickly does it spin
    public float SpinShootRate; //How frequently do the bullets fire?

    [Header("Phase Timer")]
    public float InBetweenTime;
    public float SpinTime; //How long does the spinnign phase last
    public float SpawnTime; 
    public float MoveTime;

    [Header("Moving")]
    public float ShootRate;
    public float MoveRate;



    AttackState CurrentState;

    Vector3 RoomCenter;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
        RB = GetComponent<Rigidbody2D>();
        RoomCenter = CurrentRoom.GetRoomCenter();
        Pool = GameObject.Find("BulletPool").GetComponent<BulletPool>();
        StartCoroutine(ChooseNextAttack());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ChooseNextAttack()
    {
        AttackState newState = (AttackState)(-1);
        int i = 0;
        while (i < 20)
        {
            
            newState = (AttackState)UnityEngine.Random.Range(0, Enum.GetNames(typeof(AttackState)).Length);

            if (newState != CurrentState)
            {
                break;
            }
            i++;
            
        }

        Debug.Log("New State == " + newState);
        Debug.Log("Enum Lenght == " + Enum.GetNames(typeof(AttackState)).Length);

        yield return new WaitForSeconds(InBetweenTime);


        if (newState == AttackState.Spawning)
        {
            StartCoroutine(SpawnEnemies());
        }
        else if (newState == AttackState.Moving)
        {
            StartCoroutine(MoveRandomly());
            StartCoroutine(ShootCycle());
        }
        else
        {
            StartCoroutine(MoveToCenter());
        }
    }

    IEnumerator SpawnEnemies()
    {
        Debug.Log("Spawning!");
        SetVelocity(Vector2.zero);
        CurrentState = AttackState.Spawning;
        Instantiate(EnemyRef, RB.position + Vector2.right * SpawnDistance, transform.rotation);
        Instantiate(EnemyRef, RB.position + Vector2.left * SpawnDistance, transform.rotation);
        Instantiate(EnemyRef, RB.position + Vector2.down * SpawnDistance, transform.rotation);

        yield return new WaitForSeconds(SpawnTime);

        StartCoroutine(ChooseNextAttack());

    }

    public override void Attack()
    {
        base.Attack();


    }


    public override void SetRoom(Room room)
    {
        base.SetRoom(room);

    }

    IEnumerator MoveRandomly()
    {
        Debug.Log("Moving!");
        CurrentState = AttackState.Moving;
        float Timer = 0;
        while (Timer < MoveTime)
        {
            yield return new WaitForSeconds(MoveRate);
            Timer += MoveRate;
            MoveInRandomDirection();
        }

        StartCoroutine(ChooseNextAttack());
    }

    IEnumerator ShootCycle()
    {
        while (CurrentState == AttackState.Moving)
        {
            yield return new WaitForSeconds(ShootRate - 0.3f);
            Anim.SetTrigger("Attack");
            ShootThree();
            yield return new WaitForSeconds(0.3f);
            Anim.SetTrigger("Idle");
        }
    }

    IEnumerator MoveToCenter()
    {
        Debug.Log("Moving to Center!");
        CurrentState = AttackState.Spinning;

        float distToCenter = Vector3.Distance(transform.position, RoomCenter);

        SetVelocity(Seek(RoomCenter));

        while (distToCenter > 1.0f)
        {
            distToCenter = Vector3.Distance(transform.position, RoomCenter);
            yield return new WaitForSeconds(0.1f);
        }
        SetVelocity(Vector2.zero);
        StartCoroutine(SpinShot());
    }

    IEnumerator SpinShot()
    {
        Anim.SetTrigger("Attack");
        float angle = 0;
        float Timer = 0;
        Debug.Log("Spinning!");
        while (Timer < SpinTime)
        {

            yield return new WaitForSeconds(SpinShootRate);
            angle += SpinRate;
            Timer += SpinShootRate;

            ShootXPattern(angle);

        }
        Anim.SetTrigger("Idle");
        StartCoroutine(ChooseNextAttack());

    }


    public override void Shoot(Vector2 Dir, float Angle = 0)
    {

        Bullet bullet = Pool.GetObjectInPool(bulletClass, transform);
        //bullet.SetMoveDir(Dir, Angle);
        bullet.SetVelocity(Dir, Angle);
    }


}
