using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public List<ItemData> Items;

    [Header("BaseStats")]
    public int MaxHeat;
    public int MaxHealth;
    public int Health;
    public float MaxMana;
    public int Speed;
    public int Money;
    public int Crystals;

    public bool HasKey;


    [Header("Modifiers")]
    public float ManaCostModifier = 1.0f;
    public float DamageModifier = 1.0f;
    public float SpeedModifier = 1.0f;
    public float ManaRechargeModifier = 1.0f;
    public float HeatLossModifier = 1.0f;
    public float FireRateModifier = 1.0f;

    public float MeleeModifier = 1.0f;
    public float SpecialModifier = 1.0f;
    public float ProjectileModifier = 1.0f;

    public float FireModifier = 1.0f;
    public float IceModifier = 1.0f;
    public float ElectricModifier = 1.0f;



    [Header("Spells")]
    public Spell ProjectileSpell;
    public Spell MeleeSpell;
    public Spell SpecialSpell;
}
