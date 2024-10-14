using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : Player
{
    public Ranger() : base()
    {
        skills.Add(attack);
        skills[0].property = Skills.Property.Slash;
        skills.Add(new DaggerFlurry());
        skills.Add(new ArrowShot());
        skills.Add(new PoisonArrow());

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].skill = Skills.Skill.Learned;
        }


    }
}
