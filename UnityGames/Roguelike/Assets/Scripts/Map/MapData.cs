
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


[CreateAssetMenu(fileName = "MapGenerationData.asset", menuName = "MapGenerationData/Map Data")]
public class MapData : ScriptableObject
{
    public int NumberOfCrawlers;
    public int IterationMin;
    public int IterationMax;
    public List<string> NormalRooms;


    public List<string> TallRooms;
    public List<string> LongRooms;
    public List<string> BigRooms;
    public List<string> BossRooms;


    public string ChooseNormalRoom()
    {
        int rand = Random.Range(0, NormalRooms.Count);
        return NormalRooms[rand];
    }

    public string ChooseTallRoom()
    {
        int rand = Random.Range(0, TallRooms.Count);
        return TallRooms[rand];
    }
    public string ChooseLongRoom()
    {
        int rand = Random.Range(0, LongRooms.Count);
        return LongRooms[rand];
    }

    public string ChooseBigRoom()
    {
        int rand = Random.Range(0, BigRooms.Count);
        return BigRooms[rand];
    }

    public string BossRoom()
    {
        return BossRooms[0];
    }
}

[System.Serializable]
public class Grid
{
    public List<GridData> MapRoomsData = new List<GridData>();
    public List<Vector2Int> MapRooms = new List<Vector2Int>();
    public List<Vector2Int> OccuppiedMapTiles = new List<Vector2Int>();

    //Create grid objects from the list of MapRooms
    //Decide what each grid should be here
    //Then pass that list into the LoadRooms functions
    public void CreateGrid()
    {


        AddGridSquare(new Vector2Int(0, 0), RoomType.Normal, RoomProperty.Start); //Designate Start Room first
        MapRoomsData[0].roomProperty = RoomProperty.Start;
        MapRoomsData[0].roomType = RoomType.Normal;

        CreateBossRoom();
        CreateSpecialRoom(RoomProperty.Shop);
        CreateSpecialRoom(RoomProperty.Treasure);
        CreateSpecialRoom(RoomProperty.MagicShop);
        
        int rand = 0;

        for (int i = 0; i < MapRooms.Count; i++)
        {       
            if (!CheckIfRoomIsNotOccuppied(MapRooms[i])) {
                continue;
            }            
            rand = Random.Range(0, 100);            
            if (rand < 10)
            {
                if (AttemptCreateBigRoom(MapRooms[i]))
                {
                    continue;
                }
            }
                      
            if (rand > 10 && rand < 20)
            {
                if (AttemptCreateLongRoom(MapRooms[i]))
                {
                    continue;
                }
            }          
            if (rand > 20 && rand < 30)
            {
                if (AttemptCreateTallRoom(MapRooms[i]))
                {
                    continue;
                }
            }            
            AddGridSquare(MapRooms[i], RoomType.Normal);
        }
    }

    void SetTileOccuppied(Vector2Int roomPos)
    {
        //MapRooms.Remove(roomPos);
        OccuppiedMapTiles.Add(roomPos);
    }

    public bool AttemptCreateTallRoom(Vector2Int roomPos)
    {
        if (CheckAbove(roomPos))
        {
            AddGridSquare(new List<Vector2Int>() { roomPos, ReturnAbove(roomPos) }, RoomType.Tall);


            return true;
        }
        else if (CheckBelow(roomPos))
        {
            AddGridSquare(new List<Vector2Int>() { roomPos, ReturnBelow(roomPos) }, RoomType.Tall);
            return true;
        }
        return false;
    }

    public bool AttemptCreateLongRoom(Vector2Int roomPos)
    {
        if (CheckRight(roomPos))
        {
            AddGridSquare(new List<Vector2Int>() { roomPos, ReturnRight(roomPos) }, RoomType.Long);
            return true;
        }
        else if (CheckLeft(roomPos))
        {
            AddGridSquare(new List<Vector2Int>() { roomPos, ReturnLeft(roomPos) }, RoomType.Long);
            
            return true;
        }
        return false;
    }

    public bool AttemptCreateBigRoom(Vector2Int roomPos)
    {
        if (CheckRight(roomPos) && CheckBelow(roomPos) && CheckIfRoomIsNotOccuppied(new Vector2Int(roomPos.x + 1, roomPos.y - 1)))
        {
            AddGridSquare(new List<Vector2Int>() { roomPos, ReturnRight(roomPos), ReturnBelow(roomPos), new Vector2Int(roomPos.x + 1, roomPos.y - 1) }, RoomType.Big);
            return true;
        }
        else if (CheckLeft(roomPos) && CheckAbove(roomPos) && CheckIfRoomIsNotOccuppied(new Vector2Int(roomPos.x - 1, roomPos.y + 1)))
        {
            AddGridSquare(new List<Vector2Int>() { roomPos, ReturnLeft(roomPos), ReturnAbove(roomPos), new Vector2Int(roomPos.x - 1, roomPos.y + 1) }, RoomType.Big);
            return true;
        }
        return false;
    }


