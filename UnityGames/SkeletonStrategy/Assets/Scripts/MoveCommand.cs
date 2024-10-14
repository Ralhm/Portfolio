using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveCommand : Command
{

    public Character StoredCharacter;
    public Tile StoredEndTile;
    public Tile StoredStartTile;

    public MoveCommand(Character character, Tile EndTile, Tile StartTile) { 
        StoredCharacter = character;
        StoredEndTile = EndTile; 
        StoredStartTile = StartTile;
    }


    public override void Execute()
    {
        StoredCharacter.BeginMovement(StoredEndTile);
    }

    public override void Undo()
    {
        Debug.Log("Character: " + StoredCharacter.gameObject.name);
        StoredCharacter.UndoMovement(StoredStartTile, StoredEndTile);
    }

    public override void Redo()
    {
        StoredCharacter.RedoMovement(StoredEndTile);
    }
}
