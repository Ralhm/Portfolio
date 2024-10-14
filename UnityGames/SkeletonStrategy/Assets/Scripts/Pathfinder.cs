using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    public List<Tile> FindPath(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            Tile CurrentTile = openList.OrderBy(x => x.F).First();

            openList.Remove(CurrentTile);
            closedList.Add(CurrentTile);

            if (CurrentTile == end)
            {
                return GetFinishedList(start, end);
            }

            List<Tile> adjacentTiles = MapManager.Instance.GetAdjacentTiles(CurrentTile);

            foreach (Tile adjacentTile in adjacentTiles)
            {
                if (adjacentTile.isBlocked || closedList.Contains(adjacentTile) || Mathf.Abs(CurrentTile.gridLocation.z - adjacentTile.gridLocation.z) > 1)
                {
                    continue;
                }

                adjacentTile.G = GetDistanceToPoint(start, adjacentTile);
                adjacentTile.H = GetDistanceToPoint(end, adjacentTile);

                adjacentTile.Previous = CurrentTile;

                if (!openList.Contains(adjacentTile)) { 
                    openList.Add(adjacentTile);
                }
            }
        }

        return new List<Tile>();
    }


    private List<Tile> GetFinishedList(Tile start, Tile end)
    {
        List<Tile> finishedList = new List<Tile>();

        Tile CurrentTile = end;

        while (CurrentTile != start)
        {
            finishedList.Add(CurrentTile);
            CurrentTile = CurrentTile.Previous;
        }

        finishedList.Reverse();

        return finishedList;

    }

    private int GetDistanceToPoint(Tile start, Tile adjacentTile)
    {
        return Mathf.Abs(start.gridLocation.x - adjacentTile.gridLocation.x) + Mathf.Abs(start.gridLocation.y - adjacentTile.gridLocation.y);
    }


}
