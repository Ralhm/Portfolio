using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class PortraitManager : MonoBehaviour
{

    //Prefab storage
    public List<Portrait> portraits;
    public RectTransform RT;
    public DialogueRunner Runner;
    public Portrait[] characterPortraits;


    public int currentSpeaker = 0;

    public Camera cam;

    public List<GameObject> positions; //Let 0 be right, 1 be left, and 2 be middle


    public void SetIdle() //This should set the current portrait to its idle state, just have it do an animation trigger
    {
        print("Setting animation state to idle");
        for (int i = 0; i < characterPortraits.Length; i++)
        {
            if (characterPortraits[i] != null)
            {
                characterPortraits[i].animator.SetBool("Idle", true);
                characterPortraits[i].animator.SetBool(characterPortraits[i].currentState, false);
            }
        }
        
    }

    public void SetSpeaker(string[] value) //For when you just want to switch speakers without calling SetAnimationState. Take in a string, convert to ints
    {
        currentSpeaker = int.Parse(value[0]);
    }

    public void SetTalk() //Call on this when you want someone to continue talking in the same manner as before without calling SetAnimationState
    {
        characterPortraits[currentSpeaker].animator.SetBool("Idle", false);
        characterPortraits[currentSpeaker].animator.SetBool(characterPortraits[currentSpeaker].currentState, true);
    }

    public void RemovePortrait(string[] value) //Yarn Version
    {
        int index = int.Parse(value[0]);
        //characterPortraits[index].animator.SetTrigger("FadeOut");
        Destroy(characterPortraits[index].gameObject);
    }

    public void RemoveAllPortraits() //Remove all portraits when dialogue ends
    {
        for (int i = 0; i < characterPortraits.Length; i++)
        {
            if (characterPortraits[i] != null)
            {
                print("removing Portrait");
                Destroy(characterPortraits[i].gameObject);

            }
        }
    }


    //This should loop through your prefabs and decide which one to generate based on name. 
    //This will be called within Yarn Files
    //Let 0 be name, 1 be the desired animation state, and 2 be side
    //Let side represent which portrait slot will be filled, 0, 1, or 2
    //Check if you're either making a new portrait, or simply changing the state on a pre-existing portrait
    public void SetAnimationState(string[] values) 
    {
        currentSpeaker = int.Parse(values[2]);
        for (int i = 0; i < portraits.Count; i++)
        {
            //print(portraits[i].name);
            //print(values[0]);
            if (values[0] == portraits[i].name)
            {
                if (characterPortraits[currentSpeaker] != null)//only make a new portrait if there is none in the given spot
                {
                    Destroy(characterPortraits[currentSpeaker].gameObject);
                }
                if (characterPortraits[currentSpeaker] != portraits[i])
                {
                    characterPortraits[currentSpeaker] = (Instantiate(portraits[i], cam.transform, true));
                    characterPortraits[currentSpeaker].transform.localPosition = positions[currentSpeaker].transform.localPosition;
                }



                //characterPortraits[currentSpeaker].transform.position = cam.ScreenToWorldPoint(Positions[currentSpeaker]);
                //characterPortraits[currentSpeaker].transform.localPosition = Positions[currentSpeaker];

                characterPortraits[currentSpeaker].animator.SetBool(values[1], true);
                characterPortraits[currentSpeaker].currentState = values[1];

            }

            
        }
        
    }
}
