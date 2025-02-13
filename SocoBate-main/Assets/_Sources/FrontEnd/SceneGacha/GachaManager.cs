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
            { 1, 50 },   // 50% chance for Rarity 1 (Common)
            { 2, 20 },   // 20% chance for Rarity 2 (Uncommon)
            { 3, 10 },   // 10% chance for Rarity 3 (Rare)
            { 4, 10 },   // 10% chance for Rarity 4 (Epic)
            { 5, 7 },    // 7% chance for Rarity 5 (Legendary)
            { 6, 3 }     // 3% chance for Rarity 6 (Mythic)
        };

        private void Start()
        {
            UnitContext.LoadAllUnitsFromSerializedData(); // Load all units from the SerializedObjects
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
        private BaseUnit GetRandomUnitFromPool()
        {
            int totalChance = rarityChances.Values.Sum();  // Sum up all the rarity chances
            int randomValue = UnityEngine.Random.Range(0, totalChance);  // Get a random value in the total chance range
            int cumulativeChance = 0;

            // Loop through all units in the UnitContext's allUnits list and check their rarity
            foreach (var unit in UnitContext.allUnits)
            {
                int unitRarity = unit.rarity;  // Get the rarity of the unit
                cumulativeChance += GetUnitRarityChance(unitRarity);  // Add the chance for that rarity

                // If the random value is within the cumulative range, we have our unit
                if (randomValue < cumulativeChance)
                {
                    return unit;
                }
            }

            return null;  // Fallback if something goes wrong (should not reach here)
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
