using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifierPanel : MonoBehaviour
{
    public TextMeshProUGUI ValueText;


    public void EditText(string text)
    {
        ValueText.text = text;
    }

}
