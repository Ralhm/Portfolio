using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Command
{
    public virtual void Execute()
    {

    }

    public virtual void Undo()
    {

    }


    public virtual void Redo()
    {

    }
}
