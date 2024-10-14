using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitAction : Command
{
    public AttackCommand attackCommand;
    public MoveCommand moveCommand;

    public UnitAction(AttackCommand attackComm, MoveCommand moveComm)
    {
        attackCommand = attackComm;
        moveCommand = moveComm;
    }

    public override void Execute()
    {
        base.Execute();
        moveCommand.Execute();
        attackCommand.Execute();
        
    }

    public override void Undo()
    {
        base.Execute();
        attackCommand.Undo();
        moveCommand.Undo();
    }

    public override void Redo()
    {
        base.Redo();
        moveCommand.Redo();
        attackCommand.Redo();

    }
}
