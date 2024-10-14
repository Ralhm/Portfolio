using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    public TextMeshProUGUI SpellText;
    //public TextMeshProUGUI ManaText;


    public Slider ManaSlider;
    public Slider HealthSlider;
    public Slider HeatSlider;


    public ModifierPanel MoneyPanel;
    public ModifierPanel CrystalsPanel;

    public ModifierPanel HeatPanel;
    public ModifierPanel MaxHeatPanel;

    public ModifierPanel HealthPanel;
    public ModifierPanel MaxHealthPanel;
    public ModifierPanel ManaPanel;
    public ModifierPanel MaxManaPanel;
    public ModifierPanel SpeedPanel;
    public ModifierPanel ManaCostPanel;
    public ModifierPanel DamagePanel;
    public ModifierPanel ManaRechargePanel;
    public ModifierPanel HeatLossPanel;
    public ModifierPanel FireRatePanel;

    public ModifierPanel FirePanel;
    public ModifierPanel IcePanel;
    public ModifierPanel ElectricPanel;

    public bool ShowingItemText;


    public TextMeshProUGUI MinutesText;
    public TextMeshProUGUI SecondsText;

    private static UI instance;
    public static UI Instance { get { return instance; } }

    // Start is called before the first frame update
    void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //text.text = player.currentSpell;
    }
    /*
    public void DisplayItemText(string name, string description)
    {
        if (!ShowingItemText)
        {
            Display.gameObject.SetActive(true);
            Display.DisplayText(name, description);
        }

    }

    public void HideItemText()
    {
        Display.gameObject.SetActive(false);
    }
    */
    public void SetText(string name)
    {
        SpellText.text = name;
    }


    public void SetHeat(float num)
    {
        HeatSlider.value = num;
        HeatPanel.EditText(num.ToString());
    }

    public void SetMaxHeat(float num)
    {
        HeatSlider.maxValue = num;
    }


    public void SetMaxMana(float max)
    {
        ManaSlider.maxValue = max;
        MaxManaPanel.EditText(max.ToString());
    }

    public void SetMana(float num)
    {
        ManaSlider.value = num;
        ManaPanel.EditText(Mathf.Round(num).ToString());
    }


    public void SetMaxHealth(float max)
    {
        HealthSlider.maxValue = max;

    }

    public void SetHealth(float val)
    {
        HealthSlider.value = val;
        HealthPanel.EditText(val.ToString());
    }


    public void SetMoney(int amount)
    {
        MoneyPanel.EditText(Player.Instance.GetMoney().ToString());
    }

    public void SetCrystals(int amount)
    {
        CrystalsPanel.EditText(Player.Instance.GetCrystals().ToString());
    }

    public void UpdateDisplayValues()
    {


        MaxHeatPanel.EditText(Player.Instance.GetMaxHeat().ToString());

        HealthPanel.EditText(Player.Instance.GetHealth().ToString());
        MaxHealthPanel.EditText(Player.Instance.GetMaxHealth().ToString());
        ManaPanel.EditText(Player.Instance.GetMana().ToString());
        MaxManaPanel.EditText(Player.Instance.GetMaxMana().ToString());
        SpeedPanel.EditText(Player.Instance.GetSpeedModifier().ToString());
        ManaCostPanel.EditText(Player.Instance.GetManaCostModifier().ToString());
        DamagePanel.EditText(Player.Instance.GetDamageModifier().ToString());
        ManaRechargePanel.EditText(Player.Instance.GetRechargeModifier().ToString());
        HeatLossPanel.EditText(Player.Instance.GetHeatLossModifier().ToString());
        FireRatePanel.EditText(Player.Instance.GetFireRateModifier().ToString());

        FirePanel.EditText(Player.Instance.GetFireModifier().ToString());
        IcePanel.EditText(Player.Instance.GetIceModifier().ToString());
        ElectricPanel.EditText(Player.Instance.GetElectricModifier().ToString());


    }

    public void SetTimer(float minutes, int seconds)
    {
        MinutesText.text = minutes.ToString();
        SecondsText.text = seconds.ToString();
    }
}
