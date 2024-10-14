using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public string TextToDisplay;
    public float TextSpeed;
    public bool isTyping;

    public void DisplayText(string name, string description)
    {
        NameText.text = name;
        TextToDisplay = description;
        StartCoroutine(PrintLetters());
    }

    public IEnumerator PrintLetters()
    {
        isTyping = true;
        DescriptionText.text = "";
        for (int i = 0; i < TextToDisplay.Length; i++)
        {
            DescriptionText.text += TextToDisplay[i];
            yield return new WaitForSeconds(TextSpeed);
        }
        isTyping = false;
    }
}
