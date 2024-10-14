using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleUI : MonoBehaviour
{
    //Implement new characters
    //Allow player to switch/choose party members
    //Animations for Party Skills
    //Make another QTE where you just hold the button up to a limit
    //There needs to be a condition for when selecting all players

    //Polish UI
    //Dialogue (Boxes, Text, advancing text, display portrait of speaking person)


    public List<InventoryItem> slots;
    public InventoryItem queuedItem;
    public TextRendererParticleSystem DamageNumbers;

    public enum AttackType
    {
        None, Mash, Sequence, Aim, Timing
    }
    public AttackType attackType;


    public enum State
    {
        SelectingPlayer, Menu, SelectingEnemy, SelectingAllEnemies, SelectingAllAllies, SelectingAllyItem, SelectingAllySkill, Attacking, Hold
    }

    public State state;

    public static BattleUI UIsingleton;

    [SerializeField] GameObject uiSkillContainer = null;
    [SerializeField] GameObject uiItemContainer = null;
    public GameManager Manager;

    public GameObject pointer;
    public GameObject newPointer;
    public GameObject TargetSelect;
    public List<GameObject> MultiTargetSelect;

    public GameObject Panel; //This panel exists so that the stat effects can be above the menu buttons when spawned, make them children of this panel

    public Animator transition;
    public float transitionTime = 1f;

    [Header("Prefab Storage")]
    //Prefabs storage
    public List<Player> players;
    public List<Enemy> EnemiesPrefabs;
    public Button SkillDisplayButton;
    public Button EnemySelectButton;
    public Button ItemDisplayButton;
    public Button ItemUserButton;
    public PlayerMenu playerMenu;
    public List<StatsBar> HealthBar;
    public List<StatsBar> SkillBar;
    public List<Sprite> buffImages;
    public List<Sprite> StatEffectImages;
    public GameObject TargetImage;
    public StatEffectDisplay statEffectDisplay;
    public StatsBar MashBarDisplay;
    public Image MashFillDisplay;

    //Lists that call on and instantiate prefabs
    public List <Enemy> enemies;
    public List <Button> enemySelection;
    public List <Button> skillButtons;
    public List <Button> itemButtons;
    public List <Button> PartySelectButtons;
    public List <Player> party;
    public List <PlayerMenu> playerMenuList;
    public List <GameObject> QTEalerts;

    public TextMeshProUGUI timeDisplay;

    public List<Vector2> enemyPositions;

    public Camera cam;

    [Header("Variables")]
    public int turnCounter = 0;
    public int partyTurn = 0; //Which character are you currently selecting an action for
    public int buttonCount;
    public int playerAttacks;
    public int alivePlayers = 0; //If all players are dead, end game
    public int aliveEnemies;
    public int skillIndex;
    public int drawRateSum = 0;
    public int enemyActions = 0;
    public float mashLimit;
    public float timer;
    int selection;
    public int PlayerTargetIndex;
    public int EnemyTargetIndex;  

    int enemyarrange;
    public bool EnemyDied;
    public bool inMenu = false;
    public bool selectingTarget = false;
    public bool Hit = false; //If user does QTE correctly, pass this in to let skill function know how to calculate damage and whatnot
    bool UsedAnItem = false;
    public float MashBar = 0;
    int direction = 1; //Shifts the aiming reticles direction;
    public int temp;
    public float detectRange;
    bool skipAttack = false;
    float distToEnemy;
    bool miss = false;
    bool timingMiss = true;

    public TextMeshProUGUI winText;

    public AudioManager AudioJungle;
    public ParticleSystem particles;

    IEnumerator LoadLevel()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
    
        SceneManager.LoadScene("FieldTest");
      
    }

    IEnumerator LoadBattle() //
    {
        print("Loading Battle");
        for (int i = 0; i < party.Count; i++) //Revive party Member at end of battle if dead
        {
            if (party[i].health <= 0)
            {
                party[i].health = 1;
            }
        }
        SaveSystem.SaveInventory(slots);
        //SavePlayerStats();
        Manager.battles += 1;
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("TestScene2");

    }

    IEnumerator Hold(int targetIndex, bool usingItem)
    {
        QTEalerts[selection].SetActive(false); //Stop displaying any QTE alerts if they failed
        

        if (Hit)
        {
            print("Entering Hit State");
            party[partyTurn].animator.SetTrigger("AttackExecute");
            AudioJungle.Play(party[partyTurn].queuedSkill.audioName);
            
        }
        else
        {
            print("Entering FailState");
            party[partyTurn].animator.SetTrigger("Fail");
            AudioJungle.Play("Miss");
        }
        if (!UsedAnItem)
        {       
            party[partyTurn].animator.SetBool(party[partyTurn].queuedSkill.type, false); //It breaks right here on the first turn if you use an item, put the if statement to prevent this
        }
        
        party[partyTurn].UpdateAnimationStatus(); //Return to the idle state they were in before the attack animation

        for (int i = 0; i < party.Count; i++) //Image Display for stat effects and buffs
        {
            BuffDisplay(party[i]);
            StatEffectDisplay(party[i]);
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            BuffDisplay(enemies[i]);
            StatEffectDisplay(enemies[i]);

        }
        
        UpdateDisplay();
        miss = false;
        timeDisplay.text = "";
        attackType = AttackType.None;
        DeactivateTargets();
        DisplayUserSkills();
        DeactivatePartyMemberOptions();
        state = State.Hold;
        yield return new WaitForSeconds(2F);
        party[partyTurn].animator.ResetTrigger("Fail");
        party[partyTurn].animator.ResetTrigger("AttackExecute");

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i].gameObject.activeSelf)
            {
                enemies.RemoveAt(i);
            }
        }

        uiItemContainer.SetActive(false);
        uiSkillContainer.SetActive(false);
        party[partyTurn].hasActed = true;
        playerAttacks -= 1; //When this reaches 0, all characters have acted, meaning it's now the enemies turn

        if (playerAttacks != 0)
        {
            SelectPlayer(true);
        }

        //enemyActions = enemies.Count;
        if (playerAttacks == 0) //Enemy Turn
        {
            
            DeactivateCharacterButtons(partyTurn);
            
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].gameObject.activeSelf)
                {
                    enemies[i].Attack(party);
                    yield return new WaitForSeconds(2F);
                    if (enemies[i].status == Character.Status.Poisoned)
                    {
                        enemies[i].Poison();
                        yield return new WaitForSeconds(1F);
                    }
                }


            }

            //End of Enemy Turn, prepare players next turn
            partyTurn = 0;
            SelectPlayer(true);
            
            alivePlayers = 0;
            for (int i = 0; i < 4; i++)
            {
                if (party[i].status != Character.Status.Dead)
                {
                    party[i].hasActed = false;
                    alivePlayers += 1;
                }
                
            }
            turnCounter += 1;
            playerAttacks = alivePlayers;

            for (int i = 0; i < enemies.Count; i++) 
            {
                BuffCounter(enemies[i]);
                if (enemies[i].status != Character.Status.Normal)
                {
                    StatEffectCounter(enemies[i]);
                }
            }

            for (int i = 0; i < party.Count; i++)
            {
                if (party[i].status == Character.Status.Poisoned)
                {
                    party[i].Poison();
                    if (party[i].health <= 0)
                    {
                        party[i].Dead();
                    }
                }
                BuffCounter(party[i]);
                if (party[i].status != Character.Status.Normal)
                {
                    StatEffectCounter(party[i]);
                }
                    

            }
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].hasActed = false;
        }


        state = State.SelectingPlayer;
        Hit = false;
        DisplayWin();
        
    }


    public void MultiTargetCheck(int targetIndex)
    {
        if (party[partyTurn].queuedSkill.singleTarget)
        {
            party[partyTurn].queuedSkill.Execute(enemies[targetIndex], party[partyTurn], Hit);
        }
        else
        {
            party[partyTurn].queuedSkill.Execute(enemies, party[partyTurn], Hit);
        }
    }

    IEnumerator AimAttack(int targetIndex) //Time a button press correctly on a reticle to do damage
    {
        temp = targetIndex;
        attackType = AttackType.Aim;
        newPointer.SetActive(true);
        newPointer.transform.position = enemies[targetIndex].transform.position + new Vector3(-7F, 0, 0);
        for (int i = 0; i < 5; i++)
        {


            if ((Hit) || (miss))
            {
                //print("breaking");
                break;
            }
            yield return new WaitForSeconds(party[partyTurn].queuedSkill.timing);
            direction *= -1;

        }


        newPointer.SetActive(false);
        MultiTargetCheck(targetIndex);


        StartCoroutine(Hold(targetIndex, false));
        direction = 1;

    }

    IEnumerator TimingAttack(int targetIndex) //Time a button press correctly to do damage
    {
        
        attackType = AttackType.Timing;
        for (int i = 1; i < 4; i++)
        {
            timeDisplay.text = i.ToString() + "...";
            yield return new WaitForSeconds(party[partyTurn].queuedSkill.timing);
            if (miss)
            {
                //print("Failure!");
                break;
            }         
        }

        if (!miss)
        {
            timeDisplay.text = "GO!";
        }
        timingMiss = false;
           
        yield return new WaitForSeconds(party[partyTurn].queuedSkill.timing * 0.5F);
        attackType = AttackType.None;
        MultiTargetCheck(targetIndex);

        timingMiss = true;
        StartCoroutine(Hold(targetIndex, false));
        

    }

    IEnumerator SequenceAttack(int targetIndex) //Input a randomly generated sequence of buttons correctly to do damage
    {
        temp = targetIndex;
        attackType = AttackType.Sequence;
        for (int i = 1; i < 5; i++)
        {
            timeDisplay.text = i.ToString();
            Hit = false;
            selection = Random.Range(0, 4);
            QTEalerts[selection].SetActive(true);
            yield return new WaitForSeconds(party[partyTurn].queuedSkill.timing);
            QTEalerts[selection].SetActive(false);
            if ((!Hit) || (miss))
            {
                //print("Failure!");
                
                break;
            }

        }
        MultiTargetCheck(targetIndex);
        StartCoroutine(Hold(targetIndex, false));
    }

    IEnumerator MashAttack(int targetIndex) //Mash a button to raise a bar to a certain degree
    {
        MashBar = 0;
        MashBarDisplay.gameObject.SetActive(true);
        attackType = AttackType.Mash;
        yield return new WaitForSeconds(party[partyTurn].queuedSkill.timing);

        if ((MashBar > 10F) && (MashBar < mashLimit))
        {
            Hit = true;
        }
        if (miss)
        {
            Hit = false;
        }
        MashBarDisplay.gameObject.SetActive(false);
        MultiTargetCheck(targetIndex);
        StartCoroutine(Hold(targetIndex, false));


    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < party.Count; i++)
        {
            HealthBar[i].SetText(party[i].health, party[i].maxHealth);
            HealthBar[i].SetValue(party[i].health);

            SkillBar[i].SetText(party[i].SP, party[i].maxSP);
            SkillBar[i].SetValue(party[i].SP);
        }       
    }

    void Awake()
    {
        UIsingleton = this;
        MashBarDisplay.gameObject.SetActive(false);
        
        Manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        DontDestroyOnLoad(Manager.gameObject);
        //Manager.ActivatePlayer();

        DamageNumbers.gameObject.SetActive(false);

        if (Manager.battles == 0) //Initialize Inventory if it's the first battle
        {
            slots.Add(Resources.Load<InventoryItem>("Health Potion"));
            slots.Add(Resources.Load<InventoryItem>("Health Potion"));
            slots.Add(Resources.Load<InventoryItem>("Health Potion"));
            slots.Add(Resources.Load<InventoryItem>("Mana Potion"));
            slots.Add(Resources.Load<InventoryItem>("Mana Potion"));
            slots.Add(Resources.Load<InventoryItem>("Mana Potion"));
            slots.Add(Resources.Load<InventoryItem>("Revival"));
            slots.Add(Resources.Load<InventoryItem>("Revival"));
            slots.Add(Resources.Load<InventoryItem>("Revival"));
            SaveSystem.SaveInventory(slots);
        }
        else
        {
            LoadInventory();
        }
        

        newPointer = Instantiate(pointer, this.transform);
        newPointer.SetActive(false);

        TargetSelect = Instantiate(TargetImage, this.transform);
        TargetSelect.SetActive(false);


        uiSkillContainer.SetActive(false);
        uiItemContainer.SetActive(false);

        
        enemyarrange = Random.Range(0, 4);
        
        if (enemyarrange == 0)
        {
            EnemyVariant1();
        }
        else if( enemyarrange == 1)
        {
            EnemyVariant2();
        }
        else if(enemyarrange == 2)
        {
            EnemyVariant3();
        }
        else
        {
            EnemyVariant4();
        }
        

        //EnemyVariant1();
        StartCoroutine(AssignEnemyStats()); //You have to do it this way otherwise the ui elements don't get displayed properly upon reloading the scene


        PlayerSpawn();


        alivePlayers = party.Count;
        buttonCount = enemies.Count;
        aliveEnemies = enemies.Count;
        playerAttacks = alivePlayers;

        print("Number of enemies: " + enemies.Count);

        //winText.text = "";


        for (int i = 0; i < party.Count; i++)
        {
            HealthBar[i].SetMaxValue(party[i].maxHealth);
            SkillBar[i].SetMaxValue(party[i].maxSP);
        }
        UpdateDisplay();

        MashFillDisplay = MashBarDisplay.fillBar.GetComponent<Image>();
    }

    IEnumerator AssignEnemyStats()
    {
        yield return new WaitForSeconds(0.5F);
        AssignEnemyUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("QuitGame"))
        {
            Application.Quit();
        }
       
        if (attackType == AttackType.Timing) //Timing Attack Update
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!timingMiss)
                {
                    AudioJungle.Play("Select");
                    Hit = true;
                }
                else if (timingMiss)
                {
                    miss = true;
                }                
            }
        }
        if (attackType == AttackType.Sequence) //Sequence Attack Update
        {
            if (selection == 0)
            {
                
                if (Input.GetButtonDown("Fire1"))
                {

                    //print("Correct!");
                    AudioJungle.Play("Select");
                    Hit = true;
                }
                else if ((Input.GetButtonDown("Fire2")) || (Input.GetButtonDown("Fire3")) || (Input.GetButtonDown("Fire4")))
                {
                    StopAllCoroutines();
                    StartCoroutine(Hold(temp, false));
                }

            }
            else if (selection == 1)
            {
                
                if (Input.GetButtonDown("Fire2"))
                {
                    //print("Correct!");
                    AudioJungle.Play("Select");
                    Hit = true;
                }
                else if ((Input.GetButtonDown("Fire1")) || (Input.GetButtonDown("Fire3")) || (Input.GetButtonDown("Fire4")))
                {
                    StopAllCoroutines();
                    StartCoroutine(Hold(temp, false));
                }

            }
            else if (selection == 2)
            {
                
                if (Input.GetButtonDown("Fire3"))
                {
                    //print("Correct!");
                    AudioJungle.Play("Select");
                    Hit = true;
                }
                else if ((Input.GetButtonDown("Fire2")) || (Input.GetButtonDown("Fire1")) || (Input.GetButtonDown("Fire4")))
                {
                    StopAllCoroutines();
                    StartCoroutine(Hold(temp, false));
                }

            }
            else if (selection == 3)
            {
                
                if (Input.GetButtonDown("Fire4"))
                {
                    //print("Correct!");
                    AudioJungle.Play("Select");
                    Hit = true;

                }
                else if ((Input.GetButtonDown("Fire2")) || (Input.GetButtonDown("Fire3")) || (Input.GetButtonDown("Fire1")))
                {
                    StopAllCoroutines();
                    StartCoroutine(Hold(temp, false));
                }
            }
        }
        if (attackType == AttackType.Mash) //Mashing Attack Update
        {
            MashBarDisplay.slider.value = MashBar;
            if (MashBar >= 0)
            {
                MashBar -= Time.deltaTime * 1.3F;
            }   
            if (Input.GetButtonDown("Fire1"))
            {
                MashBar += 0.7F;
            }
            if (MashBar > mashLimit)
            {
                MashBarDisplay.gameObject.SetActive(false);
                miss = true;
                attackType = AttackType.None;
            }
            
            if ((MashBar > 10F) && (MashBar < mashLimit))
            {
                MashFillDisplay.color = Color.green;

            }
            else
            {
                MashFillDisplay.color = Color.red;
            }
            
        }
        if (attackType == AttackType.Aim) //Aiming Attack Update
        {
            newPointer.transform.position += new Vector3(direction * Time.deltaTime * party[partyTurn].queuedSkill.timing * 8, 0, 0);
            distToEnemy = Vector2.Distance(newPointer.transform.position, enemies[temp].transform.position);
            if (Input.GetButtonDown("Fire1"))
            {
                newPointer.SetActive(false);
                if (distToEnemy < detectRange)
                {
                    AudioJungle.Play("Select");
                    Hit = true;
                    
                }
                else
                {
                    miss = true;
                }


            }
        }

        if (Input.GetButtonDown("Vertical") && (Input.GetAxis("Vertical") > 0)) //Allow player to choose which character to act with
        {
            switch (state)
            {
                case State.SelectingPlayer:
                    SelectPlayer(false);
                    break;

                case State.SelectingEnemy:
                    SelectEnemy(false);
                    break;
                case State.SelectingAllyItem:
                    SelectAlly(false);
                    break;

                case State.SelectingAllySkill:
                    SelectAlly(false);
                    break;
            }


        }
        else if (Input.GetButtonDown("Vertical") && (Input.GetAxis("Vertical") < 0))
        {
            switch (state)
            {
                case State.SelectingPlayer:
                    SelectPlayer(true);
                    break;

                case State.SelectingEnemy:
                    SelectEnemy(true);
                    break;

                case State.SelectingAllyItem:
                    SelectAlly(true);
                    break;

                case State.SelectingAllySkill:
                    SelectAlly(true);
                    break;
            }

        }

        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Fire1"))
        {
            switch (state)
            {
                case State.SelectingEnemy:
                    Attack(EnemyTargetIndex);
                    TargetSelect.SetActive(false);
                    break;

                case State.SelectingAllEnemies:
                    Attack(EnemyTargetIndex);
                    foreach(GameObject go in MultiTargetSelect)
                    {
                        Destroy(go);
                    }
                    MultiTargetSelect.Clear();
                    break;

                case State.SelectingAllyItem:
                    UseItem(PlayerTargetIndex);
                    UsedAnItem = true;
                    break;

                case State.SelectingAllySkill:
                    UsePartySkill(PlayerTargetIndex);
                    UsedAnItem = false;
                    break;
            }
            
        }

        if (state != State.Attacking && state != State.Hold) //Can only enter Menu if not attacking
        {
            if (state == State.Menu)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    AudioJungle.Play("Deselect");
                    DeactivateMenus(partyTurn);
                }
            }
            if (state == State.SelectingEnemy || state == State.SelectingAllEnemies)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    AudioJungle.Play("Deselect");
                    DeactivateTargets();

                }
            }
            if ((state == State.SelectingAllyItem) || (state == State.SelectingAllySkill))
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    AudioJungle.Play("Deselect");
                    DeactivatePartyMemberOptions();
                }

            }
        }

    }

    public void SelectEnemy(bool Direction)
    {
        AudioJungle.Play("Select");
        if (!Direction)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyTargetIndex -= 1;

                if (EnemyTargetIndex < 0)
                {
                    EnemyTargetIndex = enemies.Count - 1;
                }
                if (enemies[EnemyTargetIndex].gameObject.activeSelf) // exit if you're on an active enemy, loop around again otherwise
                {
                    break;
                }
            }

        }
        else
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyTargetIndex += 1;
                if (EnemyTargetIndex >= enemies.Count)
                {
                    EnemyTargetIndex = 0;
                }
                if (enemies[EnemyTargetIndex].gameObject.activeSelf) // exit if you're on an active enemy, loop around again otherwise
                {
                    break;
                }
            }
        }
        TargetSelect.transform.position = enemies[EnemyTargetIndex].transform.position;
    }

    public void SelectAlly(bool Direction)
    {
        AudioJungle.Play("Select");
        if (!Direction)
        {
            PlayerTargetIndex -= 1;
            if (PlayerTargetIndex < 0)
            {
                PlayerTargetIndex = party.Count - 1;
            }
        }
        else
        {
            PlayerTargetIndex += 1;
            if (PlayerTargetIndex >= party.Count)
            {
                PlayerTargetIndex = 0;
            }
        }
        TargetSelect.transform.position = party[PlayerTargetIndex].transform.position;
    }

    public void SelectPlayer(bool Direction) //If direction is false, move down player. Else, move up
    {
        AudioJungle.Play("Select");
        if (!Direction)
        {
            DeactivateCharacterButtons(partyTurn);
            for (int i = 0; i < 4; i++)
            {
                partyTurn -= 1;
                if (partyTurn < 0)
                {
                    partyTurn = 3;
                }
                if ((!party[partyTurn].hasActed) && (party[partyTurn].status != Character.Status.Dead))
                {
                    break;
                }
            }

            DisplayCharacterButtons(partyTurn);
        }
        else
        {
            DeactivateCharacterButtons(partyTurn);
            for (int i = 0; i < 4; i++)
            {
                partyTurn += 1;
                if (partyTurn > 3)
                {
                    partyTurn = 0;
                }
                if ((!party[partyTurn].hasActed) && (party[partyTurn].status != Character.Status.Dead))
                {
                    break;
                }
            }
            DisplayCharacterButtons(partyTurn);
        }


    }


    public void DisplayTargets() //Allow player to select target after selecting character to act with
    {
        /*
        for (int i = 0; i < enemySelection.Count; i++)
        {
            if (enemies[i].status != Character.Status.Dead)
            {
                enemySelection[i].gameObject.SetActive(true);
            }            
        }
        */
        DisplayUserSkills();

        if (party[partyTurn].queuedSkill.singleTarget)
        {
            state = State.SelectingEnemy;
            TargetSelect.SetActive(true);
            EnemyTargetIndex = 0;
            SelectEnemy(true);
        }
        else
        {
            state = State.SelectingAllEnemies;
            int j = 0;
            for (int i = 0; i < enemies.Count; i++) //Spawn target objects
            {
                if (enemies[i].gameObject.activeSelf) //Only create target button for active enemies
                {
                    MultiTargetSelect.Add(Instantiate(TargetImage, this.transform));
                    MultiTargetSelect[j].transform.position = enemies[i].transform.position;
                    j++;
                }

            }
        }
        
    }


    public void DeactivateTargets()
    {
        /* DEPRECATED FOR USING BUTTONS ON ENEMIES INSTEAD OF INDEXING
        for (int i = 0; i < enemySelection.Count; i++)
        {
            if (enemies[i].status != Character.Status.Dead)
            {
                enemySelection[i].gameObject.SetActive(false);
            }
           
        }
        */
        print("Deactivating Targets");
        if (state == State.SelectingEnemy)
        {
            TargetSelect.SetActive(false);
            print("Single Enemy Deactivation");
        }
        else if (state == State.SelectingAllEnemies)
        {
            print("Multi Enemy Deactivation");
            foreach (GameObject go in MultiTargetSelect)
            {
                Destroy(go);
            }
            MultiTargetSelect.Clear();
        }
        DisplayUserSkills();
        state = State.Menu;

        
    }

    //Display initial menu buttons

    public void DisplayUserSkills()
    {

        
        state = State.Menu;
        uiSkillContainer.SetActive(!uiSkillContainer.activeSelf); //Toggle activation with single key press
        DeactivateCharacterButtons(partyTurn);

    }
    /*
    public void DeactivateUserSkills()
    {
        state = State.Menu;
        uiSkillContainer.SetActive(!uiSkillContainer.activeSelf); //Toggle activation with single key press
        DeactivateCharacterButtons(partyTurn);
    }
    */

    public void DisplayUserItems()
    {
        
        state = State.Menu;
        uiItemContainer.SetActive(!uiItemContainer.activeSelf);
        DeactivateCharacterButtons(partyTurn);

    }

    public void DeactivateUserItems()
    {

    }

    public void DisplayCharacterButtons(int i) //Display/deactivate character buttons
    {
        if (party[i].status != Character.Status.Dead)
        {
            skillButtons[i].gameObject.SetActive(true);
            itemButtons[i].gameObject.SetActive(true);
        }


    }

    public void DeactivateCharacterButtons(int i)
    {
        skillButtons[i].gameObject.SetActive(false);
        itemButtons[i].gameObject.SetActive(false);
    }

    public void DeactivateMenus(int i) //Close all menus and return to players menu
    {
        state = State.SelectingPlayer;
        uiSkillContainer.SetActive(false);
        uiItemContainer.SetActive(false);
        DisplayCharacterButtons(i);
    }

    public void DisplayPartyMemberOptions() //Display Party Members to select to use the item on
    {
        for (int i = 0; i < party.Count; i++)
        {

            //PartySelectButtons[i].gameObject.SetActive(true);

        }


        PlayerTargetIndex = 0;
        TargetSelect.SetActive(true);
        SelectAlly(true);
        if (state == State.SelectingAllyItem)
        {
            uiItemContainer.SetActive(false);
        } 
        else if(state == State.SelectingAllySkill)
        {
            uiSkillContainer.SetActive(false);
        }
        

    }

    public void DeactivatePartyMemberOptions()
    {
        for (int i = 0; i < party.Count; i++)
        {

            //PartySelectButtons[i].gameObject.SetActive(false);
        }
        if (state == State.SelectingAllyItem)
        {
            DisplayUserItems();
        }
        else if (state == State.SelectingAllySkill)
        {
            DisplayUserSkills();
        }
        TargetSelect.SetActive(false);
        state = State.Menu;
    }

    public void UseItem(int target) 
    {
        print("Using " + queuedItem.itemName);
        AudioJungle.Play("Item");
        queuedItem.Execute(party[target]);

        UsedAnItem = true;

        slots.RemoveAt(queuedItem.index);
        StartCoroutine(Hold(0, true));
    }



    public void UsePartySkill(int target)
    {
        for (int i = 0; i < party.Count; i++)
        {

            //PartySelectButtons[i].gameObject.SetActive(false);
        }
        StartCoroutine(PartySkill(target));

    }

    IEnumerator PartySkill(int target)
    {
        print("using " + party[partyTurn].queuedSkill.SkillName + " on party member");
        
        Hit = true;
        party[partyTurn].animator.SetBool("Idle", false);
        party[partyTurn].animator.SetBool("Wounded", false);
        party[partyTurn].animator.SetBool(party[partyTurn].queuedSkill.type, true);
        yield return new WaitForSeconds(1F);
        party[partyTurn].queuedSkill.Execute(party[target], party[partyTurn], true);
        StartCoroutine(Hold(0, true));
    }

    IEnumerator DebuffSkill(int target) //Super Spaghetti, this is so debuff skills will actually work with how we hav it now
    {
        yield return new WaitForSeconds(1F);
        MultiTargetCheck(target);
        StartCoroutine(Hold(0, true));
    }

    public void Attack(int targetIndex) //Have selected character attack selected enemy
    {
        state = State.Attacking;
        UsedAnItem = false;
        party[partyTurn].animator.SetBool("Idle", false);
        party[partyTurn].animator.SetBool("Wounded", false);
        party[partyTurn].animator.SetBool(party[partyTurn].queuedSkill.type, true);
        for (int i = 0; i < enemySelection.Count; i++)
        {

            enemySelection[i].gameObject.SetActive(false);
            
        }

        switch (party[partyTurn].queuedSkill.action)
        {
            case Skills.ActionType.Timing:
                StartCoroutine(TimingAttack(targetIndex));
                break;

            case Skills.ActionType.Sequence:
                StartCoroutine(SequenceAttack(targetIndex));
                break;

            case Skills.ActionType.Aim:
                StartCoroutine(AimAttack(targetIndex));
                break;

            case Skills.ActionType.Mash:
                StartCoroutine(MashAttack(targetIndex));
                break;

            case Skills.ActionType.None:
                Hit = true; //uh oh spahettios              
                StartCoroutine(DebuffSkill(targetIndex)); 

                break;
        }
       
    }

    public void DisplayWin()
    {
        if (aliveEnemies == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                DeactivateCharacterButtons(i);
                if (party[i].health <= 0) //Revive dead characters
                {
                    party[i].health = 1;
                    party[i].UpdateAnimationStatus();
                }
            }
            
            winText.text = "Enemies trounced!";
            SavePlayerStats();
            StartCoroutine(LoadBattle());
        }

    }

    public void BuffDisplay(Character character)
    {
        for (int j = 0; j < character.buffs.Count; j++)
        {
            if (character.buffs[j].buffType == Skills.BuffType.Defense)
            {
                character.statEffectDisplay.buffs[j].sprite = buffImages[3];
            }
            if (character.buffs[j].buffType == Skills.BuffType.Attack)
            {
                character.statEffectDisplay.buffs[j].sprite = buffImages[1];
            }
        }
        for (int j = 0; j < character.debuffs.Count; j++)
        {
            if (character.debuffs[j].buffType == Skills.BuffType.Defense)
            {
                character.statEffectDisplay.debuffs[j].sprite = buffImages[2];
            }
            if (character.debuffs[j].buffType == Skills.BuffType.Attack)
            {
                character.statEffectDisplay.debuffs[j].sprite = buffImages[0];
            }
        }

    }

    public void BuffCounter(Character character)
    {
        for (int j = 0; j < character.debuffs.Count; j++)
        {
            character.debuffs[j].duration -= 1;
            if (character.debuffs[j].duration == 0)
            {
                character.debuffs[j].duration = character.debuffs[j].baseDuration;
                print("Removing " + character.debuffs[j].SkillName);
                character.debuffs.RemoveAt(j);
                character.statEffectDisplay.SetDefaultImage(character.statEffectDisplay.debuffs[j]);
            }
        }

        for (int j = 0; j < character.buffs.Count; j++)
        {
            character.buffs[j].duration -= 1;
            if (character.buffs[j].duration == 0)
            {
                character.buffs[j].duration = character.buffs[j].baseDuration;
                print("Removing " + character.buffs[j].SkillName);
                character.buffs.RemoveAt(j);
                character.statEffectDisplay.SetDefaultImage(character.statEffectDisplay.buffs[j]);

            }
        }
    }

    public void StatEffectDisplay(Character character)
    {
        if (character.status == Character.Status.Poisoned)
        {
            character.statEffectDisplay.statusEffect.sprite = StatEffectImages[0];
        }
        if (character.status == Character.Status.Stunned)
        {
            character.statEffectDisplay.statusEffect.sprite = StatEffectImages[1];
        }
        if (character.status == Character.Status.Sleep)
        {
            character.statEffectDisplay.statusEffect.sprite = StatEffectImages[2];
        }
        if (character.status == Character.Status.Paralyzed)
        {
            character.statEffectDisplay.statusEffect.sprite = StatEffectImages[3];
        }
        if (character.status == Character.Status.Confused)
        {
            character.statEffectDisplay.statusEffect.sprite = StatEffectImages[4];
        }
    }

    public void StatEffectCounter(Character character) //Check if Status Effect Wears Off
    {
        if (character.status != Character.Status.Normal && character.status != Character.Status.Dead)
        {
            character.duration -= 1;
            if (character.duration == 0)
            {
                print("Removing Status Effect from Character");
                character.animator.enabled = true;
                character.status = Character.Status.Normal;
                character.statEffectDisplay.SetDefaultImage(character.statEffectDisplay.statusEffect);
                character.duration = 2;

            }
        }
        
    }

    
    IEnumerator EaseIn(int i, float x) //Let x represent the final position
    {
        if (Manager.battles != 0)
        {
            LoadPlayerStats();
        }
        UpdateDisplay();
        for (int j = 0; j < 40; j++) //Probably could have just used a Lerp for this
        {
            party[i - 1].transform.position += new Vector3(x - party[i - 1].transform.position.x, 0, 0) * 0.1f;
            yield return null;
            if (party[i - 1].transform.position.x > x)
            {
                break;
            }
            
        }
        //Spawn skill menu activation buttons
        skillButtons.Add(Instantiate(SkillDisplayButton, this.transform));
        PlayerButtons newSkillButton = skillButtons[i - 1].GetComponent<PlayerButtons>();
        newSkillButton.RT.position = cam.WorldToScreenPoint(party[i - 1].transform.position);
        newSkillButton.RT.localPosition += new Vector3(-200F, 0, 0);
        newSkillButton.index = (i - 1); //Set the buttons index value, tying it to its corresponding character
        newSkillButton.gameObject.SetActive(false); //Set all player buttons as false at beginning

        //spawn item menu activation buttons
        itemButtons.Add(Instantiate(ItemDisplayButton, this.transform));
        PlayerButtons newItemButton = itemButtons[i - 1].GetComponent<PlayerButtons>();
        newItemButton.RT.position = cam.WorldToScreenPoint(party[i - 1].transform.position);
        newItemButton.RT.localPosition += new Vector3(200F, 0, 0);
        newItemButton.index = (i - 1);
        newItemButton.gameObject.SetActive(false);


        party[i - 1].statEffectDisplay = Instantiate(statEffectDisplay, this.transform);
        party[i - 1].statEffectDisplay.RT.position = cam.WorldToScreenPoint(party[i - 1].transform.position);
        party[i - 1].statEffectDisplay.RT.localPosition += new Vector3(0, 200F, 0);

        party[i - 1].index = i - 1;
        DisplayCharacterButtons(partyTurn); //Display Character 1's action button


        DamageNumbers.gameObject.SetActive(true);

    }
    public void CalculateDrawRate()
    {
        for (int i = 0; i < party.Count; i++)
        {

        }
    }
    
    public void PlayerSpawn()
    {
        for (int i = 1; i < 5; i++)
        {
            party.Add(Instantiate(players[i - 1]));
            //party[i - 1].transform.position = new Vector2(-4F + (-2F * i), 7.5F - (3F * i));
            party[i - 1].transform.position = new Vector2(-20F + (-20F * i), 4F - (2F * i));

            StartCoroutine(EaseIn(i, party[i - 1].transform.position.x * 0.1f));
            


        }

    }

    public void LoadInventory()
    {
        Inventory data = SaveSystem.LoadInventory();

        for (int i = 0; i < data.itemName.Length; i++)
        {
            slots.Add(Resources.Load<InventoryItem>(data.itemName[i]));

        }


    }

    public void LoadPlayerStats()
    {
        print("Loading Player Stats");
        PlayerData data = SaveSystem.LoadPlayer();

        for (int i = 0; i < party.Count; i++)
        {
            party[i].maxHealth = data.maxHealth[i];
            party[i].level = data.level[i];
            party[i].health = data.health[i];
            party[i].SP = data.SP[i];
            party[i].maxSP = data.maxSP[i];
            party[i].attackPower = data.attackPower[i];

        }
        
    }

    public void SavePlayerStats()
    {
        SaveSystem.SavePlayer(party);
    }
    
    public void AssignEnemyUI()
    {
        for (int i = 0; i < enemies.Count ; i++)
        {
            enemies[i].statEffectDisplay = Instantiate(statEffectDisplay, Panel.transform);
            enemies[i].statEffectDisplay.RT.position = cam.WorldToScreenPoint(enemies[i].transform.position);
            enemies[i].statEffectDisplay.RT.localPosition += new Vector3(0, 200F, 0);
            enemies[i].index = i;

            enemySelection.Add(Instantiate(EnemySelectButton, this.transform));
            Buttons newButton = enemySelection[i].GetComponent<Buttons>();
            newButton.RT.position = cam.WorldToScreenPoint(enemies[i].transform.position);

            newButton.index = (i);

        }

    }

    public void ReAssignEnemyValues()
    {
        print("REASSIGNING");
        for (int i = 0; i < enemies.Count; i++)
        {
            enemySelection[i].GetComponent<Buttons>().index = i;
            enemies[i].index = i;
        }
        EnemyDied = false;

    }



    //Create functions that generate various different enemy formations, then randomly pick between those functions
    public void EnemyVariant1()
    {
        for (int i = 1; i < 4; i++)
        {
            
            enemies.Add(Instantiate(EnemiesPrefabs[i - 1]));
            enemies[i - 1].transform.position = enemyPositions[(i - 1) * 2];
            
            
        }

    }
    public void EnemyVariant2()
    {
        for (int i = 1; i < 5; i++)
        {
            enemies.Add(Instantiate(EnemiesPrefabs[1]));
            enemies[i - 1].transform.position = enemyPositions[i - 1];
        }

    }
    public void EnemyVariant3()
    {
        enemies.Add(Instantiate(EnemiesPrefabs[0]));
        enemies.Add(Instantiate(EnemiesPrefabs[1]));
        enemies.Add(Instantiate(EnemiesPrefabs[0]));

        for (int i = 1; i < 4; i++)
        {
            enemies[i - 1].transform.position = enemyPositions[i * 2 - 2];

        }

    }

    public void EnemyVariant4()
    {
        for (int i = 1; i < 3; i++)
        {
            enemies.Add(Instantiate(EnemiesPrefabs[2]));
            enemies[i - 1].transform.position = enemyPositions[i * 2 - 1];

        }

    }

    public void EnemyVariant0()
    {
        enemies.Add(Instantiate(EnemiesPrefabs[1]));
        enemies[0].transform.position = enemyPositions[1];

    }



}

