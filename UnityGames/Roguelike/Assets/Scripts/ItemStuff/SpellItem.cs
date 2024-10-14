using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpellItem : Item
{

    public Spell spell;


    protected override void Awake()
    {
        SetRandomItem();
        if (Type != ShopType.None)
        {
            PriceText.text = ItemProperty.Price.ToString();
        }
        else
        {
            PriceText.gameObject.SetActive(false);
        }

        startingY = transform.localPosition.y;
        HideItemText();
    }

    public override void SetRandomItem()
    {
        base.SetRandomItem();

        spell = (Spell)ItemProperty;

    }


    public override void OnCollect(Player player)
    {
        if (Type != ShopType.None)
        {
            if (player.GetCrystals() < ItemProperty.Price)
            {
                Debug.Log("Not enough crystals!");
                return;
            }
        }


        if (spell.type == SpellType.Melee)
        {
            player.SetMelee(spell);
        }
        else if (spell.type == SpellType.Projectile)
        {
            player.SetProjectile(spell);
        }
        else if (spell.type == SpellType.Special)
        {
            player.SetSpecial(spell);
        }
        gameObject.SetActive(false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            OnCollect(collision.gameObject.GetComponent<Player>());
        }
    }
}
