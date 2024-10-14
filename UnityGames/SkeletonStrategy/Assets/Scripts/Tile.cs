using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public int G, H;

    public int F { get { return G + H; } }

    public bool isBlocked;

    public Tile Previous;

    public bool IsOccupied;

    public Character CurrentCharacter;

    public Vector3Int gridLocation;

    public void Init(bool isOffset)
    {
        //renderer.color = isOffset ? offsetColor : baseColor;
    }

    public bool GetIsOccupied()
    {
        return IsOccupied;
    }

    public void SetIsOccupied(bool input)
    {
        IsOccupied = input;
    }

    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void ShowAttackTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
    }

}