    public void CreateBossRoom()
    {
        int i = MapRooms.Count - 1;
        while (i > 0)
        {
            //Check just to make sure
            if (FindEmptyNeighbour(MapRooms[i], RoomProperty.Boss))
            {
                break;
            }

            i--;
        }    
    }

    public void CreateSpecialRoom(RoomProperty property)
    {
        int rand = Random.Range(1, MapRooms.Count - 2);
        int i = MapRooms.Count - 1;
        while (i > 0)
        {

            if (FindEmptyNeighbour(MapRooms[rand], property))
            {
                break;
            }
            rand = Random.Range(1, MapRooms.Count - 2);

            i--;
        }
    }

    public bool FindEmptyNeighbour(Vector2Int room, RoomProperty property)
    {

        if (!CheckAboveMap(room) && CheckAbove(room))
        {
            AddGridSquare(ReturnAbove(room), RoomType.Normal, property);
            return true;
        }
        else if (!CheckRightMap(room) && CheckRight(room))
        {
            AddGridSquare(ReturnRight(room), RoomType.Normal, property);
            return true;
        }
        else if (!CheckBelowMap(room) && CheckBelow(room))
        {
            AddGridSquare(ReturnBelow(room), RoomType.Normal, property);
            return true;
        }
        else if (!CheckLeftMap(room) && CheckLeft(room))
        {
            AddGridSquare(ReturnLeft(room), RoomType.Normal, property);
            return true;
        }
        return false;
    }

    public void AddGridSquare(Vector2Int room, RoomType type)
    {
        MapRoomsData.Add(new GridData(room, type));
        SetTileOccuppied(room);
    }

    public void AddGridSquare(Vector2Int room, RoomType type, RoomProperty property)
    {
        MapRoomsData.Add(new GridData(room, type, property));
        SetTileOccuppied(room);
    }

    public void AddGridSquare(List<Vector2Int> Rooms, RoomType type)
    {
        MapRoomsData.Add(new GridData(Rooms, type));

        for (int i = 0; i <  Rooms.Count; i++)
        {
            SetTileOccuppied(Rooms[i]);
        }     
    }

    public void RemoveGrid(Vector2Int room)
    {
        GridData grid = GetRoom(room);

        MapRoomsData.Remove(grid);
    }

    public GridData GetRoom(Vector2Int pos)
    {
        for (int i = 0; i < MapRoomsData.Count; i++)
        {
            if (MapRoomsData[i].Position == pos)
            {
                return MapRoomsData[i];
            }
        }
        Debug.Log("RETURNING NULL!!!");

        return null;
    }

    public bool CheckIfOccuppied(Vector2Int loc)
    {
        for (int i = 0; i < MapRoomsData.Count; i++)
        {
            if (MapRoomsData[i].Position == loc)
            {
                if (MapRoomsData[i].IsOccuppied)
                {
                    return true;
                }

               
            }
        }
        return false;
    }

    public bool CheckIfExists(Vector2Int pos)
    {
        for (int i = 0; i < MapRoomsData.Count; i++)
        {
            if (MapRoomsData[i].Position == pos)
            {
                return true;
            }
        }
        return false;
    }

    public void SetOccuppied(Vector2Int loc)
    {
        for (int i = 0; i < MapRoomsData.Count; i++)
        {
            if (MapRoomsData[i].Position == loc)
            {
                MapRoomsData[i].IsOccuppied = true;


            }
        }
    }

    public bool ContainsRoom(Vector2Int loc)
    {
        for (int i = 0; i < MapRoomsData.Count; i++)
        {
           if (MapRoomsData[i].Position == loc)
            {
                //MapRooms[i].IsOccuppied = true;
                return true;
            }
        }
        return false;
    }

    public bool CheckIfRoomIsNotOccuppied(Vector2Int loc)
    {

        if (OccuppiedMapTiles.Contains(loc))
        {
            return false;
        }
        return true;

    }

    public bool CheckAbove(Vector2Int loc)
    {
        return CheckIfRoomIsNotOccuppied(new Vector2Int(loc.x, loc.y + 1));

    }



    public bool CheckBelow(Vector2Int loc)
    {
        return CheckIfRoomIsNotOccuppied(new Vector2Int(loc.x, loc.y - 1));
    }

    public bool CheckRight(Vector2Int loc)
    {
        return CheckIfRoomIsNotOccuppied(new Vector2Int(loc.x + 1, loc.y));
    }


    public bool CheckLeft(Vector2Int loc)
    {
        return CheckIfRoomIsNotOccuppied(new Vector2Int(loc.x - 1, loc.y));
    }

    public bool CheckAboveMap(Vector2Int loc)
    {
        return MapRooms.Contains(new Vector2Int(loc.x, loc.y + 1));
    }

