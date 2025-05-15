using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpellType
{
    Projectile, Special, Melee

};

public enum Element
{
    None, Ice, Electric, Fire, Shadow
}


[CreateAssetMenu(fileName = "SpellData.asset", menuName = "SpellData/Default Spell")]
public class Spell : ItemData
{

    public Player player;

    public Element property;
    public SpellType type;

    public bool Cancellable = true;

    public string SpellName;
    public float ManaCost;
    public int Damage;
    public float AOE;
    public float HeatAmount;
    public AudioClip SFX;
    public bool Active;


    public virtual void ActivateSpell()
    {
        if (player.GetMana() < ManaCost)
        {
            return;
        }
        //player.StartCasting(ManaCost * player.GetCostModifier(), CastDuration * player.GetCastModifier());  



        if (type == SpellType.Melee || type == SpellType.Special)
        {
            if (player.CurrentSpell != null)
            {
                player.CancelSpell();
            }
            
        }
        
        player.CastSpell(this);
        Active = true;

        Cast();
    }
    public virtual void Cast()
    {
        
        player.LoseMana(ManaCost);
        player.IncreaseHeat(HeatAmount);
    }

    public void SetOwner(Player player)
    {
        this.player = player;

    }

    public void SetOwner()
    {
        player = Player.Instance;
    }

    //for when the player acquires this spell through purchase or changing spells
    public void OnPickup()
    {
        SetOwner();
        SetPlayerSpell();

    }

    public void SetPlayerSpell()
    {
        switch (type)
        {
            case SpellType.Projectile:
                player.SetProjectile(this);
                break;

            case SpellType.Special:
                player.SetSpecial(this);
                break;

            case SpellType.Melee:
                player.SetMelee(this);
                break;
        }



        
    }

    public void CheckAOE(Vector2 origin)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, AOE, new Vector2(0, 0));
        //Debug.Log(hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {

            if (hits[i].transform.gameObject.layer == 7)
            {
                Enemy enemy = hits[i].transform.gameObject.GetComponent<Enemy>();

                ApplyDamage(enemy);
            }
        }
    }



    public void ApplyDamage(Enemy enemy)
    {
        float Bonus = (Damage * player.GetHeatBonus());

        Debug.Log("Damage: " + Damage);
        Debug.Log("Heat Bonus: " + Bonus);

        float FinalDamage = ((Damage + Bonus) * player.GetDamageModifier());

        Debug.Log("Damage after modifier: " + FinalDamage);

        switch (property)
        {
            case Element.Fire:
                FinalDamage *= player.GetFireModifier(); 
                break;
            case Element.Ice:
                FinalDamage *= player.GetIceModifier();
                break;
            case Element.Electric:
                FinalDamage *= player.GetElectricModifier();
                break;
        }

        Debug.Log("Damage after element modifier: " + FinalDamage);

        switch (type)
        {
            case SpellType.Projectile:
                FinalDamage *= player.GetProjectileModifier(); 
                break;

            case SpellType.Melee:
                FinalDamage *= player.GetMeleeModifier();
                break;
            case SpellType.Special:
                FinalDamage *= player.GetSpecialModifier();
                break;
        }


        Debug.Log("Damage after type modifier: " + FinalDamage);

        if (enemy.Resistance == property)
        {
            FinalDamage *= (0.75f);
        }

        if (enemy.Weakness == property)
        {
            FinalDamage *= (1.25f);
        }


        Debug.Log("Final Damage Bonus: " + FinalDamage);

        enemy.TakeDamage((int)FinalDamage);




    }


    public virtual void CancelSpell()
    {
        Active = false;
    }

    public virtual void StopSpell()
    {

    }

    //only set current spell to null when ending the spell naturally
    public virtual void FinishSpell()
    {
        player.CurrentSpell = null;
        player.Anim.SetTrigger("EnterMovement");
        player.SetState(Player.State.Normal);

    }



}
