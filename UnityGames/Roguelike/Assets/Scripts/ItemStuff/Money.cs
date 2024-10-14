using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Collectable
{

    public List<int> Values;
    public List<Sprite> Sprites;
    public List<int> Probability;



    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SelectPriceRandomly();
        RandomForce();
    }

    private void FixedUpdate()
    {
        Attraction();
    }



    public void SelectPriceRandomly()
    {
        int rand = Random.Range(0, 100);
        //Debug.Log("Random money int = " + rand);
        //Debug.Log("Name: " + gameObject.name);
        if (rand < Probability[0])
        {
            SetMoneyType(0);
        }
        else if (rand < Probability[1] && rand > Probability[0])
        {
            SetMoneyType(1);
        }
        else
        {
            SetMoneyType(2);
        }
    }

    public void SetMoneyType(int num)
    {
        Value = Values[num];
        GetComponent<SpriteRenderer>().sprite = Sprites[num];
    }

    public void SetPrice1()
    {
        SetMoneyType(0);
    }

    public void SetPrice5()
    {
        SetMoneyType(1);
    }

    public void SetPrice10()
    {
        SetMoneyType(2);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Player>().IncreaseMoney(Value);
            gameObject.SetActive(false);
        }
    }
}
