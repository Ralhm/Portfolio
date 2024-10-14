using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//spawn an item or money
public class TreasureChest : MonoBehaviour
{
    public List<Item> items;
    public Money money;
    public int ItemChance;

    public int MoneyChanceLow;
    public int MoneyChanceHigh;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            int rand = Random.Range(0, 100);

            if (rand < ItemChance)
            {
                SpawnItem();
            }
            else
            {
                SpawnMoney();
            }
            gameObject.SetActive(false);


        }
    }

    public void SpawnItem()
    {
        int index = 0;
        if (items.Count > 1)
        {
            index = Random.Range(0, items.Count);
        }


        if (items != null)
        {

            Item newItem = Instantiate(items[index], transform.position, transform.rotation);

        }
        else
        {
            Debug.Log("NDFNFKLBFKLBFKBDK");
        }



    }

    public void SpawnMoney()
    {
        int rand = Random.Range(MoneyChanceLow, MoneyChanceHigh);
        for (int i = 0; i < rand; i++) {
            GameManager.instance.SpawnMoney(transform);

        }
    }
}
