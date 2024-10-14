using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Things that could be better:
//GridData has a RoomInfo value
//Replacement function instead of bullshit inside 
//Like everything lmao you seriously need to restructure/start from scratch if you do a generation like this again
//Everything is spread apart
//Just use a goddamn Vector2 instead of Vector2Int
//Maybe even a vector3 since that would be easier for the positioning
//The whole mapdata file ought to be reworked, not enough functions that return what should be returned, or what is there is too roundabout
//Lots of defunct unused shit
//Instead of deciding room types in the MapController queue, decide on all room types beforehand after crawling, and then spawn them
//Maybe it's not that bad, try keeping the height/width the same on bigger rooms and try centering them based on the average position rooms they replace



//TODO LIST
//Replace slapped on tiles in rooms with tilemaps
//incentive speed, get an item or special currency if you clear a floor quickly




public enum Direction
{
    Up, Left, Down, Right
};

public class MapGenerator : MonoBehaviour
{

    public MapData mapGenerationData;

    public Grid TileGrid = new Grid();
    public static List<Vector2Int> positionsVisited = new List<Vector2Int>();
    private static readonly Dictionary<Direction, Vector2Int> directionMovementMap = new Dictionary<Direction, Vector2Int>
    {
        {Direction.Up, Vector2Int.up },
        {Direction.Left, Vector2Int.left },
        {Direction.Down, Vector2Int.down },
        {Direction.Right, Vector2Int.right }
    };

    private void Awake()
    {
        positionsVisited = new List<Vector2Int>();
        SetSeed();
    }

    private void Start()
    {
        //MapController.instance.LoadRoom("Start", new Vector2(0, 0));

        TileGrid.AddGridSquare(new Vector2Int(0, 0), RoomType.Normal, RoomProperty.Start);
        //TileGrid.MapRooms[0].IsOccuppied = true;
        TileGrid.MapRoomsData[0].roomProperty = RoomProperty.Start;
        TileGrid.MapRoomsData[0].roomType = RoomType.Normal;
        
        TileGrid.MapRooms = GenerateMap(mapGenerationData);
        TileGrid.CreateGrid();




        SpawnRooms(TileGrid.MapRoomsData);
        StartCoroutine(DelayRooms());
        //MapController.instance.RemoveUnconnectedDoors();
    }
    private void SetSeed(int state = 0)
    {
        if (state == 0)
        {
            state = System.DateTime.Now.Millisecond;
        }
        Debug.Log("Random seed: " + state);
        Random.InitState(state);
    }

    IEnumerator DelayRooms()
    {
        yield return new WaitForSeconds(2.0f);
        MapController.instance.RemoveUnconnectedDoors();
        MapController.instance.SetKeyRoom();
    }


    public static List<Vector2Int> GenerateMap(MapData mapData)
    {
        List<MapCrawler> mapCrawlers = new List<MapCrawler>();

        for(int i = 0; i < mapData.NumberOfCrawlers; i++)
        {
            mapCrawlers.Add(new MapCrawler(Vector2Int.zero));
        }

        int iterations = Random.Range(mapData.IterationMin, mapData.IterationMax);

        for (int i = 0; i < iterations; i++)
        {
            foreach(MapCrawler crawler in mapCrawlers)
            {
                Vector2Int newPos = crawler.Move(directionMovementMap);

                if (!positionsVisited.Contains(newPos))
                {
                    if (newPos != new Vector2Int(0, 0)) //strongarm check to prevent start duplicates
                    {
                        positionsVisited.Add(newPos);
                    }
                   
                }


            }
        }
        Debug.Log("Finished Crawling!!!");
        return positionsVisited;
        
    }


    public void SpawnRooms(List<GridData> rooms)
    {
        Debug.Log("Beginning to spawn rooms!!!!");
        MapController.instance.LoadRoom("Start", new Vector2Int(0, 0), RoomType.Normal);
        foreach(GridData room in rooms)
        {

            int rand = Random.Range(0, 3);
            //Debug.Log(rand);

            SpawnRoom(room);



        }


        
    }

    
    public bool IsGridSquareOccuppied(Vector2Int Location)
    {
        return TileGrid.CheckIfOccuppied(Location);
    }
    


    public void SpawnRoom(GridData Room)
    {
        if (Room.roomProperty == RoomProperty.Boss)
        {
            MapController.instance.LoadRoom("BossRoom", Room);
            return;
        }
        else if (Room.roomProperty == RoomProperty.Shop)
        {
            MapController.instance.LoadRoom("ShopRoom", Room);
            return;
        }
        else if (Room.roomProperty == RoomProperty.Treasure)
        {
            MapController.instance.LoadRoom("TreasureRoom", Room);
            return;
        }
        else if (Room.roomProperty == RoomProperty.MagicShop)
        {
            MapController.instance.LoadRoom("MagicShop", Room);
            return;
        }

        switch (Room.roomType)
        {
            case RoomType.Normal:
                MapController.instance.LoadRoom(mapGenerationData.ChooseNormalRoom(), Room);
                break;
            case RoomType.Tall:
                MapController.instance.LoadRoom(mapGenerationData.ChooseTallRoom(), Room);
                break;
            case RoomType.Long:
                MapController.instance.LoadRoom(mapGenerationData.ChooseLongRoom(), Room);
                break;
            case RoomType.Big:
                MapController.instance.LoadRoom(mapGenerationData.ChooseBigRoom(), Room);
                break;
        }


        
    }

    public bool DoesTileExist(Vector2Int tile)
    {
        return TileGrid.CheckIfRoomIsNotOccuppied(tile);
    }


}
