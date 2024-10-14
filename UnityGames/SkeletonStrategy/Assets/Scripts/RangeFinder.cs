using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder 
{
    public List<Tile> GetTilesInRange(Tile currentTile, int range)
    {
        var inRangeTiles = new List<Tile>();
        int stepCount = 0;

        inRangeTiles.Add(currentTile);

        var tileForPreviousStep = new List<Tile>();
        tileForPreviousStep.Add(currentTile);

        while (stepCount < range)
        {
            var surroundingTiles = new List<Tile>();

            foreach (var tile in tileForPreviousStep)
            {
                surroundingTiles.AddRange(MapManager.Instance.GetAdjacentTiles(tile));
            }

            inRangeTiles.AddRange(surroundingTiles);
            tileForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;

        }

        return inRangeTiles.Distinct().ToList();

    }
}
