using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //Try redoing this but instead of storing data in arrays, you store each characters info in a unique file
    //This will allow for easier reading and dynamic party compositions
    public int[] level;
    public int[] health;
    public int[] maxHealth;
    public int[] attackPower;
    public int[] maxSP;
    public int[] SP;

    public PlayerData(List<Player> players)
    {
        maxSP = new int[players.Count];
        maxHealth = new int[players.Count];
        health = new int[players.Count];
        level = new int[players.Count];
        attackPower = new int[players.Count];
        SP = new int[players.Count];


        for (int i = 0; i < players.Count; i++)
        {
            maxSP[i] = players[i].maxSP;
            maxHealth[i] = players[i].maxHealth;
            level[i] = players[i].level;
            health[i] = players[i].health;
            SP[i] = players[i].SP;
            attackPower[i] = players[i].attackPower;
        }
    }
}
