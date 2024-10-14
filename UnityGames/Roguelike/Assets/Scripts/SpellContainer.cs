using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellContainer : Container
{
    [SerializeField]
    public Spell[] Spells;
    public Spell GetRandomSpell()
    {
        int rand = Random.Range(0, Spells.Length);

        return Spells[rand];
    }
}
