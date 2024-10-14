using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerButtons : MonoBehaviour
{
    public RectTransform RT;
    public int index;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Enter()
    {
        

    }

    public void Exit()
    {
        

    }

    public void Click()
    {
        BattleUI.UIsingleton.AudioJungle.Play("Select");
        BattleUI.UIsingleton.DisplayUserSkills(); 

    }

    public void PartyAction()
    {
        
        if (BattleUI.UIsingleton.state == BattleUI.State.SelectingAllyItem)
        {
            BattleUI.UIsingleton.UseItem(index);
        }
        else if (BattleUI.UIsingleton.state == BattleUI.State.SelectingAllySkill)
        {
            BattleUI.UIsingleton.UsePartySkill(index);
        }
        
    }


    public void DisplayItems()
    {
        BattleUI.UIsingleton.AudioJungle.Play("Select");
        BattleUI.UIsingleton.DisplayUserItems();
    }


}
