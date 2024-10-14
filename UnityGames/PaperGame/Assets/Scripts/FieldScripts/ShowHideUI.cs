using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] GameObject uiContainer = null;

    void Start()
    {

        uiContainer.SetActive(false);

    }

    public void DisplayUI()
    {
        uiContainer.SetActive(!uiContainer.activeSelf); //Toggle activation with single key press
    }

}
