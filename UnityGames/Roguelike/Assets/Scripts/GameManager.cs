using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;


    public int ClearedRooms;
    public int NumRooms;

    public SaveSystem Saver;
    public Pooler Pool;

    public Key KeyRef;

    int Minutes;
    float Seconds;

    float RoomTimer;
    bool RoomTimerActive;

    // Start is called before the first frame update
    void Awake()
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


    public void StartRoomTimer()
    {
        RoomTimer = 0;
        RoomTimerActive = true;
    }

    public void StopRoomTimer()
    {
        RoomTimerActive = false;
    }

    public int GetRoomTimer()
    {
        return (int)RoomTimer;
    }

    public void IncrementClearedRooms()
    {
        ClearedRooms++;
        if (ClearedRooms == NumRooms - 1)
        {
            
        }
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            LoadNextLevel();
        }
    }

    public void FixedUpdate()
    {
        if (RoomTimerActive)
        {
            RoomTimer += Time.deltaTime;
        }
        SetTime(Time.deltaTime);
    }

    public void SetTime(float dt)
    {
        Seconds += dt;
        if (Seconds >= 60.0f)
        {
            Minutes += 1;
            Seconds -= 60.0f;
        }
        UI.Instance.SetTimer(Minutes, (int)Seconds);
    }

    public int GetMinutes()
    {
        return Minutes;
    }

    public int GetSeconds()
    {
        return (int)Seconds;
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


    }

    public void SavePlayer(PlayerData data)
    {
        Saver.SavePlayerData(data);
    }


    public PlayerData LoadPlayer()
    {
        return Saver.LoadPlayerData();
    }


    public void IncrementNumRooms()
    {
        NumRooms++;
    }

    public void SpawnKey(Transform transform)
    {
        Instantiate(KeyRef, transform.position, transform.rotation);
    }

    public void SpawnMoney(Transform transform)
    {
        Pool.GetMoneyInPool(transform);
    }

    public void SpawnCrystal(Transform transform)
    {
        Pool.GetCrystalsInPool(transform);
    }

    public void SpawnHealth(Transform transform)
    {
        Pool.GetHealthInPool(transform);
    }

    public void SpawnDamageNumber(Transform transform, int damage)
    {
        Pool.GetDamageInPool(transform, damage);
    }
}
