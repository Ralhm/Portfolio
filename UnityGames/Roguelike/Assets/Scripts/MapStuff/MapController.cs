using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MapController : MonoBehaviour
{
    public static MapController instance;
    public MapGenerator MapGen;


    public int RoomHeight;
    public int RoomWidth;


    public string WorldName;
    GridData CurrentRoomData;
    public List<GameObject> DoorColliders;
    public Queue<GridData> RoomQueue = new Queue<GridData>();

    public List<Room> LoadedRooms = new List<Room>();

    bool isLoadingRoom = false;

    public Room CurrentRoom;

    //Debug bool
    public bool UseVoidMask;



    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }


    }


    public GridData LoadRoom(string name, GridData data)
    {
        GridData newRoomData = new GridData(data);
         

        newRoomData.RoomName = name;

        RoomQueue.Enqueue(newRoomData);
        return newRoomData;
    }

    public GridData LoadRoom(string name, Vector2Int pos, RoomType type)
    {
        GridData newRoomData = new GridData(pos, type);


        newRoomData.RoomName = name;

        RoomQueue.Enqueue(newRoomData);
        return newRoomData;
    }



    private void Start()
    {

    }

    private void Update()
    {
        UpdateRoomQueue();

        if (Input.GetKeyDown(KeyCode.K))
        {
            CurrentRoom.KillAllEnemies();
        }
    }

    void UpdateRoomQueue()
    {
        if (isLoadingRoom || RoomQueue.Count == 0)
        {
            return;
        }

        CurrentRoomData = RoomQueue.Dequeue();
        isLoadingRoom = true;

        Vector2Int vec = new Vector2Int((int)CurrentRoomData.Position.x, (int)CurrentRoomData.Position.y);

        if (MapGen.TileGrid.CheckIfOccuppied(vec))
        {
            isLoadingRoom = false;
            return;
        }   
        if (CurrentRoomData.roomProperty != RoomProperty.Start)
        {


        }
            
        StartCoroutine(LoadRoomRoutine(CurrentRoomData));

    }

    IEnumerator LoadRoomRoutine(GridData Info)
    {
        string RoomName = WorldName + Info.RoomName;

        //Debug.Log("Room Name: " + Info.RoomName);

        //Make scenes overlap with LoadSceneMode.Additive
        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(RoomName, LoadSceneMode.Additive);

        while (loadRoom.isDone == false)
        {
            yield return null;
        }
    }

    public void RegisterRoom(Room room)
    {


        Vector2Int vec = new Vector2Int((int)CurrentRoomData.Position.x, (int)CurrentRoomData.Position.y);
        if (!(room.Property == RoomProperty.Start))
        {
            if (IsSquareOccuppied(vec))
            {
                
                Debug.Log("Deleting OCCUPPIED room! at " + CurrentRoomData.Position);
                Destroy(room);
                isLoadingRoom = false;
                return;


            }
        }
        

        room.roomInfo = CurrentRoomData;


        //Debug.Log(CurrentRoomData.Position);

        MapGen.TileGrid.SetOccuppied(vec);
        room.transform.position = new Vector3(CurrentRoomData.Position.x * RoomWidth, CurrentRoomData.Position.y * RoomHeight, 0);



        room.roomPos.x = CurrentRoomData.Position.x;
        room.roomPos.y = CurrentRoomData.Position.y;
        room.name = WorldName + "-" + CurrentRoomData.RoomName + " " + room.roomPos.x + ", " + room.roomPos.y;
        room.transform.parent = transform;

        isLoadingRoom = false;

        if (LoadedRooms.Count == 0)
        {
            CameraController.instance.CurrentRoom = room;
        }


        LoadedRooms.Add(room);
        //room.RemoveUnconnectedDoors();
        //Debug.Log("I was added to the LoadedRooms!");
        GameManager.instance.IncrementNumRooms();
    }

    public void RemoveUnconnectedDoors()
    {
        //These have to be done sequentially, otherwise you might check for a nearby room whose doors haven't been set yet
        for (int i = 0; i < LoadedRooms.Count; i++)
        {
            LoadedRooms[i].CheckForNearbyRooms();    
        }

        for (int i = 0; i < LoadedRooms.Count; i++)
        {
            LoadedRooms[i].SetDoorConnections();
        }

    }

    public void SetKeyRoom()
    {
        int i = 0;
        int rand = 0;
        Debug.Log("Map rooms count = " + LoadedRooms.Count);
        while (true)
        {
            rand = Random.Range(0, LoadedRooms.Count);
            if (LoadedRooms[rand].Property == RoomProperty.Combat)
            {
                LoadedRooms[rand].SetIsKeyRoom();
                Debug.Log("--------Found a proper room!------");

                return;
            }
            i++;

        }
    }

    public bool DoesTileExist(Vector2Int tile)
    {

        return MapGen.TileGrid.CheckIfRoomIsNotOccuppied(tile);
    }


    public void FindDoor(Door door)
    {
        for (int i = 0; i < LoadedRooms.Count; i++)
        {
            if (LoadedRooms[i].HasTile(door))
            {                  
                break;
            }
        }
    }

    public bool DoesRoomExist(Vector2 roomPos)
    {
        return LoadedRooms.Find( item => item.roomPos.x == roomPos.x && item.roomPos.y == roomPos.y ) != null;
    }

    public bool IsSquareOccuppied(Vector2Int pos)
    {
        return MapGen.IsGridSquareOccuppied(pos);
    }

    public void OnEnterRoom(Room room)
    {
        CameraController.instance.OnPlayerEnterRoom(room);
        CurrentRoom = room;
    }

    public bool CheckForTile(Vector2Int pos)
    {
        return MapGen.DoesTileExist(pos);
    }

    public bool CheckForTileAbove(Vector2Int tile)
    {
        return MapGen.TileGrid.CheckAbove(tile);
    }

    public bool CheckForTileBelow(Vector2Int tile)
    {
        return MapGen.TileGrid.CheckBelow(tile);
    }

    public bool CheckForTileLeft(Vector2Int tile)
    {
        return MapGen.TileGrid.CheckAbove(tile);
    }

    public bool CheckForTileRight(Vector2Int tile)
    {
        return MapGen.TileGrid.CheckBelow(tile);
    }


}
