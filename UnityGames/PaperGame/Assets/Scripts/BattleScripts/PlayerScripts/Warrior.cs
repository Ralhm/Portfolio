using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Player
{
    public Warrior() : base()
    {
        skills.Add(attack);
        skills[0].property = Skills.Property.Slash;
        skills.Add(new BroadSlash());
        skills.Add(new StrongThrust());
        skills.Add(new ShieldBash());
        skills.Add(new Taunt());


        resistance.Add(new Skills());
        resistance.Add(new Skills());
        resistance.Add(new Skills());
        resistance[0].property = Skills.Property.Slash;
        resistance[1].property = Skills.Property.Stab;
        resistance[2].property = Skills.Property.Bash;

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].skill = Skills.Skill.Learned;
        }


    }


}
