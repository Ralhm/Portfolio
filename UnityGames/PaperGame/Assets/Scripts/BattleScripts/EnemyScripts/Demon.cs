using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : Enemy
{



    public Demon() : base()
    {
        basicAttack.property = Skills.Property.Bash;
        basicAttack.subProperty = Skills.Property.None;
        weakness.Add(new Skills());
        weakness.Add(new Skills());
        resistance.Add(new Skills());
        resistance.Add(new Skills());
        resistance.Add(new Skills());

        weakness[0].property = Skills.Property.Fire;
        weakness[1].property = Skills.Property.Ice;
        resistance[0].property = Skills.Property.Slash;
        resistance[1].property = Skills.Property.Stab;
        resistance[2].property = Skills.Property.Bash;
    }

    public override void Attack(List<Player> Victim) //Make this a generic function that calls on random coroutines, each one corresponding to a possible skill the enemy can use
    {
       
        print("Demon Attacking");
        StartCoroutine(StandardAttack(basicAttack, Victim, this));

    }
}
