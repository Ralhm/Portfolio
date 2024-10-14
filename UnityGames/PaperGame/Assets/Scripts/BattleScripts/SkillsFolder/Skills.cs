using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable] //By Making this Serializable, You can see all these in the unity inspector
public class Skills
{
    public int cost = 2;
    public int bonus = 0;
    public string SkillName;
    public string SkillDescription;
    public float timing = 1;
    public string audioName;
    public string type; //For Animation Calls
    public bool singleTarget = true;
    public bool statusEffect = false;

    public int duration = 2; //For status effect skills
    public int baseDuration = 2;
    public bool effectActive = false; //For Debuff/Buff related skills
    public int effectAmount = 0;
    public int successRate = 0; //Odds of status effect Landing
    public Sprite icon;

    public string MashTutorial = "Left Click to fill the bar above the threshold, but don't fill it to the max.";
    public string TimingTutorial = "Left Click when the countdown says 'GO'.";
    public string AimTutorial = "Left Click when the reticle is above the enemy.";
    public string SequenceTutorial = "Hit the 4 keys as they appear.";

    public enum BuffType
    {
        Attack, Defense
    }

    public BuffType buffType;

    public enum ActionType
    {
        Sequence, Timing, Aim, Mash, None
    }

    public ActionType action;

    public enum Skill
    {
        NotLearned, Learned
    }
    public enum Property
    {
        None, Slash, Stab, Bash, Ice, Fire, Electric, Poison, Stun, Confuse, Curse, Blind, Sleep, Paralyze, Party
    }

    public Property property;
    public Property subProperty;

    public Skill skill;

    public Skills() //Constructor
    {
        type = "Standard";
        SkillName = "Undefined SKill!!!!";
        SkillDescription = "Error skill";
        skill = Skill.NotLearned;
        property = Property.None;
        subProperty = Property.None;
    }

    public void AttackAll(List<Enemy> Targets, Character User)
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i].gameObject.activeSelf)
            {
                if (statusEffect)
                {
                    Targets[i].Affliction(subProperty, successRate, duration);
                }
                Targets[i].TakeDamage(User, bonus, property, subProperty);
            }

        }

        /*DEPRECATED METHOD FOR IF YOU WANT TO DESTROY THE ENEMY UPON DEATH, CAUSES MORE TROUBLE THAN IT'S WORTH
        int attackedTargets = Targets.Count;

        for (int i = 0; i <= Targets.Count; i++)
        {
            if (i == Targets.Count)
            {
                if (attackedTargets != 0)
                {
                    i = 0;
                }
                else
                {
                    break;
                }
            }
            if (!Targets[i].attacked)
            {
                attackedTargets -= 1;
                Targets[i].attacked = true;
                Debug.Log("Attacking Target: " + i);
                Targets[i].TakeDamage(User, bonus, property, subProperty);
                if (statusEffect)
                {
                    Targets[i].Affliction(subProperty, successRate, duration);
                }
            }

        }
        for (int i = 0; i < Targets.Count; i++) //Reset the attacked bool afterwards
        {
            Targets[i].attacked = false;
        }
        */

    }


    public virtual void Execute(Character Target, Character User, bool Hit)
    {
        Debug.Log("Using Generic Skill");
    }

    public virtual void Execute(Player Target, Character User, bool Hit)
    {
        Debug.Log("Using Generic Skill on Party Member");
    }

    public virtual void Execute(List<Enemy> Target, Character User, bool Hit)
    {
        Debug.Log("Using Generic Skill on All enemies");
    }

    public virtual bool isEffected(Character Target, Skills skill)
    {
        Debug.Log(skill.SkillName);
        if (Target.debuffs != null)
        {
            for (int i = 0; i < Target.debuffs.Count; i++)
            {
                if (Target.debuffs[i].SkillName == skill.SkillName)
                {
                    Target.debuffs[i].duration += duration;
                    Debug.Log("Adding to the duration of the targets Debuff");
                    return true;
                }
            }
        }

        return false;
    }


}
