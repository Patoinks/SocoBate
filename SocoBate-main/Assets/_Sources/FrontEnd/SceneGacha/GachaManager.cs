using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

namespace Gacha
{
    public class GachaManagerCS : MonoBehaviour
    {
        public TextMeshProUGUI resultText;    // For displaying the result (Hero name)
        public GameObject heroSpritePrefab;   // The prefab for displaying the hero sprite (if needed)
        public Transform spawnPoint;          // Point where the hero sprite will spawn

        private List<Hero> heroPool = new List<Hero>();   // Pool of all possible heroes for the gacha

        // Example Rarities
        private Dictionary<string, int> rarityChances = new Dictionary<string, int>
        {
            { "Common", 50 },   // 50% chance
            { "Rare", 30 },     // 30% chance
            { "Epic", 15 },     // 15% chance
            { "Legendary", 5 }  // 5% chance
        };

        // Example: Fill hero pool (this can be expanded with more heroes)
        private void Start()
        {
            heroPool.Add(new Hero(1, "Fire Knight", "Sprite_1", "Common"));
            heroPool.Add(new Hero(2, "Water Mage", "Sprite_2", "Rare"));
            heroPool.Add(new Hero(3, "Earth Titan", "Sprite_3", "Epic"));
            heroPool.Add(new Hero(4, "Wind Assassin", "Sprite_4", "Legendary"));
            // Add more heroes as needed
        }

        // Simulate pulling from the gacha
        public void PullGacha()
        {

            // Get the pulled hero
            Hero pulledHero = GetRandomHeroFromPool();

            // Display results
            ShowResult(pulledHero);
        }

        // Get a random hero based on the rarity chances
        private Hero GetRandomHeroFromPool()
        {
            // Get total chance
            int totalChance = rarityChances.Values.Sum();

            // Generate random number to pick a hero based on chance distribution
            int randomValue = UnityEngine.Random.Range(0, totalChance);
            int cumulativeChance = 0;

            // Find hero based on the chance
            foreach (var hero in heroPool)
            {
                cumulativeChance += GetHeroRarityChance(hero.rarity);

                if (randomValue < cumulativeChance)
                {
                    return hero;
                }
            }

            return null; // Fallback, shouldn't reach here
        }

        // Get the chance value for a given rarity
        private int GetHeroRarityChance(string rarity)
        {
            return rarityChances.ContainsKey(rarity) ? rarityChances[rarity] : 0;
        }

        // Display the result
        private void ShowResult(Hero hero)
        {
            if (hero != null)
            {
                // Display the hero name
                resultText.text = $"You pulled: {hero.name} ({hero.rarity})";
                
                // Optionally, instantiate the hero sprite prefab at a certain position
                GameObject heroSprite = Instantiate(heroSpritePrefab, spawnPoint.position, Quaternion.identity);
                // Assuming you have a sprite setup for each hero sprite reference
                heroSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(hero.spriteId);
            }
            else
            {
                resultText.text = "No hero pulled!";
            }
        }
    }
}
