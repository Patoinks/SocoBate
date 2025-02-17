using UnityEngine;
using TMPro;
using System.Linq;
using Context;
using Database;
using System;
using Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Gacha
{
    public class GachaManagerCS : MonoBehaviour
    {
        public TextMeshProUGUI resultText;    // For displaying the result (Unit name)
        public GameObject heroSpritePrefab;   // The prefab for displaying the unit sprite (if needed)
        public Transform spawnPoint;          // Point where the unit sprite will spawn

        private Dictionary<int, int> rarityChances = new Dictionary<int, int>
        {
            { 2, 55 },   // 10% chance for Rarity 2 (Rare)
            { 3, 35 },   // 10% chance for Rarity 3 (Rare)
            { 4, 7 },   // 10% chance for Rarity 4 (Epic)
            { 5, 3 },    // 7% chance for Rarity 5 (Legendary)
        };

        private void Start()
        {
            UnitContext.LoadAllUnitsFromSerializedData(); // Load all units from SerializedObjects

            // Debugging: Check how many units are loaded
            Debug.Log($"Total Units Loaded: {UnitContext.allUnits.Count}");
            foreach (var unit in UnitContext.allUnits)
            {
                Debug.Log($"Loaded Unit: {unit.unitName} | Rarity: {unit.rarity}");
            }
        }


        public async void PullGacha()
        {
            // Get the pulled unit based on rarity chances
            BaseUnit pulledUnit = GetRandomUnitFromPool();

            // Display results and send unit data to the player's account
            ShowResult(pulledUnit);

            // Add the unit to the account and database
            await AddUnitToAccount(pulledUnit);
        }

        // Get a random unit based on rarity chances
        // Get a random unit based on rarity chances, but exclude rarity 2
        private BaseUnit GetRandomUnitFromPool()
{
    // Create a list of units weighted by their rarity chance
    List<BaseUnit> weightedPool = new List<BaseUnit>();

    foreach (var unit in UnitContext.allUnits)
    {
        if (rarityChances.ContainsKey(unit.rarity))
        {
            int weight = rarityChances[unit.rarity];

            // Add the unit to the list multiple times based on its weight
            for (int i = 0; i < weight; i++)
            {
                weightedPool.Add(unit);
            }
        }
    }

    // Check if we have any units to pick from
    if (weightedPool.Count == 0)
    {
        Debug.LogWarning("No valid units available for gacha pull!");
        return null;
    }

    // Pick a random unit from the weighted pool
    int randomIndex = UnityEngine.Random.Range(0, weightedPool.Count);
    return weightedPool[randomIndex];
}




        // Get the chance for a given rarity
        private int GetUnitRarityChance(int rarity)
        {
            return rarityChances.ContainsKey(rarity) ? rarityChances[rarity] : 0;
        }

        private void ShowResult(BaseUnit unit)
        {
            if (unit != null)
            {
                // Display the unit name and rarity
                resultText.text = $"You pulled: {unit.unitName} (Rarity: {unit.rarity})";

                // Optionally, instantiate the unit sprite prefab at a certain position
                GameObject unitSprite = Instantiate(heroSpritePrefab, spawnPoint.position, Quaternion.identity);
                unitSprite.GetComponent<SpriteRenderer>().sprite = unit.unitSprite;
            }
            else
            {
                resultText.text = "No unit pulled!";
            }
        }

        private async Task AddUnitToAccount(BaseUnit unit)
        {
            if (unit != null)
            {
                Guid accountId = UserContext.account.AccountId;  // Get the logged-in account ID
                Debug.Log($"Unit drawn: {unit.unitName} | Account ID: {accountId} | Rarity: {unit.rarity}");

                // Insert the new hero into the account and the database
                await UnitController.NewHeroUnlocked(accountId, unit.unitName);  // Pass the unit name (string) to match the DB schema
            }
        }
    }
}
