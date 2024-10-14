using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBullet : Bullet
{

    Projectile spellData;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void SetSpellData(Projectile spell)
    {
        spellData = spell;
    }

    private void OnEnable()
    {
        StartCoroutine(DestroySelf());
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            spellData.ApplyDamage(collision.gameObject.GetComponent<Enemy>());

            if (spellData.DestroyOnContact)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
