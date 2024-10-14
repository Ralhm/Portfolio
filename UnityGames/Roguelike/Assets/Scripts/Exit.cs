using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{

    void Start()
    {

        GetComponentInParent<BossRoom>().AddExit(this);
        gameObject.SetActive(false);
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            GameManager.instance.SavePlayer(collision.gameObject.GetComponent<Player>().GetPlayerData());
            GameManager.instance.LoadNextLevel();


        }
    }

 

}
