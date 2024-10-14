using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public string name;
    public int level;

    public RectTransform RT;
    public List<Skills> skills;
    public Skills queuedSkill;
    public bool hasActed = false;
    public AudioManager AudioJungle;
    public int baseDrawRate = 10;
    public int drawRate;
    public int drawRateMin = 0;
    public int drawRateMax = 0;

    public StandardAttack attack = new StandardAttack();

    public void Start()
    {
        drawRate = baseDrawRate;
        AudioJungle = BattleUI.UIsingleton.AudioJungle;
        UpdateAnimationStatus();
        //print("Printing Party Size from UI" + BattleUI.UIsingleton.party.Count);
    }



    public void Dead()
    {
        animator.ResetTrigger("Damage");

        drawRate = 0;
        status = Status.Dead;
        ClearEffects();
        if (health < 0)
        {
            health = 0;
        }

    }

    public void Wounded()
    {
        animator.SetBool("Dead", false);
        animator.SetBool("Wounded", true);
        animator.SetBool("Idle", false);
    }

    public void Revive(int value)
    {
        if (status == Status.Dead)
        {
            drawRate = baseDrawRate;
            status = Status.Normal;
            RestoreHealth(value);
            UpdateAnimationStatus();
        }

    }

    public Player()
    {
        skills = new List<Skills>();
    }

    public override void TakeDamage(Character User, int bonus, Skills.Property property, Skills.Property subProperty)
    {
        base.TakeDamage(User, bonus, property, subProperty);
        UpdateAnimationStatus();
        BattleUI.UIsingleton.UpdateDisplay();
        AudioJungle.Play("Damage");
    }

    public void UpdateAnimationStatus()
    {
        if (health <= 0)
        {
            Dead();
            animator.SetBool("Dead", true);
            animator.SetBool("Idle", false);
            animator.SetBool("Wounded", false);
        }
        else if (health <= 5 && health > 0)
        {
            animator.SetBool("Dead", false);
            animator.SetBool("Wounded", true);
            animator.SetBool("Idle", false);
        }
        else if (health > 5)
        {
            animator.SetBool("Dead", false);
            animator.SetBool("Wounded", false);
            animator.SetBool("Idle", true);
        }
    }

    public override void RestoreHealth(int value)
    {
        base.RestoreHealth(value);
        UpdateAnimationStatus();
    }

}
