using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatEffectDisplay : MonoBehaviour
{
    public RectTransform RT;
    public List<Image> debuffs;
    public List<Image> buffs;
    public Image statusEffect;
    public Sprite defaultImage;

    public void SetDefaultImage(Image image)
    {
        image.sprite = defaultImage;


    }




    void Awake()
    {
        /*
        for (int i = 0; i < debuffs.Count; i++)
        {
            debuffs[i].sprite = defaultImage;
        }
        for (int i = 0; i < buffs.Count; i++)
        {
            buffs[i].sprite = defaultImage;
        }
        */
    }

    //Add Default images that it resets to when the buff is not active

}
