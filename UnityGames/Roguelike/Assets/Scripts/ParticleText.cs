using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParticleText : MonoBehaviour
{
    public TextMesh DamageText;
    public float LifeSpan;
    public float SpawnForce;
    public Rigidbody2D RB;

    private void Start()
    {

    }

    public void SetDamage(Vector3 pos, int num)
    {
        transform.position = pos;
        DamageText.text = "-" + num.ToString();
        RandomForce();
        StartCoroutine(Deactivate());
    }


    public void RandomForce()
    {
        Vector2 force = new Vector2(Random.Range(-1.0f, 1.0f) * SpawnForce, SpawnForce);

        RB.AddForce(force);
        //Debug.DrawRay(transform.position, force, Color.blue, 5);
    }


    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(LifeSpan);
        gameObject.SetActive(false);
    }

}
