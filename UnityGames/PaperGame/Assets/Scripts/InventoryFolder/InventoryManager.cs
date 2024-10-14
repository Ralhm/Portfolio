using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{


    public UseItem useItemButton;
    public List<UseItem> itemButtons;
    public BattleUI ui;



    // Start is called before the first frame update
    void Awake()
    {
        
        ui = GameObject.Find("BattleMenu").GetComponent<BattleUI>();



    }

    void OnEnable()
    {
        print("Enabling Item Menu");
        for (int i = 0; i < ui.slots.Count; i++)
        {
            itemButtons.Add(Instantiate(useItemButton, this.transform));
            itemButtons[i].index = i;
            itemButtons[i].ItemName = ui.slots[i].itemName;
            itemButtons[i].ItemDescription = ui.slots[i].description;
            ui.slots[i].index = i;
        }


    }

    void OnDisable()
    {
        for (int i = 0; i < itemButtons.Count; i++)
        {
            Destroy(itemButtons[i].gameObject);
        }
        itemButtons.Clear();
    }


}
