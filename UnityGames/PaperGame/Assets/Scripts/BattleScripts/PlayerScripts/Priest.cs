using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Player
{
    public Priest() : base()
    {
        skills.Add(attack);
        skills[0].property = Skills.Property.Bash;
        skills.Add(new AttackDebuff());
        skills.Add(new AttackBuff());
        skills.Add(new DefenseDebuff());
        skills.Add(new DefenseBuff());
        skills.Add(new Heal());
        skills.Add(new Resurrect());

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].skill = Skills.Skill.Learned;
        }


    }
}
