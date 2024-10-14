using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour
{
    
    public BattleUI ui;



    public Button SkillDisplayButton;
    public Button AttackButton;
    public Button ItemDisplayButton;

    public List<Button> skillButtons;
    public List<Button> itemButtons;

    void Start()
    {
        ui = GameObject.Find("BattleMenu").GetComponent<BattleUI>();
    }

    public void GenerateButtons(int i)
    {


        /*
        //print(i);
        skillButtons.Add(Instantiate(SkillDisplayButton, this.transform));
        PlayerButtons newSkillButton = skillButtons[i].GetComponent<PlayerButtons>();
        print(ui.party[i].index);
        newSkillButton.RT.position = ui.cam.WorldToScreenPoint(ui.party[i].transform.position + new Vector3(-2F, 0, 0));
        newSkillButton.index = (i); //Set the buttons index value, tying it to its corresponding character
        newSkillButton.gameObject.SetActive(false); //Set all player buttons as false at beginning


        itemButtons.Add(Instantiate(ItemDisplayButton, this.transform));
        PlayerButtons newItemButton = itemButtons[i].GetComponent<PlayerButtons>();
        newItemButton.RT.position = ui.cam.WorldToScreenPoint(ui.party[i].transform.position + new Vector3(2F, 0, 0));
        newItemButton.index = (i);
        newItemButton.gameObject.SetActive(false);
        */
    }
    

}
