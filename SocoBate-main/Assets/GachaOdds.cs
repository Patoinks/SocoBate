using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Context;
public class GachaOdds : MonoBehaviour
{

    public TMP_Text rarity3Text;
    public TMP_Text rarity4Text;
    public TMP_Text rarity5Text;

    // Reference to the scroll views for each rarity
    public Transform scrollView3Rarity;
    public Transform scrollView4Rarity;
    public Transform scrollView5Rarity;

    // Prefab to instantiate for each unit's splash art
    public GameObject unitSplashPrefab;

    // Reference to GachaManagerCS to get units
    private void Start()
    {
        PopulateScrollViews();
        PopulateRarityOdds();
    }

    private void PopulateRarityOdds()
    {
        GachaManagerCS gachaManager = FindObjectOfType<GachaManagerCS>();
        if (gachaManager == null)
        {
            Debug.LogError("GachaManagerCS not found!");
            return;
        }

        Dictionary<int, int> rarityChances = new Dictionary<int, int>(gachaManager.GetRarityChances());

        int totalWeight = 0;
        foreach (var weight in rarityChances.Values)
        {
            totalWeight += weight;
        }

        // Calculate and set odds text for each rarity
        if (rarityChances.ContainsKey(3))
            rarity3Text.text = $"3 Rarity: {((float)rarityChances[3] / totalWeight * 100):F2}%";

        if (rarityChances.ContainsKey(4))
            rarity4Text.text = $"4 Rarity: {((float)rarityChances[4] / totalWeight * 100):F2}%";

        if (rarityChances.ContainsKey(5))
            rarity5Text.text = $"5 Rarity: {((float)rarityChances[5] / totalWeight * 100):F2}%";
    }

    private void PopulateScrollViews()
    {
        GachaManagerCS gachaManager = FindObjectOfType<GachaManagerCS>();
        if (gachaManager == null)
        {
            Debug.LogError("GachaManagerCS not found!");
            return;
        }

        // Filter units by rarity
        List<BaseUnit> rarity3Units = new List<BaseUnit>();
        List<BaseUnit> rarity4Units = new List<BaseUnit>();
        List<BaseUnit> rarity5Units = new List<BaseUnit>();

        foreach (var unit in UnitContext.allUnits)
        {
            switch (unit.rarity)
            {
                case 3:
                    rarity3Units.Add(unit);
                    break;
                case 4:
                    rarity4Units.Add(unit);
                    break;
                case 5:
                    rarity5Units.Add(unit);
                    break;
            }
        }

        // Populate each scroll view with units' splash arts
        PopulateScrollView(scrollView3Rarity, rarity3Units);
        PopulateScrollView(scrollView4Rarity, rarity4Units);
        PopulateScrollView(scrollView5Rarity, rarity5Units);
    }

    // Helper method to populate the scroll view with unit splash arts
    private void PopulateScrollView(Transform scrollViewContent, List<BaseUnit> units)
    {
        foreach (BaseUnit unit in units)
        {
            // Instantiate a new unit splash prefab
            GameObject splashArtObject = Instantiate(unitSplashPrefab, scrollViewContent);
            splashArtObject.transform.SetParent(scrollViewContent, false);

            // Get the components in the prefab
            Image splashImage = splashArtObject.GetComponentInChildren<Image>();


            // Load the splash art sprite from Resources/SplashUnits/ folder
            string splashPath = $"Sprites/SplashUnits/{unit.unitName}Splash";
            Sprite splashSprite = Resources.Load<Sprite>(splashPath);

            if (splashSprite != null)
            {
                splashImage.sprite = splashSprite;
            }


        }
    }
}
