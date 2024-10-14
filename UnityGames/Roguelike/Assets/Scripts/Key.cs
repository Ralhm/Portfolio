using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Transform FocusTarget;
    public float SmoothTime;

    Vector3 Offset;
    Vector3 TargetPos;

    Vector3 Velocity = Vector3.zero;

    bool IsFollowing;

    void Start()
    {
        FocusTarget = Player.Instance.gameObject.transform;
    }

    private void FixedUpdate()
    {
        if (IsFollowing)
        {
            FollowPlayer();
        }
    }

    public void FollowPlayer()
    {
        Vector3 target = FocusTarget.position + Offset;

        TargetPos = Vector3.SmoothDamp(transform.position, target, ref Velocity, SmoothTime);


        transform.position = TargetPos;

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Key Colliding:" + collision.gameObject.name);

        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Player>().SetHasKey(true);

            IsFollowing = true;


        }
        if (collision.gameObject.layer == 10)
        {
            Door door = collision.gameObject.GetComponent<Door>();

            if (door.isLocked)
            {
                

                if (door.CurrentRoom.ClearedRoom)
                {
                    Debug.Log("Unlocking door!");
                    door.UnlockRoom();
                    gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("Room has not been cleared!");
                }

            }
            else
            {
                Debug.Log("Door is Not Locked!");
            }

        }

    }



}
