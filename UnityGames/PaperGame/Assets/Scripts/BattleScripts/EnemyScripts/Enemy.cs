using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    public Skills basicAttack = new Skills(); //An enemy skill that they can call on. You can make more of these
    public BattleUI ui;
    public Character targetList;
    public bool hasActed = false;
    public bool attacked = false;

    void Awake()
    {
        animator.SetBool("Idle", true);
        
    }

    public int SelectTarget(List<Player> players)
    {
        int targetSum = 0;
        for (int i = 0; i < players.Count; i++) //Generate character attack odds
        {
            players[i].drawRateMin = targetSum;
            targetSum += players[i].drawRate;
            players[i].drawRateMax = targetSum - 1;
        }
        int target = Random.Range(0, targetSum);

        for (int j = 0; j < players.Count; j++)
        {
            if ((target > players[j].drawRateMin) && (target < players[j].drawRateMax))
            {
                return j; //Return the index of the target
            }

        }
        return 0;
    }

    //Deprecate
    public IEnumerator Die()
    {
        
        print("enemy " + index + " dying! COROUTINE");
        BattleUI.UIsingleton.aliveEnemies -= 1;

        status = Status.Dead;
        ClearEffects();
        animator.SetTrigger("Death");
        statEffectDisplay.gameObject.SetActive(false);

        //BattleUI.UIsingleton.enemies.RemoveAt

        yield return new WaitForSeconds(1.5F);
        /*
        for (int i = 0; i < BattleUI.UIsingleton.enemies.Count; i++)
        {
            BattleUI.UIsingleton.enemySelection[i].GetComponent<Buttons>().index = i;
            BattleUI.UIsingleton.enemies[i].index = i;
        }
        */
        print("Enemy " + index + " dying! After Waiting!");
        BattleUI.UIsingleton.AudioJungle.Play("EnemyDeath1");     
        Destroy(this.gameObject);
        //BattleUI.UIsingleton.enemies.RemoveAt(index);

    }

    public void Death()
    {
        print("enemy " + index + " dying! START");
        BattleUI.UIsingleton.aliveEnemies -= 1;

        status = Status.Dead;
        ClearEffects();
        animator.SetTrigger("Death");



    }

    public void DeathEvent()
    {
        statEffectDisplay.gameObject.SetActive(false);
        print("Enemy " + index + " dying EVENT");
        BattleUI.UIsingleton.AudioJungle.Play("EnemyDeath1");
        this.gameObject.SetActive(false);
    }

    public override void Poison() //Catch all function for Status Effects, Call this upon new turns, only call if Status is not normal
    {
        BattleUI.UIsingleton.AudioJungle.Play("Damage");
        health -= 3;
        particles.Emit(3);
        BattleUI.UIsingleton.DamageNumbers.SpawnParticleNumber(transform.position, 3, Color.black, true, 3 * 0.05F);
        StartCoroutine(shake(3 * 0.1f, 3 * 0.15f));
        if (health <= 0)
        {
            Death();
        }

    }

    public override void Confusion(Character Victim)
    {
        
    }

    public bool Paralysis()
    {
        if (Random.Range(0, 100) < 50)
        {
            print("Enemy is Paralyzed and can't move");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool StatusCheck()
    {
        if (status == Character.Status.Stunned || status == Character.Status.Sleep)
        {        
            return true;

        }
        else if (status == Character.Status.Paralyzed)
        {
            if (Paralysis())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public IEnumerator StandardAttack(Skills skill, List<Player> Victim, Enemy User)
    {
        if (StatusCheck())
        {
            print("Enemy is Immobile");
            BattleUI.UIsingleton.DamageNumbers.SpawnParticle(transform.position + new Vector3(0, 2F, 0), "Immobile!", Color.red);
            yield break;
        }

        int target = SelectTarget(Victim);
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1F);
        if (status == Character.Status.Confused)
        {
            if (Random.Range(0, 100) < 90)
            {
                print("Enemy is Confused!");




                int selection = Random.Range(0, BattleUI.UIsingleton.enemies.Count);//Randomly select an enemy to attack
                BattleUI.UIsingleton.enemies[selection].TakeDamage(User, 0, skill.property, skill.subProperty);

                /*
                if (!BattleUI.UIsingleton.enemies[selection].gameObject.activeSelf) //Make sure the enemy is active
                {
                    for (int i = 0; i < BattleUI.UIsingleton.enemies.Count; i++)
                    {
                        selection += 1;
                        if (selection >= BattleUI.UIsingleton.enemies.Count)
                        {
                            selection = 0;
                        }

                        if (BattleUI.UIsingleton.enemies[selection].gameObject.activeSelf)
                        {
                            BattleUI.UIsingleton.enemies[selection].TakeDamage(User, 0, skill.property, skill.subProperty);
                        }
                    }
 
                }
                else
                {
                    BattleUI.UIsingleton.enemies[selection].TakeDamage(User, 0, skill.property, skill.subProperty);
                }
                */
                
                yield break;              
            }
        }

        Victim[target].TakeDamage(User, 0, skill.property, skill.subProperty);
        animator.SetTrigger("Idle");


    }

    public Enemy() //Default Constructor
    {

    }

    public virtual void Attack(List<Player> Victim) //Make this a generic function that calls on random coroutines, each one corresponding to a possible skill the enemy can use
    {
        animator.SetTrigger("Attack");
        print("Default Attacking...");
    }
    
    public override void TakeDamage(Character User, int bonus, Skills.Property property, Skills.Property subProperty)
    {
        //print("Enemy taking damage");
        base.TakeDamage(User, bonus, property, subProperty);
        if (health <= 0)
        {
            animator.enabled = true;
            Death();
        }
    }



}
