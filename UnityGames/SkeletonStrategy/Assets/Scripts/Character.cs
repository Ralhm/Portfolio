using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public bool isAlly;
    public int Health;
    public int MaxHealth;
    public int MoveRange;
    public float Speed;

    public int AttackRange;
    public int AttackPower;

    public Tile CurrentTile;

    public bool HasActed = false;
    public bool HasAttacked = false;
    public bool HasMoved = false;
    public bool SelectingAttack = false;


    protected bool IsSelected;
    protected bool IsMoving;
    protected int DistanceToTravel = 0;
    protected int DistanceTraveled = 0;
    protected int AllyIndex;
    protected int EnemyIndex;

    protected Pathfinder pathFinder;
    protected RangeFinder rangeFinder;
    protected List<Tile> Path;
    protected List<Tile> tilesInRange;

    public HealthBar healthBar;


    public void SetAllyIndex(int index)
    {
        AllyIndex = index;
    }

    public void SetEnemyIndex(int index)
    {
        EnemyIndex = index;
    }

    public int GetAllyIndex()
    {
        return AllyIndex;
    }

    public void CenterPositionOnTile(Tile tile)
    {
        transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        CurrentTile = tile;
    }

    //Center unit on tile during start
    public void GetCurrentTile()
    {
        Vector3 unitPos = Camera.main.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z + 10));
        Vector2 unitPos2D = new Vector2(unitPos.x, unitPos.y);

        RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z + 10), new Vector3(transform.position.x, transform.position.y, transform.position.z + 10));

        CenterPositionOnTile(hit.collider.gameObject.GetComponent<Tile>());
    }

    public void BeginMovement(Tile EndTile)
    {

        if (EndTile != null)
        {
            Path = pathFinder.FindPath(CurrentTile, EndTile);

            if (Path.Count > MoveRange)
            {
                Debug.Log("Out of Range!");
                Path.Clear();
                return;
            }

            HideInRangeTiles();
            DistanceToTravel = Path.Count;
            UnSetTile();
            EndTile.ShowTile();
            IsMoving = true;
            MapManager.Instance.InAction = true;



        }
        else
        {
            Debug.Log("No End Tile!");

        }
    }

    public virtual void GetInRangeTiles()
    {
        tilesInRange = rangeFinder.GetTilesInRange(CurrentTile, MoveRange);

        foreach(Tile tile in tilesInRange)
        {
            tile.ShowTile();
        }
    }

    //Poorly named and implemented function, meant for AI enemies to get a list of tiles within the target character, needs work
    public List<Tile> GetAttackRangeTiles(Tile inputTile, int range)
    {
        List<Tile> list = rangeFinder.GetTilesInRange(inputTile, range);

        foreach (Tile tile in tilesInRange)
        {
            //tile.ShowTile();
        }

        return list;

    }

    public void HideInRangeTiles()
    {
        tilesInRange = rangeFinder.GetTilesInRange(CurrentTile, MoveRange);

        foreach (Tile tile in tilesInRange)
        {
            tile.HideTile();
        }
    }

    public void GetInRangeAttackTiles()
    {
        tilesInRange = rangeFinder.GetTilesInRange(CurrentTile, AttackRange);

        foreach (Tile tile in tilesInRange)
        {
            tile.ShowAttackTile();
        }
    }

    public virtual void Die()
    {
        UnSetTile();
        gameObject.SetActive(false);
    }

    public virtual void MoveAlongPath()
    {
        float step = Speed * Time.deltaTime;



        float zVal = Path[0].transform.position.z;

        transform.position = Vector3.MoveTowards(transform.position, Path[0].transform.position, step);
        transform.position = new Vector3(transform.position.x, transform.position.y, zVal);

        if (Vector2.Distance(transform.position, Path[0].transform.position) < 0.01f) {

            DistanceTraveled++;

            CenterPositionOnTile(Path[0]);
            Path.RemoveAt(0);
        }

        if (DistanceTraveled == DistanceToTravel)
        {
            FinishedMoving();
            
        }
    }

    public virtual void FinishedMoving()
    {
        DistanceTraveled = 0;
        IsMoving = false;
        SelectingAttack = true;
        MapManager.Instance.InAction = false;
        HasMoved = true;
        GetInRangeAttackTiles();
        SetTile();
    }

    public virtual void Attack(Character Target)
    {
        //Jaaaaaaaaank
        for (int i = 0; i < tilesInRange.Count; i++)
        {
            if (Target.CurrentTile == tilesInRange[i])
            {
                Target.TakeDamage(AttackPower);
                HasActed = true;
            }
        }
        EndTurn();
        SelectingAttack = false;


    }

    public void EndTurn()
    {
        Debug.Log("Ending Turn!");     
        MapManager.Instance.IsAllySelected = false;
        SelectingAttack = false;
        HasActed = true;        
        HideInRangeTiles();

        MapManager.Instance.IncrementPlayerActions();
    }

    public virtual void UndoAttack(Character Attacker, Character Target)
    {
        if (!Target.gameObject.activeSelf)
        {
            Debug.Log("Target Name: " + Target.gameObject.name);
            Target.UndoDeath(Target); //This should not be like this
        }

        Target.UndoDamage(Attacker.AttackPower);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }

        healthBar.SetValue(Health);

        Debug.Log(gameObject.name + " Taking Damage, Current Health: %d" + Health);
    }

    public void UndoDamage(int damage)
    {
        Health += damage;
        healthBar.SetValue(Health);
    }

    public virtual void UndoDeath(Character Target)
    {
        Target.gameObject.SetActive(true);
        Target.GetCurrentTile();
        Target.SetTile();
        healthBar.SetValue(Health);
    }

    public virtual void UndoMovement(Tile StartTile, Tile EndTile)
    {
        if (HasActed)
        {
            HasActed = false;
        }
        UnSetTile(EndTile);
        CurrentTile.HideTile();
        transform.position = StartTile.transform.position;
        SetTile(StartTile);
        MapManager.Instance.IsAllySelected = false;
        HasMoved = false;
        HideInRangeTiles();
        //CurrentTile.ShowTile();

    }

    public void RedoMovement(Tile startTile)
    {
        CenterPositionOnTile(startTile);
        DistanceTraveled = 0;
        IsMoving = false;
        SelectingAttack = true;
        MapManager.Instance.InAction = false;
        HasMoved = true;
        GetInRangeAttackTiles();
        SetTile();
    }

    public void SetTile(Tile newTile)
    {
        //Debug.Log("Setting Tile:: " + gameObject.name);
        CurrentTile = newTile;
        CurrentTile.SetIsOccupied(true);
        CurrentTile.CurrentCharacter = this;
    }

    public void SetTile()
    {
        CurrentTile.SetIsOccupied(true);
        CurrentTile.CurrentCharacter = this;
    }

    public void UnSetTile()
    {

        CurrentTile.SetIsOccupied(false);
        CurrentTile.CurrentCharacter = null;
    }

    public void UnSetTile(Tile inputTile)
    {

        inputTile.SetIsOccupied(false);
        inputTile.CurrentCharacter = null;
    }

    public void ResetActionBools()
    {
        HasActed = false; 
        HasMoved = false; 
        HasAttacked = false; 
        SelectingAttack = false;
    }

    public virtual void Wait()
    {

    }
}
