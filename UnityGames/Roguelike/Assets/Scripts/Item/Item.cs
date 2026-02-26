using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public enum ShopType
{
    None, Normal, Magic
}

public class Item : MonoBehaviour
{
    public TextMeshProUGUI PriceText;
    public ItemDisplay Display;

    protected ItemData ItemProperty;
    public ItemContainer SpawnableItems; //I would like this to work but it's just not

    public ShopType Type;

    public float frequency = 1;
    public float Distance = 0.1f;
    public float MinDistance = 3;
    bool InRange = false;
    protected float startingY;

    protected virtual void Awake()
    {
        SetRandomItem();
        if (Type == ShopType.None)
        {
            //Debug.Log("Removing Price text!");
            PriceText.gameObject.SetActive(false);
        }

        HideItemText();

        startingY = transform.localPosition.y;
        
        
    }

    public void ActivatePriceTag(ShopType shopType)
    {
        Type = shopType;

        if (Type != ShopType.None) {
            PriceText.gameObject.SetActive(true);
            PriceText.text = ItemProperty.Price.ToString();
        }

    }

    protected virtual void FixedUpdate()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Sin(Time.time * frequency) * Distance + startingY, transform.position.z);
        DisplayDescription();
    }
    public virtual void SetRandomItem()
    {
        ItemProperty = SpawnableItems.GetRandomItem();
        GetComponent<SpriteRenderer>().sprite = ItemProperty.ItemSprite;

    }

    public void DisplayItemText(string name, string description)
    {
        if (InRange)
        {
            Display.gameObject.SetActive(true);
            Display.DisplayText(name, description);
        }

    }

    public void HideItemText()
    {
        Display.gameObject.SetActive(false);
    }

    public void DisplayDescription()
    {

        if (!InRange)
        {
            if ((Player.Instance.transform.position - transform.position).magnitude < MinDistance)
            {
                InRange = true;
                DisplayItemText(ItemProperty.ItemName, ItemProperty.ItemDescription);
            }
        }
        else if (InRange)
        {
            if ((Player.Instance.transform.position - transform.position).magnitude > MinDistance)
            {
                InRange = false;
                HideItemText();
            }
        }

    }

    public virtual void OnCollect(Player player)
    {
        if (Type == ShopType.Normal)
        {
            if (player.GetMoney() < ItemProperty.Price)
            {
                Debug.Log("Not enough money!");
                return;
            }
        }


        ItemProperty.Activate(player);
        HideItemText();

        UI.Instance.UpdateDisplayValues();
        player.AddItem(ItemProperty);
        Destroy(gameObject);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            OnCollect(collision.gameObject.GetComponent<Player>());
        }
    }
}
