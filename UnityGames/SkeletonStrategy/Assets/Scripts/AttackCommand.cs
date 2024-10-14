using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackCommand : Command
{
    public bool DidNothing = false;
    public Character StoredAttacker;
    public Character StoredTarget;

    public AttackCommand(Character Attacker, Character Target, bool nothing = false)
    {
        StoredAttacker = Attacker;
        StoredTarget = Target;
        DidNothing = nothing;
    }

    public override void Execute()
    {
        if (!DidNothing)
        {
            StoredAttacker.Attack(StoredTarget);
        }
        else
        {
            StoredAttacker.EndTurn();
        }
    }

    public override void Undo()
    {
        if (!DidNothing)
        {
            StoredAttacker.UndoAttack(StoredAttacker, StoredTarget);

        }
        else
        {

        }
    }

    public override void Redo()
    {
        base.Redo();
        if (!DidNothing)
        {
            StoredAttacker.Attack(StoredTarget);
        }
        else
        {
            StoredAttacker.EndTurn();
        }
    }

}
