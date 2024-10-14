using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Base Class that players and enemies inherit from
public class Character : MonoBehaviour
{
    
    public enum Status
    {
        Normal, Poisoned, Stunned, Dead, Confused, Sleep, Cursed, Paralyzed
    }


    public Status status;

    public bool isDead;
    public int maxSP;
    public int SP;
    public bool immobile = false;

    public int maxHealth;
    public int health;
    public int attackPower;
    public int magic;
    public int defense;
    public int magDefense;
    public int duration = 2; //For Status effects 
    public int index;

    [Header("Animation, Particles, And Status")]
    public Animator animator;
    public ParticleSystem particles;

    public StatEffectDisplay statEffectDisplay;

    //Try Looping through the list of deubffs/buffs, and if they already have said debuff by checking the skillname, add to the duration rather than re-apply it
    public List<Skills> debuffs = new List<Skills>();
    public List<Skills> buffs = new List<Skills>();

    //List of weaknesses and resistances, if the defender has one of these that matches the attackers skill, calculate accordingly
    public List<Skills> weakness = new List<Skills>();
    public List<Skills> resistance = new List<Skills>();



    public void ClearEffects()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            statEffectDisplay.SetDefaultImage(statEffectDisplay.buffs[i]);
        }
        for (int i = 0; i < debuffs.Count; i++)
        {
            statEffectDisplay.SetDefaultImage(statEffectDisplay.debuffs[i]);
        }
        buffs.Clear();
        debuffs.Clear();
        
    }

    public virtual void Poison() //Catch all function for Status Effects, Call this upon new turns, only call if Status is not normal
    {
        BattleUI.UIsingleton.AudioJungle.Play("Damage");
        health -= 3;
        BattleUI.UIsingleton.DamageNumbers.SpawnParticleNumber(transform.position, 3, Color.black, true, 3 * 0.05F);
        particles.Emit(3);
        StartCoroutine(shake(3 * 0.1f, 3 * 0.15f));

    }

    public virtual void Confusion(Character Victim)
    {

    }

    public void RestoreSP(int value)
    {
        if(status != Status.Dead)
        {
            SP += value;
            if (SP > maxSP)
            {
                SP = maxSP;
            }
        }

    }
    public void LoseSP(int value)
    {
        SP -= value;
    }



    public virtual void RestoreHealth(int value)
    {
        if (status != Status.Dead)
        {
            health += value;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
        BattleUI.UIsingleton.DamageNumbers.SpawnParticleNumber(transform.position, value, Color.green, false);

    }

    public IEnumerator shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(originalPosition.x - magnitude, originalPosition.x + magnitude);
            float y = Random.Range(originalPosition.y  - magnitude, originalPosition.y + magnitude);

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null; //Wait until next frame
        }

        transform.localPosition = originalPosition;
    }

    public void Affliction(Skills.Property subProperty, int success, int Duration)
    {
        int chance = Random.Range(0, 100);
        print("Chance = " + chance);
        if (chance < success) //Check if the status effect is successful
        {
            print("Success!");
            if (subProperty == Skills.Property.Poison) //Check for Status Effects
            {
                status = Status.Poisoned;
                BattleUI.UIsingleton.DamageNumbers.SpawnParticle(transform.position + new Vector3(0, 2F, 0), "Poisoned!", Color.green);
                animator.enabled = true; //These true statements are here because a character can only have 1 stat effect at a time. 
                //If they get a new stat effect while they're stunned or asleep, this will re-enable their animator. KInda spaghetti but it'll have to do for now.
            }

            if (subProperty == Skills.Property.Stun)
            {
                status = Status.Stunned;
                BattleUI.UIsingleton.DamageNumbers.SpawnParticle(transform.position + new Vector3(0, 2F, 0), "Stunned!", Color.gray);
                animator.enabled = false;
            }
            if (subProperty == Skills.Property.Sleep)
            {
                status = Status.Sleep;
                BattleUI.UIsingleton.DamageNumbers.SpawnParticle(transform.position + new Vector3(0, 2F, 0), "Asleep!", Color.blue);
                animator.enabled = false;
            }
            if (subProperty == Skills.Property.Paralyze)
            {
                status = Status.Paralyzed;
                BattleUI.UIsingleton.DamageNumbers.SpawnParticle(transform.position + new Vector3(0, 2F, 0), "Paralyzed!", Color.yellow);
                animator.enabled = true;
            }
            if (subProperty == Skills.Property.Confuse)
            {
                status = Status.Confused;
                BattleUI.UIsingleton.DamageNumbers.SpawnParticle(transform.position + new Vector3(0, 2F, 0), "Confused!", Color.magenta);
                animator.enabled = true;
            }
            duration = Duration;
        }

    }

    public void AddBuff(Skills buff)
    {
        for (int i = 0; i < buffs.Count; i++) //Check if the buff is already applied
        {
            if (buffs[i] == buff)
            {
                buffs[i].duration += buff.duration;
                return;
            }
        }
        if (buffs.Count != 3) //Add Buff if it is not already applied
        {
            buffs.Add(buff);
        }
        else //If max number of buffs is already applied, replace the bottom buff
        {
            print("Replacing bottom buff");
            buffs.Add(buff);
            buffs.RemoveAt(0);
        }
    }

    public void AddDebuff(Skills debuff)
    {
        for (int i = 0; i < debuffs.Count; i++) //Check if the buff is already applied
        {
            if (debuffs[i] == debuff)
            {
                debuffs[i].duration += debuff.duration;
                return;
            }
        }
        if (debuffs.Count != 3) //Add Buff if it is not already applied
        {
            debuffs.Add(debuff);
        }
        else //If max number of buffs is already applied, replace the bottom buff
        {
            print("Replacing bottom debuff");
            debuffs.Add(debuff);
            debuffs.RemoveAt(0);
        }
    }


    public virtual void TakeDamage(Character User, int bonus, Skills.Property property, Skills.Property subProperty)
    {
        animator.SetTrigger("Damage");
        print("Character Taking damage");
        int totalDamage = 0;
        if (bonus >= 0)
        {
            if (property != Skills.Property.None) //If a skill has no main property then it does no damage
            {
                
                if (status == Status.Sleep)
                {
                    status = Status.Normal;
                    statEffectDisplay.SetDefaultImage(statEffectDisplay.statusEffect);
                    animator.enabled = true;
                    totalDamage += 2;
                }
                if (property == Skills.Property.Slash || property == Skills.Property.Stab || property == Skills.Property.Bash)
                {
                    totalDamage += User.attackPower - defense + bonus;
                }
                else if (property == Skills.Property.Fire || property == Skills.Property.Ice || property == Skills.Property.Electric)
                {
                    totalDamage += User.magic - defense + bonus;
                }
                
                for (int i = 0; i < debuffs.Count; i++)
                {
                    if (debuffs[i].buffType == Skills.BuffType.Defense) //If the defender has a defense DEbuff
                    {
                        totalDamage += debuffs[i].effectAmount;
                    }
                }
                for (int i = 0; i < User.debuffs.Count; i++)
                {
                    if (User.debuffs[i].buffType == Skills.BuffType.Attack) //If the Attacker has an Attack DEbuff
                    {
                        totalDamage -= User.debuffs[i].effectAmount;
                    }
                }

                for (int i = 0; i < buffs.Count; i++)
                {
                    if (buffs[i].buffType == Skills.BuffType.Defense) //If the defender has a defense BUFF
                    {
                        totalDamage -= buffs[i].effectAmount;
                    }
                }

                for (int i = 0; i < User.buffs.Count; i++)
                {
                    if (User.buffs[i].buffType == Skills.BuffType.Attack) //If the Attacker has an attack BUFF
                    {
                        totalDamage += User.buffs[i].effectAmount;
                    }
                }

                for (int i = 0; i < weakness.Count; i++) //Account for Weaknesses and Resistances
                {
                    if (weakness[i].property == property)
                    {
                        BattleUI.UIsingleton.DamageNumbers.SpawnParticle(transform.position + new Vector3(0, 2F, 0), "Weak!", new Color(0.5f, 0.5f, 1f));
                        print("Hit a Weakness!");
                        totalDamage += 2;
                    }
                }
                for (int i = 0; i < resistance.Count; i++)
                {
                    if (resistance[i].property == property)
                    {
                        BattleUI.UIsingleton.DamageNumbers.SpawnParticle(transform.position + new Vector3(0, 2F, 0), "Resist!", Color.white);
                        print("Hit a Resistance!");
                        totalDamage -= 2;
                    }
                }
            }



            StartCoroutine(shake(totalDamage * 0.1f, totalDamage * 0.15f));
            if (totalDamage < 0)
            {
                totalDamage = 0;
            }
            health -= totalDamage;

            BattleUI.UIsingleton.DamageNumbers.SpawnParticleNumber(transform.position, totalDamage, Color.black, true, totalDamage*0.05F);

        }
        else
        {
            totalDamage = 0;
            health -= totalDamage;
        }
        particles.Emit(totalDamage);
    }



}
