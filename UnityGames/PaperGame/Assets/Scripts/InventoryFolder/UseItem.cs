using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UseItem : MonoBehaviour
{
    public RectTransform RT;
    public InventoryManager manager;
    public BattleUI ui;
    public int index;
    public string ItemName;
    public string ItemDescription;
    public TextMeshProUGUI ItemText;
    public TextMeshProUGUI ItemDescriptionDisplay;


    

    // Start is called before the first frame update
    void Start()
    {
        ui = GameObject.Find("BattleMenu").GetComponent<BattleUI>();
        ItemDescriptionDisplay = GameObject.Find("ItemDescription").GetComponent<TextMeshProUGUI>();
        ItemText.SetText(ItemName);




    }

    public void Enter()
    {
        ItemDescriptionDisplay.SetText(ItemDescription);
    }

    public void useItem()
    {
        ui.AudioJungle.Play("Select");
        ui.queuedItem = ui.slots[index];
        ui.queuedItem.index = index;
        ui.state = BattleUI.State.SelectingAllyItem;
        ui.DisplayPartyMemberOptions();
    }

}
