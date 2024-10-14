using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


public class PlayerController : MonoBehaviour
{
    public static PlayerController playerSingleton;

    //Make the dialogue display portraits
    //Make the portraits animate

    //Make the dialogue able to move/vibrate




    public enum State
    {
        Menu, Moving, Idle, Dialogue
    }

    public State state = State.Idle;

    public bool test = false;
    public int speed;
    public int acceleration;
    public int Money = 30;

    public float horizInput;
    public float verticalInput;

    public Rigidbody2D RB;
    public DialogueRunner Runner;
    public FieldUI fieldUI;

    public Yarn.Value yarn;


    void Start()
    {
        fieldUI = FieldUI.uiSingleton;
        Runner = FieldUI.uiSingleton.dialogueManager.Runner;
        Runner.AddCommandHandler("Purchase", SpendMoney);
        Runner.AddCommandHandler("Adjust", AdjustYarnValue);
        Runner.AddCommandHandler("AdjustBool", AdjustYarnBool);
        playerSingleton = this;
        state = State.Idle;
        
        print("Player Money = " + Money);


        
        Runner.variableStorage.SetValue("Money", Money.ToString());
        yarn = Runner.variableStorage.GetValue("Money");
        print(yarn);

        SetYarnValue("$Money", Money.ToString());

    }

    public void SetYarnValue(string value, string amount)
    {
        Runner.variableStorage.SetValue(value, amount);
    }

    public void AdjustYarnBool(string[] value)
    {

        /*
        if (value[1] == "1")
        {
            test = true;
            print(test);
        }
        else
        {
            test = false;
        }
        */
    }

    public void AdjustYarnValue(string[] value) //This should take in the name and how much it should change
    {

        Runner.variableStorage.SetValue("$" + value[0], Money.ToString());


    }

    public void SpendMoney(string[] amount)
    {
        print("Getting Value: " + Runner.variableStorage.GetValue(amount[0]));

        Money -= int.Parse(amount[0]);
        print("Current Money = " + Money);

        AdjustYarnValue(amount);
    }

    void Update()
    {


        if (state != State.Menu && state != State.Dialogue)
        {
            horizInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            RB.velocity = new Vector2(horizInput * speed, verticalInput * speed);
            if ((horizInput != 0) || (verticalInput != 0))
            {
                state = State.Moving;
            }

            else
            {
                state = State.Idle;
            }
        }
        else
        {
            horizInput = 0;
            verticalInput = 0;
            RB.velocity = new Vector2(horizInput * speed, verticalInput * speed);
        }






        
    }
}
