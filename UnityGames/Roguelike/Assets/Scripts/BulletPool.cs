using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletPool : MonoBehaviour
{
    public List<Bullet> PooledObjects;



    public Bullet GetObjectInPool(Bullet findObject, Transform transform)
    {
        for (int i = 0; i < PooledObjects.Count; i++)
        {

            if (!PooledObjects[i].gameObject.activeInHierarchy && findObject.type == PooledObjects[i].type)
            {
                PooledObjects[i].gameObject.SetActive(true);
                PooledObjects[i].transform.position = transform.position;
                //Debug.Log("Returning Pre-Exisitng bullet!");
                return PooledObjects[i];
            }
        }
        return SpawnToPool(findObject, transform);
    }


    Bullet SpawnToPool(Bullet spawnObject, Transform transform)
    {
        //Debug.Log("Spawning new bullet!");
        Bullet instance = Instantiate(spawnObject, transform.position, transform.rotation);
        PooledObjects.Add(instance);      
        return instance;

    }


    //If you change spells, you'll need to replace the spell bullets in the pool
    public void ClearPool()
    {
        for (int i = 0;i < PooledObjects.Count; i++)
        {
            Destroy(PooledObjects[i]);
            
        }
        PooledObjects.Clear();
    }


}
