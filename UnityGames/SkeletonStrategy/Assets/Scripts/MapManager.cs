using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;
    public static MapManager Instance {  get { return instance; } }

    public Tile TilePrefab;
    public GameObject TileContainer;

    public Dictionary<Vector2Int, Tile> Map;

    public List<Ally> AllyList;
    public List<Enemy> EnemyList;

    public List<UnitAction> CommandList;
    public List<Command> RedoList;

    MoveCommand CurrentMoveCommand;
    AttackCommand CurrentAttackCommand;

    public bool AllyPhase = true;

    public int ActedAllies = 0;
    public int ActedEnemies = 0;

    public int NumEnemies;
    public int NumAllies;

    public bool IsAllySelected = false;

    public bool InAction = false;

    public Tile SelectedTile;
    public int SelectedAllyIndex = 0;

    public int RedoVal = 0;

    // Start is called before the first frame update
    void Awake()
    {

        CommandList = new List<UnitAction>();
        RedoList = new List<Command>();


        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        Map = new Dictionary<Vector2Int, Tile>();

        BoundsInt bounds = tileMap.cellBounds;

        for (int i = bounds.max.z; i > bounds.min.z; i--)
        {
            for (int j = bounds.min.y; j < bounds.max.y; j++)
            {
                for (int k = bounds.min.x; k < bounds.max.x; k++)
                {
                    var tileLocation = new Vector3Int(k, j, i);
                    var tileKey = new Vector2Int(k, j);

                    if (tileMap.HasTile(tileLocation) && !Map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(TilePrefab, TileContainer.transform);
                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                        overlayTile.gridLocation = tileLocation;
                        Map.Add(tileKey, overlayTile);
                    }
                }
            }
        }

        NumAllies = AllyList.Count;
        NumEnemies = EnemyList.Count;
    }

    public void SetSelectedAlly(int index)
    {
        SelectedAllyIndex = index;
        AllyList[index].GetInRangeTiles();
    }

    public void IncrementPlayerActions()
    {
        ActedAllies++;
        if (ActedAllies == NumAllies)
        {
            EndPhase();
            ActedAllies = 0;
            return;
        }

    }

    public void IncrementEnemyActions()
    {
        ActedEnemies++;
        if (ActedEnemies == EnemyList.Count)
        {
            EndPhase();
            return;
        }
        Debug.Log("Acted Enemies: " + ActedEnemies);
        if (EnemyList[ActedEnemies].gameObject.activeSelf)
        {
            EnemyList[ActedEnemies].GetInRangeTiles();
        }
        else
        {
            IncrementEnemyActions();
        }
        
    }

    IEnumerator ClearDelay()
    {
        yield return new WaitForSeconds(0.1f);
        CommandList.Clear();
    }

    public void EndPhase()
    {
        if (AllyPhase)
        {
            Debug.Log("Entering Enemy phase!");
            CommandList.Clear();
            AllyPhase = false;

            for (int i = 0; i < EnemyList.Count; i++)
            {
                if (EnemyList[i].gameObject.activeSelf)
                {
                    EnemyList[i].ResetActionBools();
                    ActedEnemies = 0;
                }
            }

            EnemyList[ActedEnemies].GetInRangeTiles();

        }
        else
        {
            Debug.Log("Entering Ally phase!");

            AllyPhase = true;

            for (int i = 0; i < AllyList.Count; i++)
            {
                if (AllyList[i].gameObject.activeSelf)
                {
                    AllyList[i].ResetActionBools();
                    ActedAllies = 0;
                }
            }

            StartCoroutine(ClearDelay()); //Very hacky, but it doesn't clear the whole list for some reason otherwise
            RedoVal = 0;
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (!InAction && !AllyList[SelectedAllyIndex].SelectingAttack){
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Undo();

            }
            if (Input.GetKeyDown(KeyCode.E)) {
                Redo();
            }
        }


    }

    public IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < AllyList.Count; i++)
        {
            AllyList[i].GetCurrentTile();
            AllyList[i].SetAllyIndex(i);
            AllyList[i].SetTile(AllyList[i].CurrentTile);
        }

        for (int i = 0; i < EnemyList.Count; i++)
        {
            EnemyList[i].GetCurrentTile();
            EnemyList[i].SetEnemyIndex(i);
            EnemyList[i].SetTile(EnemyList[i].CurrentTile);
        }
    }

    public List<Tile> GetAdjacentTiles(Tile currentTile)
    {


        List<Tile> adjacentTiles = new List<Tile>();

        Vector2Int locationToCheckTop = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);
        if (Map.ContainsKey(locationToCheckTop))
        {
            adjacentTiles.Add(Map[locationToCheckTop]);
        }

        Vector2Int locationToCheckBottom = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);
        if (Map.ContainsKey(locationToCheckBottom))
        {
            adjacentTiles.Add(Map[locationToCheckBottom]);
        }

        Vector2Int locationToCheckRight = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);
        if (Map.ContainsKey(locationToCheckRight))
        {
            adjacentTiles.Add(Map[locationToCheckRight]);
        }

        Vector2Int locationToCheckLeft = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);
        if (Map.ContainsKey(locationToCheckLeft))
        {
            adjacentTiles.Add(Map[locationToCheckLeft]);
        }

        return adjacentTiles;
    }

    public void ExecuteMoveCommand(Tile EndTile)
    {
        CurrentMoveCommand = new MoveCommand(AllyList[SelectedAllyIndex], EndTile, AllyList[SelectedAllyIndex].CurrentTile);
        CurrentMoveCommand.Execute();

    }

    public void ExecuteAttackCommand(Character Target)
    {
      
        AttackCommand newAttack = new AttackCommand(AllyList[SelectedAllyIndex], Target);
        newAttack.Execute();
        UnitAction action = new UnitAction(newAttack, CurrentMoveCommand);
        CommandList.Add(action);
    }
    public void Undo()
    {
        Debug.Log("Command Count: " + CommandList.Count);

        if ((CommandList.Count - 1 - RedoVal) > -1)
        {
            //Debug.Log("Current Character Action: " + AllyList[SelectedAllyIndex]);
            CommandList[CommandList.Count - 1 - RedoVal].Undo();
            RedoVal++;
        }
    }

    public void Redo()
    {
        if (RedoVal > 0)
        {
            CommandList[CommandList.Count - RedoVal].Redo();
            RedoVal--;
        }
    }



    public void ExecuteNothingCommand(Character Target)
    {
        Debug.Log("Executing Nothing!!");
        AttackCommand newAttack = new AttackCommand(AllyList[SelectedAllyIndex], Target, true);
        newAttack.Execute();
        UnitAction action = new UnitAction(newAttack, CurrentMoveCommand);

        CommandList.Add(action);
    }

    public void ExecuteAction(bool ClearRange)
    {
        //CommandList[0].Execute();
    }



    //If you do a new command after undoing a few, remove the commands that came after
    public void ClearFutureCommandsAfterNewCommand()
    {
        
        if (RedoVal == CommandList.Count)
        {
            CommandList.Clear();
            RedoVal = 0;
            return;
        }

        if (RedoVal > 0)
        {
            Debug.Log("Clearing FUTURE Commands!");
            for (int i = 0; i < RedoVal; i++)
            {
                Debug.Log("Removing Command: " + (CommandList.Count - RedoVal));
                CommandList.Remove(CommandList[CommandList.Count - RedoVal]);
            }
            Debug.Log("Command Count AFTER removal: " + CommandList.Count);

            //CommandList.RemoveRange(CommandList.Count - 1 - RedoVal, CommandList.Count - 1);
            RedoVal = 0;
        }

    }



}
