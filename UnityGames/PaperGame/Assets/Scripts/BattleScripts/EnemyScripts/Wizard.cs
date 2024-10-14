using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    public Wizard() : base()
    {
        basicAttack.property = Skills.Property.Fire;
        basicAttack.subProperty = Skills.Property.None;

        weakness.Add(new Skills());
        weakness[0].property = Skills.Property.Bash;
        resistance.Add(new Skills());
        resistance[0].property = Skills.Property.Fire;
        resistance.Add(new Skills());
        resistance[1].property = Skills.Property.Electric;
        resistance.Add(new Skills());
        resistance[2].property = Skills.Property.Ice;
    }


    public override void Attack(List<Player> Victim) //Make this a generic function that calls on random coroutines, each one corresponding to a possible skill the enemy can use
    {
        
        print("Wizard Attacking");
        StartCoroutine(StandardAttack(basicAttack, Victim, this));
    }
}
