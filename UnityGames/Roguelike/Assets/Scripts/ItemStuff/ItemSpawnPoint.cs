using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnPoint : MonoBehaviour
{
    public List<Item> items;

    public ShopType Type;
    public bool SpawnOnAwake = false;

    private void Start()
    {
        if (SpawnOnAwake)
        {
            SpawnItem();
        }
        GetComponentInParent<Room>().AddItemSpawnPoint(this);
    }
    public void SpawnItem()
    {
        int index = 0;
        if (items.Count > 1)
        {
            index = Random.Range(0, items.Count);
        }


        if (items != null)
        {

            Item newItem = Instantiate(items[index], this.transform);
            newItem.Type = Type;
            newItem.ActivatePriceTag(Type);
        }
        else
        {
            Debug.Log("ITEMS ARE NULL");
        }


        
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1);
    }
#endif

}
