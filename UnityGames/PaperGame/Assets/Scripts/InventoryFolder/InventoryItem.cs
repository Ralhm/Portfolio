using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InventoryItem : MonoBehaviour
{

    public int value;
    public int index;
    public int ItemID;
    public string itemName;
    public string description;
    [SerializeField] public Sprite icon;


    public virtual void Execute(Player player)
    {
        print("Executing default item");
    }




    /*
    public void OnBeforeSerialize()
    {

    }
    public void OnAfterDeserialize()
    {

    }

    */
}
