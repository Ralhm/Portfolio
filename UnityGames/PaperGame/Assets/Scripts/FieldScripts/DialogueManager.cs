using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Yarn.Unity;

//Should be able to adjust speed of dialogue
//Add Character Portraits to dialogue


public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public DialogueRunner Runner;
    public DialogueUI UI;
    public InMemoryVariableStorage yarnVariables;

    public Animator animator;
    public PortraitManager portraitManager;


    public Queue<string> sentences;

    // Start is called before the first frame update
    void Awake()
    {
        Runner.AddCommandHandler("SetAnimation", portraitManager.SetAnimationState);
        Runner.AddCommandHandler("SetName", SetName);
        Runner.AddCommandHandler("SetSpeaker", portraitManager.SetSpeaker);
        Runner.AddCommandHandler("RemovePortrait", portraitManager.RemovePortrait);
        sentences = new Queue<string>();
    }
    public void StartDialogue(NPC npc)
    {

        Runner.StartDialogue(npc.talkToNode);
        animator.SetBool("IsOpen", true);
        //nameText.text = npc.characterName;
    }

    public void SetName(string[] name)
    {
        nameText.text = name[0];
    }
    

    public void DisplayNextSentence()
    {
        UI.MarkLineComplete();
        /*
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        */
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray()) //ToCharArray converts strings into character arrays
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
    }
 
}
