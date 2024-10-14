using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : Container
{
    [SerializeField]
    public ItemData[] items;
    public ItemData GetRandomItem()
    {
        int rand = Random.Range(0, items.Length);

        return items[rand];
    }
}
