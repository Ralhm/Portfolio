using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSpawnPoint : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {

        GetComponentInParent<Room>().AddCrystalSpawnPoint(this);
    }

    public void SpawnCrystal(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameManager.instance.SpawnCrystal(transform);
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
#endif
}
