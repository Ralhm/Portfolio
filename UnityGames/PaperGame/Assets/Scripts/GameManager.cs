using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public List<Player> PlayerStats;
    public PlayerController FieldPlayer;
    public GameObject Manager;
    public int battles = 0;


    public enum State
    {
        Field, Battle, Menu
    }

    public State state = State.Menu;

    // Start is called before the first frame update
    void Awake()
    {
        SaveSystem.SavePlayer(PlayerStats);

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivatePlayer()
    {
        FieldPlayer.gameObject.SetActive(!FieldPlayer.gameObject.activeSelf);  
    }

    public void LoadNextScene() //This should represent starting a new game
    {
        //FieldPlayer = Instantiate(FieldPlayer);
        //FieldPlayer.name = "Player";

        DontDestroyOnLoad(Manager);
        //DontDestroyOnLoad(FieldPlayer);

        //ActivatePlayer();
        state = State.Battle;
        //PlayerPrefs.DeleteAll(); //Delete all playerprefs upon starting a new game
        SceneManager.LoadScene("TestScene2");
        

    }
}
