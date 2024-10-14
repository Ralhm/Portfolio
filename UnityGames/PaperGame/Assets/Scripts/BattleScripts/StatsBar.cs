using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StatsBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI statsDisplay;
    public string statType;
    public Image fillBar;

    public void SetText(int current, int max)
    {
        statsDisplay.text = statType + current.ToString() + " / " + max.ToString();
    }

    public void SetMaxValue(int num)
    {
        slider.maxValue = num;
        slider.value = num;

    }

    public void SetValue(int num)
    {
        slider.value = num;
    }
}
