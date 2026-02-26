using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{
    [Header("Properties")]
    public int health = 5;
    public float Speed;
    public float MaxSpeed;
    public float Acceleration;
    public int ContactDamage = 1;
    public float MoneyDropChance;
    public float HealthDropChance;
    public float traceDistance;
    public float traceRadius;
    public Vector2 traceOffset;
    public Element Weakness;
    public Element Resistance;
    public LayerMask Mask;

    [Header("Wander Stuff")]
    public float offset;
    public float radius;
    public float rate;
    public bool WanderWithVelocity;

    protected Vector2 MoveDir;


    [Header("References")]
    public Room CurrentRoom;
    public Bullet bulletClass;
    public Rigidbody2D RB;
    public Animator Anim;

    protected BulletPool Pool;
    protected Player player;

    protected Collider2D collider;


    private void Start()
    {
        player = Player.Instance;
        RB = GetComponent<Rigidbody2D>();


    }

    private void FixedUpdate()
    {


        
    }

    public void SetVelocity(Vector2 Dir)
    {
        MoveDir = Dir;
        RB.velocity = Dir * Speed;
    }

    public void SetVelocity(Vector2 Dir, float Speed)
    {
        MoveDir = Dir;
        RB.velocity = Dir * Speed;
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

    public Vector2 Flee(Vector2 pos)
    {
        Vector2 dir = pos - RB.position;
        return -dir.normalized;
        
    }


    public Vector2 Flee(Rigidbody2D rb)
    {
        Vector2 dir = rb.position - RB.position;
        return -dir.normalized;
    }


   public float GetDistanceToPlayer()
   {
        return (transform.position - player.transform.position).magnitude;
   }

    public Vector2 GetVectorToPlayer()
    {
        return (RB.position - player.RB.position);
    }

    public void MoveInRandomDirection()
    {
        Vector2 Dir = FindClearPath();
        SetVelocity(Dir);
    }

    public Vector2 ReturnRandomDirection()
    {
        return new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
    }

    public void AccelerateInRandomDirection()
    {
        Vector2 Dir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        MoveDir = Dir;
    }

    public Vector2 Wander()
    {
        float range = 100;
        float binomial = Random.Range(-range, range) - Random.RandomRange(-range, range);
        Vector2 targetPos = new Vector2(0, 0);
        if (WanderWithVelocity)
        {
            targetPos = RB.position + RB.velocity.normalized * offset;
        }
        else
        {
            targetPos = RB.position + MoveDir * offset;
        }


        float point = rate * binomial * radius;
        Vector3 cross = Vector3.Cross(new Vector3(MoveDir.x, MoveDir.y, 0), Vector3.forward);
        Vector2 right = new Vector2(cross.x, cross.y);
        targetPos += point * right;

        Debug.DrawRay(targetPos, right);
        Debug.DrawLine(RB.position, targetPos);

        return Seek(targetPos);
    }

    public void Accelerate(float dt)
    {
        RB.velocity += Acceleration * dt * MoveDir;
        RB.velocity = Vector2.ClampMagnitude(RB.velocity, MaxSpeed);
    }


    public virtual void Attack()
    {

    }

    public void CheckAllSides()
    {
        RaycastHit2D DownHit = CheckForWall(Vector2.down);
        if (DownHit.collider != null)
        {
            HitAWall(Vector2.down);
        }


        RaycastHit2D UpHit = CheckForWall(Vector2.up);
        if (UpHit.collider != null)
        {
            HitAWall(Vector2.up);
        }

        RaycastHit2D LeftHit = CheckForWall(Vector2.left);
        if (LeftHit.collider != null)
        {
            HitAWall(Vector2.left);
        }

        RaycastHit2D RightHit = CheckForWall(Vector2.right);
        if (RightHit.collider != null)
        {
            HitAWall(Vector2.right);
        }
    }

    public RaycastHit2D CheckForWall(Vector2 Dir)
    {


        RaycastHit2D raycastHit = new RaycastHit2D();
        raycastHit = Physics2D.CircleCast(RB.position + traceOffset + Dir * 0.5f, traceRadius, Dir, traceDistance, Mask);
        Debug.DrawRay(RB.position + traceOffset + Dir * 0.5f, Dir * traceDistance, Color.green);


        return raycastHit;
       
    }

    public Vector2 FindClearPath()
    {
        int i = 0;
        RaycastHit2D Hit;
        Vector2 Dir = Vector2.zero;
        while (i < 30)
        {
            Dir = ReturnRandomDirection();
            Hit = CheckForWall(Dir);

            if (Hit.collider == null)
            {

                return Dir;
            }

            i++;
        }
        return Dir;
    }

    public virtual void HitAWall(Vector2 Dir)
    {

    }  

    public IEnumerator shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(originalPosition.x - magnitude, originalPosition.x + magnitude);
            float y = Random.Range(originalPosition.y - magnitude, originalPosition.y + magnitude);

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null; //Wait until next frame
        }

        transform.localPosition = originalPosition;
    }

    public virtual void Shoot(Vector2 Dir, float Angle = 0)
    {
        Bullet bullet = Instantiate(bulletClass, transform.position, transform.rotation);

    }

    public void ShootThree()
    {
        Vector2 Dir = Seek(player.RB);

        Shoot(Dir);
        Shoot(Dir, 45);
        Shoot(Dir, -45);
    }

    public void ShootPlusPattern()
    {
        Shoot(Vector2.up);
        Shoot(Vector2.down);
        Shoot(Vector2.right);
        Shoot(Vector2.left);
    }

    public void ShootXPattern()
    {
        Shoot(Vector2.up, 45);
        Shoot(Vector2.up, -45);
        Shoot(Vector2.down, 45);
        Shoot(Vector2.down, -45);
    }

    public void ShootPlusPattern(float Angle = 0)
    {
        Shoot(Vector2.up, Angle);
        Shoot(Vector2.down, Angle);
        Shoot(Vector2.right, Angle);
        Shoot(Vector2.left, Angle);
    }

    public void ShootXPattern(float Angle = 0)
    {
        Shoot(Vector2.up, 45 + Angle);
        Shoot(Vector2.up, -45 + Angle);
        Shoot(Vector2.down, 45 + Angle);
        Shoot(Vector2.down, -45 + Angle);
    }




    public void TakeDamage(int damage)
    {
        //Debug.Log("Getting Hit!");
        StartCoroutine(shake(0.1f, 0.15f));


        health -= damage;

        GameManager.instance.SpawnDamageNumber(transform, damage);
        if (health <= 0)
        {

            OnDeath();
        }
    }

    public void OnDeath()
    {
        if (gameObject.activeSelf)
        {
            Debug.Log(gameObject.name + "Dying!!!");
            if (CurrentRoom != null)
            {
                CurrentRoom.DecreaseEnemies();
                
            }
            DropHealth();
            DropMoney();
            gameObject.SetActive(false);
        }

    }

    public void DropMoney()
    {
        int rand = Random.Range(0, 100);

        Debug.Log("Attmepting to drop money:" + rand);
        if (rand <= MoneyDropChance)
        {
            GameManager.instance.SpawnMoney(transform);
        }
    }

    void DropHealth()
    {
        int rand = Random.Range(0, 100);

        Debug.Log("Attmepting to drop money:" + rand);
        if (rand <= HealthDropChance)
        {
            GameManager.instance.SpawnHealth(transform);
        }
    }

    public virtual void SetRoom(Room room)
    {
        CurrentRoom = room;
    }

    public void EnableCollider()
    {
        collider.enabled = true;
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(ContactDamage);
        }
        
    }




}
