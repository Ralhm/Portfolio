using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : Enemy
{

    public float MoveTime;
    public float WaitTime;


    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
        RB = GetComponent<Rigidbody2D>();
        StartCoroutine(MoveRandomly());
    }




    public IEnumerator MoveRandomly()
    {

        Vector2 Dir = Vector2.zero;
        while (true)
        {
            Dir = FindClearPath();
            SetVelocity(Dir);

            yield return new WaitForSeconds(MoveTime);

            RB.velocity = Vector2.zero;

            yield return new WaitForSeconds(WaitTime);

        }
    }

}
