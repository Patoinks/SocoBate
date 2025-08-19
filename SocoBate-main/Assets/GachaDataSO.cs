// GachaDataSO.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Models; // Assuming BaseUnit is in the Models namespace

// This allows you to create an instance of this object in your Project folder.
[CreateAssetMenu(fileName = "New Gacha Data", menuName = "Gacha/Gacha Data Asset")]
public class GachaDataSO : ScriptableObject
{
    [Header("Unit Pool")]
    [Tooltip("The list of all units that can be pulled from this gacha.")]
    public List<BaseUnit> unitPool;

    [Header("Rarity Chances")]
    [Tooltip("The percentage chance to pull a unit of a specific rarity.")]
    public List<RarityChance> rarityChances;
    
    // A helper method to easily get a chance by rarity.
    public int GetChanceForRarity(int rarity)
    {
        var chance = rarityChances.FirstOrDefault(rc => rc.rarity == rarity);
        return chance?.chance ?? 0;
    }
}

// A simple serializable class to make editing rarity chances easy in the Inspector.
[System.Serializable]
public class RarityChance
{
    public int rarity;
    [Range(0, 100)] public int chance;
}