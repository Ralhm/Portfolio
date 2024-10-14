using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : InventoryItem
{
    
    public override void Execute(Player player)
    {
        player.RestoreHealth(value);
    }
}
