using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : Bullet
{

    public Vector2 SeekPos;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetMoveDir(Seek(Player.Instance.RB));
        Accelerate(Time.deltaTime);
    }



    private void OnEnable()
    {
        StartCoroutine(DestroySelf());
    }

}
