using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Normal, Tall, Long, Big
}

public enum RoomProperty
{
    Combat, Boss, Shop, Treasure, Hidden, MagicShop, Start
}

public class Room : MonoBehaviour
{
    public List<Room> AdjacentRooms;

    public GridData roomInfo;

    public Collider2D roomCollider;

    public GameObject KeySymbol;

    public ItemSpawnPoint itemPoint;

    protected CrystalSpawnPoint crystalSpawnPoint;

    public RoomType Type;
    public RoomProperty Property;

    public bool CombatRoom = true;
    public bool ClearedRoom;
    public bool ClosedDoors = false;

    //if true static on center of room, if false center on player
    public bool CenterOnRoom = true;
    public int Width;
    public int Height;
    public Vector2 roomPos;
    public bool MultiRoom = false;

    public bool IsKeyRoom;

    public int BaseCrystalAmount;
    public int CrystalDecreaseRate = 20;

    public List<Door> Doors;

    public List<Enemy> Enemies;

    public List<EnemySpawnPoint> EnemySpawnPoints;

    public int NumEnemies = 0;

    public GameObject VoidMask;

    public float RoomMinX, RoomMaxX, RoomMinY, RoomMaxY;

    // Start is called before the first frame update
    void Start()
    {

        if (Property != RoomProperty.Combat && Property != RoomProperty.Boss)
        {
            ClearedRoom = true;
        }


        if (MapController.instance == null)
        {
            Debug.Log("Map controller is Null!");
            return;
        }

        MapController.instance.RegisterRoom(this);
        if (roomCollider == null)
        {
            roomCollider = GetComponent<Collider2D>();
        }
        

        Bounds roomBounds = GetRoomBounds();

        if (MultiRoom)
        {
            RoomMinX = roomCollider.transform.position.x + roomCollider.offset.x - roomBounds.size.x / 2.0f;
            RoomMaxX = roomCollider.transform.position.x + roomCollider.offset.x + roomBounds.size.x / 2.0f;

            RoomMinY = roomCollider.transform.position.y + roomCollider.offset.y - roomBounds.size.y / 2.0f;
            RoomMaxY = roomCollider.transform.position.y + roomCollider.offset.y + roomBounds.size.y / 2.0f;
        }
        if (Property != RoomProperty.Start)
        {
            if (MapController.instance.UseVoidMask)
            {
                VoidMask.SetActive(true);
            }
            
        }
        


    }



    public Vector3 GetRoomCenter()
    {
        return new Vector3(roomPos.x * MapController.instance.RoomWidth, roomPos.y * MapController.instance.RoomHeight);
    }

