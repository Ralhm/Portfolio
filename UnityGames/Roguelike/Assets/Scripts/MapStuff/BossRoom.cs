using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : Room
{

    public Exit RoomExit;



    public override void OnRoomCleared()
    {
        base.OnRoomCleared();
        RoomExit.gameObject.SetActive(true);
    }

    public override void SpawnCrystals()
    {
        int ElapsedTime = (GameManager.instance.GetMinutes() * 60) + GameManager.instance.GetSeconds();

        ElapsedTime -= 60;


        int amount = BaseCrystalAmount - (ElapsedTime / CrystalDecreaseRate);
        crystalSpawnPoint.SpawnCrystal(amount);
        Debug.Log("Amount of Crystals to spawn: " + amount);

    }

    public void AddExit(Exit exit)
    {
        RoomExit = exit;
    }
}
