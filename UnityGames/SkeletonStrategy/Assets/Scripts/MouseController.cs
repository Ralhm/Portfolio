using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseController : MonoBehaviour
{




    // Update is called once per frame
    void Update()
    {
        var focusedTileHit = GetFocusedOnTile();

        if (focusedTileHit.HasValue)
        {
            GameObject HighlightedTile = focusedTileHit.Value.collider.gameObject;
            transform.position = HighlightedTile.transform.position;
            //gameObject.GetComponent<SpriteRenderer>().sortingOrder = HighlightedTile.GetComponent<SpriteRenderer>().sortingOrder;


            if (Input.GetMouseButtonDown(0) && !MapManager.Instance.InAction && MapManager.Instance.AllyPhase)
            {
                Tile SelectedTile = HighlightedTile.GetComponent<Tile>();
                if (SelectedTile.CurrentCharacter == null && !MapManager.Instance.IsAllySelected)
                {
                    Debug.Log("Returning!");
                    return;
                }


                


                if (MapManager.Instance.IsAllySelected)
                {
                    if (MapManager.Instance.AllyList[MapManager.Instance.SelectedAllyIndex].HasMoved)
                    {
                        MapManager.Instance.SelectedTile = SelectedTile;
                        Debug.Log("Has Moved!!");


                        MapManager.Instance.ClearFutureCommandsAfterNewCommand();
                        if (SelectedTile.CurrentCharacter != null)
                        {
                            Debug.Log("Attacking!");
                            MapManager.Instance.ExecuteAttackCommand(SelectedTile.CurrentCharacter);
                        }
                        else
                        {
                            Debug.Log("Doing nothing!");
                            MapManager.Instance.ExecuteNothingCommand(SelectedTile.CurrentCharacter);
                            
                        }
                        
                    }
                    else
                    {
                        if (!SelectedTile.GetIsOccupied())
                        {

                            MapManager.Instance.ExecuteMoveCommand(SelectedTile);

                        }
                    }


                }
                else
                {
                    
                    if (SelectedTile.CurrentCharacter != null)
                    {
                        if (SelectedTile.CurrentCharacter.GetComponent<Enemy>())
                        {
                            Debug.Log("Can't click on enemies!");
                            return;
                        }

                        Debug.Log("Selecting Ally!");

                        if (SelectedTile.CurrentCharacter.HasActed)
                        {
                            Debug.Log("Character Already Acted!");
                            return;
                        }
                    
                        if (MapManager.Instance.SelectedTile != null)
                        {
                            MapManager.Instance.SelectedTile.HideTile();
                        }
                        MapManager.Instance.SelectedTile = SelectedTile;
                        SelectedTile.ShowTile();
                        MapManager.Instance.SetSelectedAlly(SelectedTile.CurrentCharacter.GetAllyIndex());
                        MapManager.Instance.IsAllySelected = true;
                    }
                }

                
            }

        }
    }

    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }


}