    public void CheckForNearbyRooms()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].CheckForRoom(roomInfo, Type);
            Doors[i].CheckIfConnected();
        }

    }

    public void SetDoorConnections()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            if (Doors[i].Connected)
            {
                MapController.instance.FindDoor(Doors[i]);
            }
        }
    }

    //Check if the room contains the matching tile
    //if it does, find the corresponding door and set it
    public bool HasTile(Door door)
    {

        for (int i = 0; i < Doors.Count; i++)
        {
            if (door.NextTile == Doors[i].OccuppiedTile)
            {
                if (CheckConnectedDoor(door, Doors[i]))
                {
                    SetConnectedDoor(door, Doors[i]);
                    return true;
                }


            }
        }


        return false;
    }


    public bool CheckConnectedDoor(Door inputDoor, Door roomDoor)
    {
        switch (inputDoor.MainType)
        {
            case Door.DoorType.Left:
                if (roomDoor.MainType == Door.DoorType.Right)
                {
                    return true;
                }
                break;
            case Door.DoorType.Right:
                if (roomDoor.MainType == Door.DoorType.Left)
                {
                    return true;
                }
                break;
            case Door.DoorType.Top:
                if (roomDoor.MainType == Door.DoorType.Bottom)
                {
                    return true;
                }
                break;
            case Door.DoorType.Bottom:
                if (roomDoor.MainType == Door.DoorType.Top)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    public void SetConnectedDoor(Door inputDoor, Door roomDoor)
    {
        inputDoor.ConnectedDoor = roomDoor;
        roomDoor.ConnectedDoor = inputDoor;

        if (inputDoor.CurrentRoom.Property == RoomProperty.Boss)
        {
            roomDoor.SetBossDoor();
        }
    }

    public Bounds GetRoomBounds()
    {
        return roomCollider.bounds;
    }

    public void SpawnEnemyAtLocation(Vector3 location, Enemy enemy)
    {

        Enemy newEnemy = Instantiate(enemy, location, transform.rotation);
        newEnemy.SetRoom(this);
        Enemies.Add(newEnemy);
        NumEnemies++;
    }

    public void DecreaseEnemies()
    {
        NumEnemies--;
        if (NumEnemies == 0)
        {
            OnRoomCleared();
        }
    }

    public void SetIsKeyRoom()
    {
        IsKeyRoom = true;
        KeySymbol.gameObject.SetActive(true);
    }

    public virtual void OnRoomCleared()
    {
        ClearedRoom = true;
        Enemies.Clear();
        OpenDoors();
        GameManager.instance.IncrementClearedRooms();
        SpawnCrystals();

        if (IsKeyRoom)
        {
            GameManager.instance.SpawnKey(crystalSpawnPoint.gameObject.transform);
            Debug.Log("This is a key room!");
        }
        else
        {
            Debug.Log("This is not a key room!");
        }
    }

    public void OnEnterRoom()
    {
        MapController.instance.OnEnterRoom(this);



        if (Property == RoomProperty.Shop || Property == RoomProperty.Start || Property == RoomProperty.Treasure || Property == RoomProperty.Hidden || Property == RoomProperty.MagicShop)
        {
            Debug.Log("ENTERING SHOP OR STARTING ROOM");
            return;
        }

        if (CombatRoom && !ClearedRoom)
        {
            CloseDoors();
            for (int i = 0; i < EnemySpawnPoints.Count; i++)
            {
                EnemySpawnPoints[i].SpawnEnemy();
            }

            GameManager.instance.StartRoomTimer();

        }
        if (MapController.instance.UseVoidMask)
        {
            VoidMask.SetActive(false);
        }
    }

    //in case the room has multiple neighbors, you need to unlock all of them
    public void UnlockDoors()
    {
        for (int i = 0; i < Doors.Count; i++)
        {
            if (Doors[i].ConnectedDoor != null)
            {
                if (Doors[i].ConnectedDoor.isLocked)
                {
                    Doors[i].ConnectedDoor.UnlockDoor();
                }
            }

        }
    }

    public void CloseDoors()
    {
        ClosedDoors = true;
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].CloseDoor();
        }
    }

    public void OpenDoors()
    {
        ClosedDoors = false;
        for (int i = 0; i < Doors.Count; i++)
        {
            Doors[i].OpenDoor();
        }
    }

    public void AddSpawnPoint(EnemySpawnPoint point)
    {
        EnemySpawnPoints.Add(point);
    }

    public void AddItemSpawnPoint(ItemSpawnPoint point)
    {
        itemPoint = point;
    }

    public void AddCrystalSpawnPoint(CrystalSpawnPoint point)
    {
        crystalSpawnPoint = point;
    }

    public virtual void SpawnCrystals()
    {
        int ElapsedTime = GameManager.instance.GetRoomTimer();
        int amount = BaseCrystalAmount - (ElapsedTime / CrystalDecreaseRate);
        crystalSpawnPoint.SpawnCrystal(amount);
        Debug.Log("Amount of Crystals to spawn: " + amount);
    }

    //Debug function
    public void KillAllEnemies()
    {
        for (int i = 0;i < Enemies.Count; i++)
        {
            Enemies[i].OnDeath();
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height, 0));
    }
#endif

}
