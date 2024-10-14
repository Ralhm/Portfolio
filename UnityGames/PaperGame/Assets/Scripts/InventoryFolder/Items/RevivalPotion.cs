using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalPotion : InventoryItem
{
    public override void Execute(Player player)
    {
        player.Revive(value);
    }
}
