using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Ally : Character
{
    // Start is called before the first frame update
    void Start()
    {
        rangeFinder = new RangeFinder();
        pathFinder = new Pathfinder();
        Path = new List<Tile>();
        tilesInRange = new List<Tile>();

        //Quick and dirty
        MaxHealth = Health;
        healthBar.SetMaxValue(MaxHealth);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsMoving)
        {
            MoveAlongPath();
        }
    }

    public override void Die()
    {
        MapManager.Instance.NumAllies--;
        UnSetTile();
        gameObject.SetActive(false);
    }

    public override void UndoDeath(Character Target)
    {
        base.UndoDeath(Target);
        Debug.Log("Undoing Ally Death!");
        MapManager.Instance.NumAllies++;
    }

    public override void UndoMovement(Tile StartTile, Tile EndTile)
    {
        Debug.Log("Undoing ALLY movement!");
        base.UndoMovement(StartTile, EndTile);
        MapManager.Instance.ActedAllies--;
    }

}
