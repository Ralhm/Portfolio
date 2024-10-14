using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFloat : MonoBehaviour
{

    public Sprite Pointer;
    // Start is called before the first frame update


    void OnEnable()
    {
        StartCoroutine(Float());
    }

    public IEnumerator Float()
    {
        while (this.gameObject.activeSelf)
        {
            transform.position += new Vector3(0, Mathf.Cos(Time.time * 5f), 0) * 0.1f;
            yield return null;
        }

    }
}
