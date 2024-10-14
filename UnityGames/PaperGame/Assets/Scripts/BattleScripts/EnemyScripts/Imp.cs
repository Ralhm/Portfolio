using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : Enemy
{

    public Imp() : base()
    {
        basicAttack.property = Skills.Property.Slash;
        basicAttack.subProperty = Skills.Property.None;

        weakness.Add(new Skills());
        weakness[0].property = Skills.Property.Stab;
        weakness.Add(new Skills());
        weakness[1].property = Skills.Property.Electric;
    }
    public override void Attack(List<Player> Victim) //Make this a generic function that calls on random coroutines, each one corresponding to a possible skill the enemy can use
    {
        
        print("Imp Attacking");
        StartCoroutine(StandardAttack(basicAttack, Victim, this));
    }
}
