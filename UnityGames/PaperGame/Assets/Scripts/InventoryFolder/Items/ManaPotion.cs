using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : InventoryItem
{
    public override void Execute(Player player)
    {
        player.RestoreSP(value);
    }
}
