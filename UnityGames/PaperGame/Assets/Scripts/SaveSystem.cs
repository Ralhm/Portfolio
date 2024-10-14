using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    public static void SavePlayer(List<Player> players)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.BattlePlayer";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(players);


        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.BattlePlayer";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save File not Found in " + path);
            return null;
        }
    }
    
    public static void SaveInventory(List<InventoryItem> inventory)
    {
        Debug.Log("Saving Inventory");
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.Inventory";
        FileStream stream = new FileStream(path, FileMode.Create);

        Inventory data = new Inventory(inventory);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static Inventory LoadInventory()
    {
        Debug.Log("Loading Inventory");
        string path = Application.persistentDataPath + "/player.Inventory";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Inventory data = formatter.Deserialize(stream) as Inventory;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save File not Found in " + path);
            return null;
        }
    }
    
}
