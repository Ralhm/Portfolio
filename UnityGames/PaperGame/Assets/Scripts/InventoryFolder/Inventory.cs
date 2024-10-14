using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{

    public int[] ItemID;
    public string[] itemName;
    public string[] description;
    public List<Sprite> icon;


    public Inventory(List<InventoryItem> items)
    {
        ItemID = new int[items.Count];
        itemName = new string[items.Count];
        description = new string[items.Count];


        for (int i = 0; i < items.Count; i++)
        {
            ItemID[i] = items[i].ItemID;
            itemName[i] = items[i].itemName;
            description[i] = items[i].description;

        }
    }



}
