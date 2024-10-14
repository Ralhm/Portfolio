using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour
{
    public float distToPlayer;
    public DialogueRunner dialogueRunner;

    public string characterName = "";

    public string talkToNode = "";

    public YarnProgram scriptToLoad;

    void Start()
    {
        dialogueRunner = GameObject.Find("DialogueManager").GetComponent<DialogueRunner>();

        if (scriptToLoad != null)
        {
            DialogueRunner dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
            dialogueRunner.Add(scriptToLoad);
        }
    }

    void Update()
    {
        distToPlayer = Vector2.Distance(transform.position, PlayerController.playerSingleton.transform.position);

        if (distToPlayer < 2F)
        {
            if (Input.GetButtonDown("Submit") && PlayerController.playerSingleton.state != PlayerController.State.Dialogue)
            {
                FieldUI.uiSingleton.dialogueManager.StartDialogue(this);
            }
        }
    }
}
