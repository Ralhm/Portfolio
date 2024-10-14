using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Player
{

    public Mage() : base()
    {
        skills.Add(attack);
        skills[0].property = Skills.Property.Bash;
        skills.Add(new Thunder());
        skills.Add(new Fireball());
        skills.Add(new IceShatter());
        skills.Add(new Lullaby());

        weakness.Add(new Skills());
        weakness[0].property = Skills.Property.Bash;
        resistance.Add(new Skills());
        resistance[0].property = Skills.Property.Fire;
        resistance.Add(new Skills());
        resistance[1].property = Skills.Property.Electric;
        resistance.Add(new Skills());
        resistance[2].property = Skills.Property.Ice;

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].skill = Skills.Skill.Learned;
        }


    }


    


}