    public bool CheckBelowMap(Vector2Int loc)
    {
        return MapRooms.Contains(new Vector2Int(loc.x, loc.y - 1));
    }

    public bool CheckLeftMap(Vector2Int loc)
    {
        return MapRooms.Contains(new Vector2Int(loc.x - 1, loc.y));
    }

    public bool CheckRightMap(Vector2Int loc)
    {
        return MapRooms.Contains(new Vector2Int(loc.x + 1, loc.y));
    }

    public Vector2Int ReturnAbove(Vector2Int loc)
    {
        return new Vector2Int(loc.x, loc.y + 1);
    }

    public Vector2Int ReturnBelow(Vector2Int loc)
    {
        return new Vector2Int(loc.x, loc.y - 1);
    }

    public Vector2Int ReturnRight(Vector2Int loc)
    {
        return new Vector2Int(loc.x + 1, loc.y);
    }

    public Vector2Int ReturnLeft(Vector2Int loc)
    {
        return new Vector2Int(loc.x - 1, loc.y);
    }

    public void SetType(Vector2Int loc, RoomType type)
    {
        GetRoom(loc).roomType = type;
    }

    public RoomType GetType(Vector2Int loc)
    {
        return GetRoom(loc).roomType;
    }
}

[System.Serializable]
public class GridData
{

    public GridData(Vector2Int loc, RoomType type)
    {

        Position = loc;
        roomType = type;
        MultiRoom = false;
        Tiles.Add(loc);
    }

    public GridData(Vector2Int loc, RoomType type, RoomProperty property)
    {

        Position = loc;
        roomType = type;
        roomProperty = property;
        MultiRoom = false;
        Tiles.Add(loc);
    }


    public GridData(List<Vector2Int> Positions, RoomType type)
    {
        Position = CalculateAveragePosition(Positions);
        roomType = type;
        MultiRoom = true;
        Tiles = Positions;
    }

    public GridData(List<Vector2Int> Positions, RoomType type, RoomProperty property)
    {
        Position = CalculateAveragePosition(Positions);
        roomType = type;
        roomProperty = property;
        MultiRoom = true;
        Tiles = Positions;
    }

    public GridData(GridData data)
    {

        Position = data.Position;
        roomType = data.roomType;
        MultiRoom = data.MultiRoom;
        Tiles = data.Tiles;
    }


    public Vector2 CalculateAveragePosition(List<Vector2Int> Positions)
    {
        Vector2 pos = new Vector2(0, 0);
        for (int i = 0; i < Positions.Count; i++)
        {
            pos += Positions[i];
        }
        pos /= Positions.Count;
        return pos;
    }

    public string RoomName;
    public List<Vector2Int> Tiles = new List<Vector2Int>();
    public Vector2 Position;
    public bool IsOccuppied = false;
    public RoomType roomType = RoomType.Normal;
    public RoomProperty roomProperty = RoomProperty.Combat;
    public bool MultiRoom = false;


    //Tell each door which grid tile they occuppy and which grid tile they point to
    //When going through a door, pass in the connecting grid tile to decide which door you're coming from
    //teleport to the owning grid tiles door


    //store the tile the door occuppies in each door
    //when going through a door, tell the room what tile you're coming from and the direction of the door
    //in the mapcontroller, find the room with the tile neighboring the passed in tile
    //then in the room, find the door with the passed in tile, teleport to that door.
    //to decide if the door is connected or not


    //door tells mapcontroller which direction and tile we're coming from
    //mapcontroller finds room with tile
    //room finds door with matching tile and direction
    //assigns connected door this way

    public Vector2Int GetBottomLeftTile()
    {
        Vector2 Pos = Position;
        Pos += new Vector2(-0.5f, -0.5f);

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }

    public Vector2Int GetBottomRightTile()
    {
        Vector2 Pos = Position;
        Pos += new Vector2(0.5f, -0.5f);

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }

    public Vector2Int GetTopLeftTile()
    {
        Vector2 Pos = Position;
        Pos += new Vector2(-0.5f, 0.5f);

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }

    public Vector2Int GetTopRightTile()
    {
        Vector2 Pos = Position;
        Pos += new Vector2(0.5f, 0.5f);

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }

    public Vector2Int GetTopTile()
    {
        Vector2 Pos = Position;
        Pos += new Vector2(0, 0.5f);

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }

    public Vector2Int GetBottomTile()
    {
        Vector2 Pos = Position;
        Pos += new Vector2(0, -0.5f);

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }

    public Vector2Int GetLeftTile()
    {
        Vector2 Pos = Position;
        Pos += new Vector2(-0.5f, 0);

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }

    public Vector2Int GetRightTile()
    {
        Vector2 Pos = Position;
        Pos += new Vector2(0.5f, 0);

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }

    public Vector2Int GetNormalTile()
    {
        Vector2 Pos = Position;

        return new Vector2Int((int)Pos.x, (int)Pos.y);
    }
}
