using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Buttons : MonoBehaviour
{
    public RectTransform RT;
    public int index;
    public Vector2 point;
    public bool highlighted = false;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Enter()
    {
        highlighted = true;
        point = RT.position;
        StartCoroutine(Float());

    }

    public void Exit()
    {
        highlighted = false;

        RT.position = point;
    }

    public void Click()
    {
        BattleUI.UIsingleton.AudioJungle.Play("Select");
        BattleUI.UIsingleton.Attack(index);
    }

    public IEnumerator Float()
    {
        while (highlighted == true)
        {
            RT.position += new Vector3(0, Mathf.Cos(Time.time * 5F), 0);
            yield return null;
        }
        
    }


}
