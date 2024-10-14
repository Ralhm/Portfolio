using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldUI : MonoBehaviour
{

    //Dont destroy on load
    //World to screen point

    //For more complicated menus like from shops, create a function that generates the UI element with all buttons, rather than make those buttons in Yarn

    public static FieldUI uiSingleton;

    [SerializeField] GameObject uiContainer = null;
    public Animator transition;

    public bool inField;
    public bool Encounters;
    public float encounterRate;
    public float transitionTime = 1f;
    float enemyEncounter;
    public bool inDialogue = false;

    public PlayerController Player;
    public DialogueManager dialogueManager;
    public GameManager Manager;
    public Camera cam;
    


    IEnumerator LoadLevel()
    {
        transition.SetTrigger("Start");

        //Manager.state = GameManager.State.Battle;

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene("TestScene2");

        
    }

    public void InDialogue()
    {
        Player.state = PlayerController.State.Dialogue;
    }
    public void ExitDialogue()
    {
        Player.state = PlayerController.State.Idle;
        dialogueManager.EndDialogue();
    }


    void Start()
    {

        uiSingleton = this;

        uiContainer.SetActive(false);

        //Manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //Manager.ActivatePlayer();

        Player = GameObject.Find("Player").GetComponent<PlayerController>();


        //cam.GetComponent<Transform>().position = Player.GetComponent<Transform>().position;



    }

    
    void Update()
    {

        if (Input.GetButtonDown("Menu"))
        {
            uiContainer.SetActive(!uiContainer.activeSelf); //Toggle activation with single key press
            if ((Player.state == PlayerController.State.Moving) || (Player.state == PlayerController.State.Idle))
            {
                Player.state = PlayerController.State.Menu;
            }
            else
            {
                Player.state = PlayerController.State.Idle;
            }
        }

        
        if (Encounters) //Encounter rate updater
        {
            /*
            if (Player.state == PlayerController.State.Moving)
            {
                encounterRate += (Time.deltaTime * 0.03f);    //Make encounter rate increase with movement time
                enemyEncounter = Random.Range(0f, 100f);
                if (enemyEncounter < encounterRate)
                {
                    StartCoroutine(LoadLevel());
                }
                
            }
            */
        }

        
    }
}
