using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UseSkill : MonoBehaviour
{
    //Script for attaching to skill buttons, registers which skill will be used when button is selected

    public RectTransform RT;
    public int index;
    public string SkillName;
    public string SkillDescription;
    public string SkillTutorial;
    public TextMeshProUGUI SkillText;
    public TextMeshProUGUI SkillDescrptionDisplay;
    public TextMeshProUGUI SkillTutorialDisplay;
    //public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        SkillText.SetText(SkillName);
        SkillDescrptionDisplay = GameObject.Find("SkillDescription").GetComponent<TextMeshProUGUI>();
        SkillTutorialDisplay = GameObject.Find("SkillTutorial").GetComponent<TextMeshProUGUI>();

    }

    public void Enter()
    {
        //animator.StartPlayback();
        SkillDescrptionDisplay.SetText(SkillDescription);
        SkillTutorialDisplay.SetText(SkillTutorial);

    }

    public void Exit()
    {


    }

    
    

    //Skill List 
    //Set the skill enum state to match whichever skill is being used so that BattleUI.Attack can know which skill is being used
    //Attach these functions to the skill list buttons
    public void useSkill()
    {
        BattleUI.UIsingleton.AudioJungle.Play("Select");
        BattleUI.UIsingleton.party[BattleUI.UIsingleton.partyTurn].queuedSkill = BattleUI.UIsingleton.party[BattleUI.UIsingleton.partyTurn].skills[index];
        if (BattleUI.UIsingleton.party[BattleUI.UIsingleton.partyTurn].SP >= BattleUI.UIsingleton.party[BattleUI.UIsingleton.partyTurn].queuedSkill.cost)
        {         
            if (BattleUI.UIsingleton.party[BattleUI.UIsingleton.partyTurn].queuedSkill.property == Skills.Property.Party) //If it's a skill meant to be used on the party
            {
                BattleUI.UIsingleton.state = BattleUI.State.SelectingAllySkill;
                BattleUI.UIsingleton.DisplayPartyMemberOptions();
            }
            else //If it's a skill meant to be used on an enemy
            {
                BattleUI.UIsingleton.DisplayTargets();
            }
            
        }
        else
        {
            print("You don't have enough SP!");
        }

    }



}
