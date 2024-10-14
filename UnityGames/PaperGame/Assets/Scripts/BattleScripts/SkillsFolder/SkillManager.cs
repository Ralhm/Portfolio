using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //Script for displaying skill buttons

    public UseSkill useSkillButton;

    public List<UseSkill> SkillButtons;
    public BattleUI ui;


    void Awake()
    {
        ui = GameObject.Find("BattleMenu").GetComponent<BattleUI>();
    }

    //Destroy/Instantiate buttons as necessary

    
    void OnEnable()
    {
        //print("Enabling Skill Menu");

        //print(ui.party[ui.partyTurn].skills.Count);
        for (int i = 0; i < ui.party[ui.partyTurn].skills.Count; i++)
        {
            //print(i);
            SkillButtons.Add(Instantiate(useSkillButton, this.transform));
            SkillButtons[i].index = i;
            SkillButtons[i].SkillName = ui.party[ui.partyTurn].skills[i].SkillName;
            SkillButtons[i].SkillDescription = ui.party[ui.partyTurn].skills[i].SkillDescription;

            if (ui.party[ui.partyTurn].skills[i].action == Skills.ActionType.Timing)
            {
                SkillButtons[i].SkillTutorial = ui.party[ui.partyTurn].skills[i].TimingTutorial;
            }
            else if (ui.party[ui.partyTurn].skills[i].action == Skills.ActionType.Mash)
            {
                SkillButtons[i].SkillTutorial = ui.party[ui.partyTurn].skills[i].MashTutorial;
            }
            else if (ui.party[ui.partyTurn].skills[i].action == Skills.ActionType.Sequence)
            {
                SkillButtons[i].SkillTutorial = ui.party[ui.partyTurn].skills[i].SequenceTutorial;
            }
            else if (ui.party[ui.partyTurn].skills[i].action == Skills.ActionType.Aim)
            {
                SkillButtons[i].SkillTutorial = ui.party[ui.partyTurn].skills[i].AimTutorial;
            }
            else
            {
                SkillButtons[i].SkillTutorial = "Automatically activates";
            }

            
        }




    }

    void OnDisable()
    {
        for (int i = 0; i < SkillButtons.Count; i++)
        {
            Destroy(SkillButtons[i].gameObject);

            
        }
        SkillButtons.Clear();
    }
    
    
}

