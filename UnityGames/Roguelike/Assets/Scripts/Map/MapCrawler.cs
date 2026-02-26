using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCrawler : MonoBehaviour
{
    public Vector2Int Position { get; set; }

    public MapCrawler(Vector2Int startPos)
    {
        Position = startPos;
    }

    public Vector2Int Move(Dictionary<Direction, Vector2Int> directionMovementMap)
    {

        Direction DirToMove = (Direction)Random.Range(0, directionMovementMap.Count);
        Position += directionMovementMap[DirToMove];
        return Position;
    }
}
