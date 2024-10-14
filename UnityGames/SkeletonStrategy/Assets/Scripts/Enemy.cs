using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : Character
{
    public Character Target;
    //JUST CYCLE THROUGH YOUR RANGE AND IF THERE"S SOMEONE THERE, MOVE TO ATTACK

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

    public override void FinishedMoving()
    {
        base.FinishedMoving();

        Attack(Target);
    }

    public override void Attack(Character Target)
    {
        //base.Attack(Target);

        Debug.Log("ENEMY ATTACKING!");

        Target.TakeDamage(AttackPower);
        EndTurn();

        MapManager.Instance.IncrementEnemyActions();



    }

    public override void GetInRangeTiles()
    {
        tilesInRange = rangeFinder.GetTilesInRange(CurrentTile, MoveRange + AttackRange);

        foreach (Tile tile in tilesInRange)
        {
            if (tile.IsOccupied && tile.CurrentCharacter.isAlly)
            {
                Debug.Log(gameObject.name + "Found Occuppied Ally Tile in Range: " + tile.CurrentCharacter.gameObject.name);
                List<Tile> PotentialMoveToTiles = GetAttackRangeTiles(tile, AttackRange);
                
                //Check if tile is within characters move range
                for (int i = 0; i < PotentialMoveToTiles.Count; i++)
                {
                    Path = pathFinder.FindPath(CurrentTile, PotentialMoveToTiles[i]);

                    if (!PotentialMoveToTiles[i].IsOccupied && Path.Count <= MoveRange)
                    {
                        BeginMovement(PotentialMoveToTiles[i]);
                        Target = tile.CurrentCharacter;
                        Debug.Log(gameObject.name + " Found someone to attack!");
                        return;
                    }
                }
            }
        }
        
        Debug.Log(gameObject.name + " Found no Allies to Attack!");
        MapManager.Instance.IncrementEnemyActions();
    }

    public override void Wait()
    {

    }

    public override void Die()
    {
        MapManager.Instance.NumEnemies--;
        base.Die();
    }

    public override void UndoDeath(Character Target)
    {
        base.UndoDeath(Target);
        Debug.Log("Undoing Enemy Death!");
        MapManager.Instance.NumEnemies++;
    }

    public void NextEnemy()
    {
        
    }
}
