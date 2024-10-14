using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    //if true static on center of room, if false center on player
    public bool CenterOnRoom = true;
    public Room CurrentRoom;
    public float RoomChangeSpeed;

    public bool ShiftingToCenter = false;

    public Camera MainCamera;

    public Transform FocusTarget;
    public float SmoothTime;

    public float MinPlayerDistance;

    Vector3 Offset = new Vector3(0, 0, -10f);
    Vector3 Velocity = Vector3.zero;

    public Bounds CameraBounds;
    public Vector3 TargetPos;

    float CamMinX, CamMaxX, CamMinY, CamMaxY;

    private void Awake()
    {
        MainCamera = GetComponent<Camera>();

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
    }

    private void Update()
    {



        if (ShiftingToCenter)
        {
            UpdatePosition();
        }

        
    }

    public void LateUpdate()
    {
        if (CurrentRoom != null) {
            if (CurrentRoom.MultiRoom)
            {

                //center and calculate position based on players position

                //Debug.Log("In Multi Room!");
                CameraBounds.center = FocusTarget.position;
                //TargetPos = Vector3.Lerp(transform.position, FocusTarget.position - Offset, Time.deltaTime * SmoothTime);
                Vector3 target = FocusTarget.position + Offset;

                TargetPos = Vector3.SmoothDamp(transform.position, target, ref Velocity, SmoothTime);


                if (!ShiftingToCenter)
                {
                    TargetPos = GetCameraBounds();
                }

                


                transform.position = TargetPos;
            }
        }

    }

    void UpdatePosition()
    {
        if (CurrentRoom == null)
        {
            return;
        }

        Vector3 targetPos = GetRoomCenterPosition();

        transform.position = Vector3.MoveTowards(transform.position, targetPos, RoomChangeSpeed * Time.deltaTime);

        if (!IsSwitchingScene())
        {
            ShiftingToCenter = false;
            if (CurrentRoom.Type != RoomType.Normal)
            {
                SetCameraBounds();
                Debug.Log("Setting Camera Bounds!");
            }
        }
    }

    public void SetCameraBounds()
    {
        float height = MainCamera.orthographicSize;
        float width = height * MainCamera.aspect;

        Bounds RoomBounds = CurrentRoom.GetRoomBounds();



        CamMinX = CurrentRoom.RoomMinX + width;
        CamMaxX = CurrentRoom.RoomMaxX - width;
        CamMinY = CurrentRoom.RoomMinY + height;
        CamMaxY = CurrentRoom.RoomMaxY - height;

        //float newX = Mathf.Clamp(TargetPos.x, minX, maxX);

        CameraBounds = new Bounds();
        //CameraBounds.center = FocusTarget.position;
        CameraBounds.SetMinMax(new Vector3(CamMinX, CamMinY, 0), new Vector3(CamMaxX, CamMaxY, 0));
    }

    public Vector3 GetCameraBounds()
    {
        float x = Mathf.Clamp(TargetPos.x, CamMinX, CamMaxX);
        float y = Mathf.Clamp(TargetPos.y, CamMinY, CamMaxY);

        return new Vector3(x, y, transform.position.z);
    }

    Vector3 GetRoomCenterPosition()
    {
        if (CurrentRoom == null)
        {
            return Vector3.zero;
        }


        Vector3 targetPos = Vector3.zero;

        if (CurrentRoom.Type == RoomType.Normal)
        {
            targetPos = CurrentRoom.GetRoomCenter();
        }
        else
        {
            targetPos = Player.Instance.transform.position;
        }
        targetPos.z = transform.position.z;

        return targetPos;
    }


    public bool IsSwitchingScene()
    {

        if (CurrentRoom.Type == RoomType.Normal)
        {
            return transform.position.Equals(GetRoomCenterPosition()) == false;
        }

  

        float Dist = Vector2.Distance(transform.position, Player.Instance.transform.position);

        Debug.Log("Distance to player: " + Dist);
        if (Dist < MinPlayerDistance)
        {
            return false;
        }

        return true;
    }

    public void OnPlayerEnterRoom(Room room)
    {
        
        CurrentRoom = room;
        ShiftingToCenter = true;
    }

}
