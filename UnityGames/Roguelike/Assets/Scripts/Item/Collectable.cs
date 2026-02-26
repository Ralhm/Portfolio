using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    public int Value;
    public float SpawnForce;
    public float AttractionForce;
    public float DistanceRange;

    protected Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RandomForce();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attraction()
    {
        if (Vector2.Distance(rb.position, Player.Instance.RB.position) < DistanceRange)
        {
            rb.AddForce((Player.Instance.RB.position - rb.position) * AttractionForce);
        }
    }

    public virtual void RandomForce()
    {
        Vector2 dir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        rb.AddForce(dir * SpawnForce);
    }



}
