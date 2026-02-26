using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System;

public class SaveSystem : MonoBehaviour 
{

    string json;

    public string PlayerData = "Player.dat";

    public void SavePlayerData(PlayerData data)
    {
        Debug.Log("Saving Player Data!");
        json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + PlayerData, EncryptedString(json));



    }

    public PlayerData LoadPlayerData()
    {
        Debug.Log("Loading Player Data!");
        json = File.ReadAllText(Application.persistentDataPath + PlayerData);
        return JsonUtility.FromJson<PlayerData>(DecryptedString(json));

    }


    string EncryptedString(string input)
    {
        byte[] binaryData = Encoding.UTF8.GetBytes(input);
        string encryptedString = Convert.ToBase64String(binaryData);

        return encryptedString;
    }


    string DecryptedString(string input)
    {
        byte[] binaryData = Convert.FromBase64String(input);
        string decryptedString = Encoding.UTF8.GetString(binaryData);

        return decryptedString;
    }

}
