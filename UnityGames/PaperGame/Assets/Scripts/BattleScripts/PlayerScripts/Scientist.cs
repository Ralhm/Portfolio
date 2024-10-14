using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist : Player
{
    public Scientist() : base()
    {
        skills.Add(attack);
        skills[0].property = Skills.Property.Stab;
        skills.Add(new Acupuncture());
        skills.Add(new DeliriumBomb());
        skills.Add(new ToxicBomb());
        skills.Add(new StrangeBrew());

        weakness.Add(new Skills());
        weakness[0].property = Skills.Property.Bash;
        weakness.Add(new Skills());
        weakness[1].property = Skills.Property.Fire;

        resistance.Add(new Skills());
        resistance[0].property = Skills.Property.Electric;

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].skill = Skills.Skill.Learned;
        }

    }
}
