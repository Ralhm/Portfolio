using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Left, Right, Top, Bottom, None
        
    };

    public GameObject LockSymbol;
    public Room CurrentRoom;
    Room NeighbourRoom;

    public float traceDistance = 2.0f;
    float drawDebugTime = 40;

    public bool HasRoom = false;

    public GameObject ClosedDoor;
    public GameObject OpenedDoor;



    public SpriteRenderer sprite;
    public bool Connected = true;
    public DoorType MainType;
    public DoorType SecondaryType = DoorType.None;

    public Transform SpawnPosition;
    public Vector2Int OccuppiedTile;
    public Vector2Int NextTile;

    public Door ConnectedDoor;
    public LayerMask DoorMask;

    public bool isBossDoor;

    public bool isLocked;

    public void Start()
    {
        CurrentRoom = GetComponentInParent<Room>();
        //CheckForRoom();
    }


    //This whole thing is kinda nasty BUT it allows for more easy room size adjustments in the future and prevents fuckiness with trace method
    //The combined Purpose of the three below functions are to see if there is a room that this door connects to.
    //If there is, store it.
    //By setting the occuppied and next tile here, we can then allocate the corresponding doors
    public void CheckForRoom(GridData grid, RoomType type)
    {


        if (type == RoomType.Normal)
        {
            OccuppiedTile = grid.GetNormalTile();
        }
        else if (type == RoomType.Tall)
        {
            switch (SecondaryType)
            {
                case DoorType.Top:
                    OccuppiedTile = grid.GetTopTile();
                    break;
                case DoorType.Bottom:
                    OccuppiedTile = grid.GetBottomTile();
                    break;
            }
        }
        else if (type == RoomType.Long)
        {
            switch (SecondaryType)
            {
                case DoorType.Left:
                    OccuppiedTile = grid.GetLeftTile();
                    break;
                case DoorType.Right:
                    OccuppiedTile = grid.GetRightTile();
                    break;
            }
        }
        else //Big Room
        {
            if ((MainType == DoorType.Left && SecondaryType == DoorType.Top) || (MainType == DoorType.Top && SecondaryType == DoorType.Left))
            {
                OccuppiedTile = grid.GetTopLeftTile();
            }

            if ((MainType == DoorType.Right && SecondaryType == DoorType.Top) || (MainType == DoorType.Top && SecondaryType == DoorType.Right))
            {
                OccuppiedTile = grid.GetTopRightTile();
            }
            if ((MainType == DoorType.Right && SecondaryType == DoorType.Bottom) || (MainType == DoorType.Bottom && SecondaryType == DoorType.Right))
            {
                OccuppiedTile = grid.GetBottomRightTile();
            }

            if ((MainType == DoorType.Left && SecondaryType == DoorType.Bottom) || (MainType == DoorType.Bottom && SecondaryType == DoorType.Left))
            {
                OccuppiedTile = grid.GetBottomLeftTile();
            }
        }
        FindNextRoom();


    }

    public void FindNextRoom()
    {
        //Set the tile belonging to the neighbor door
        NextTile = OccuppiedTile;
        switch (MainType)
        {
            case DoorType.Left:
                NextTile += new Vector2Int(-1, 0);             
                break;
            case DoorType.Right:
                NextTile += new Vector2Int(1, 0);
                break;
            case DoorType.Top:
                NextTile += new Vector2Int(0, 1);
                break;
            case DoorType.Bottom:
                NextTile += new Vector2Int(0, -1);
                break;
        }

        //If this door has already had it's corresponding connection set
        if (ConnectedDoor != null)
        {
            return;
        }

        if (!MapController.instance.DoesTileExist(NextTile))
        {

            Room room = MapController.instance.RoomMap[NextTile];
            room.HasTile(this);
            Connected = true;
            ClosedDoor.SetActive(false);
        }
        else
        {
            CloseConnection();

        }
        return;
    }


    public void CloseConnection()
    {
        Connected = false;
        OpenedDoor.SetActive(false);
        ClosedDoor.GetComponent<TilemapRenderer>().enabled = false;

    }

    public void CloseDoor()
    {
        ClosedDoor.SetActive(true);
        OpenedDoor.SetActive(false);
        //DoorCollider.enabled = true;
    }

    public void OpenDoor()
    {

        if (isLocked)
        {
            return;
        }

        if (Connected)
        {

            //DoorCollider.enabled = false;
            OpenedDoor.SetActive(true);
            ClosedDoor.SetActive(false);
        }

    }

    public void SetBossDoor()
    {
        isBossDoor = true;
        isLocked = true;
        CloseDoor();

        LockSymbol.gameObject.SetActive(true);

    }

    public void UnlockDoor()
    {
        isLocked = false;
        LockSymbol.gameObject.SetActive(false);
        OpenDoor();
    }

    public void UnlockRoom()
    {
        ConnectedDoor.CurrentRoom.UnlockDoors();
    }


    public void TeleportPlayer(GameObject Player)
    {
        Player.transform.position = SpawnPosition.position;
    }

    public void EnterDoor()
    {
        if (!CurrentRoom.ClosedDoors)
        {
            CurrentRoom.OnEnterRoom();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Door Triggered: " + collision.gameObject.name);

        if (collision.gameObject.layer == 6)
        {

            if (isLocked)
            {
                
                Player player = collision.gameObject.GetComponent<Player>();
                if (player.GetKey())
                {
                    player.SetHasKey(false);
                    UnlockRoom();
                }
                else
                {
                    Debug.Log("Player has no key!");
                }
            }

            ConnectedDoor.EnterDoor();
            ConnectedDoor.TeleportPlayer(collision.gameObject);

        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("Collided!");
        }
        
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(SpawnPosition.position, 0.2f);
        
    }
#endif

}
