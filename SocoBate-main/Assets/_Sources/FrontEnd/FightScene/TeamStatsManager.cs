using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using TMPro;

public class TeamStatsManager : MonoBehaviour
{
    public ScrollRect playerScrollView;
    public ScrollRect enemyScrollView;

    public GameObject unitRowPrefab;  // Prefab for each unit row
    public Button updateButton;  // Button that updates data
    public TMP_Text displayText;  // Optional: Display the current stat being shown (e.g., Damage Dealt, Damage Taken, Healing)

    private Dictionary<BaseUnit, float> unitDamageStats;  // Damage dealt by units
    private Dictionary<BaseUnit, float> unitDamageTakenStats;  // Damage taken by units
    private Dictionary<BaseUnit, float> unitHealingStats;  // Healing stats of units

    private List<BaseUnit> playerUnits;
    private List<BaseUnit> enemyUnits;

    public DuelScript duelScript;  // Reference to the DuelScript
    private float maxDamageDealt = 0f;  // Used for normalizing damage slider
    private float minDamageDealt = float.MaxValue; // Minimum damage for normalizing slider
    private float maxDamageTaken = 0f;  // Used for normalizing damage taken slider
    private float minDamageTaken = float.MaxValue; // Minimum damage for normalizing slider
    private float maxHealing = 0f;  // Used for normalizing healing slider
    private float minHealing = float.MaxValue; // Minimum healing for normalizing slider

    private string currentStat = "DamageDealt";  // Can be "DamageDealt", "DamageTaken", or "Healing"

    void Start()
    {
        unitDamageStats = new Dictionary<BaseUnit, float>();
        unitDamageTakenStats = new Dictionary<BaseUnit, float>();
        unitHealingStats = new Dictionary<BaseUnit, float>();
        updateButton.onClick.AddListener(UpdateUnitStats);
        FetchUnitsFromDuelScript();
    }

    void FetchUnitsFromDuelScript()
    {
        DuelScript duelScript = FindObjectOfType<DuelScript>();
        if (duelScript != null)
        {
            playerUnits = duelScript.playerUnits;
            enemyUnits = duelScript.enemyUnits;

            Debug.Log($"Fetched Units: Player Count = {playerUnits.Count}, Enemy Count = {enemyUnits.Count}");

            if (playerUnits.Count > 0 || enemyUnits.Count > 0)
            {
                InitializeRows();
            }
            else
            {
                Debug.LogWarning("Units are still empty. Trying again in 0.5 seconds...");
                Invoke(nameof(FetchUnitsFromDuelScript), 0.5f);  // Retry fetching units after a short delay
            }
        }
        else
        {
            Debug.LogError("DuelScript not found!");
        }
    }

    void InitializeRows()
    {
        if (unitRowPrefab == null)
        {
            Debug.LogError("unitRowPrefab is NOT assigned in TeamStatsManager!");
            return;
        }

        Debug.Log($"Initializing Rows: Player Units = {playerUnits.Count}, Enemy Units = {enemyUnits.Count}");

        foreach (BaseUnit unit in playerUnits)
        {
            InstantiateUnitRow(unit, playerScrollView.content);
        }

        foreach (BaseUnit unit in enemyUnits)
        {
            InstantiateUnitRow(unit, enemyScrollView.content);
        }

        StartCoroutine(ForceRefreshUI());
    }

    void InstantiateUnitRow(BaseUnit unit, Transform parent)
    {
        // Instantiate the row
        GameObject row = Instantiate(unitRowPrefab, parent);

        // Set unit name
        row.transform.Find("UnitNameText").GetComponent<TMP_Text>().text = unit.unitName;

        // Set splash image
        RawImage splashImage = row.transform.Find("Splash")?.GetComponent<RawImage>();
        if (splashImage != null)
        {
            string splashImageName = unit.unitName + "Splash";
            Texture2D loadedTexture = Resources.Load<Texture2D>($"Sprites/SplashUnits/{splashImageName}");
            if (loadedTexture != null)
            {
                splashImage.texture = loadedTexture;
            }
            else
            {
                Debug.LogWarning($"Splash image not found for {splashImageName}");
            }
        }
        else
        {
            Debug.LogWarning("Splash image component not found in row prefab.");
        }

        // Set initial damage values and sliders (set to 0)
        row.transform.Find("DamageText").GetComponent<TMP_Text>().text = "0";
        row.transform.Find("DamageSlider").GetComponent<Slider>().value = 0;
    }

    IEnumerator ForceRefreshUI()
    {
        yield return null; // Wait for the next frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerScrollView.content);
        LayoutRebuilder.ForceRebuildLayoutImmediate(enemyScrollView.content);
    }

    // Call this method when the "Update" button is pressed
    void UpdateUnitStats()
    {
        // Update the current stat display
        displayText.text = "Current stat: " + currentStat;

        // Reset the min/max values for the stat
        ResetMinMaxValues();

        // Update the rows with the corresponding stats (DamageDealt, DamageTaken, Healing)
        if (currentStat == "DamageDealt")
        {
            UpdateRows(unitDamageStats, maxDamageDealt, minDamageDealt);
        }
        else if (currentStat == "DamageTaken")
        {
            UpdateRows(unitDamageTakenStats, maxDamageTaken, minDamageTaken);
        }
        else if (currentStat == "Healing")
        {
            UpdateRows(unitHealingStats, maxHealing, minHealing);
        }
    }


    void ResetMinMaxValues()
    {
        // Reset the min and max values for the current stat
        if (currentStat == "DamageDealt")
        {
            minDamageDealt = float.MaxValue;
            maxDamageDealt = 0f;
        }
        else if (currentStat == "DamageTaken")
        {
            minDamageTaken = float.MaxValue;
            maxDamageTaken = 0f;
        }
        else if (currentStat == "Healing")
        {
            minHealing = float.MaxValue;
            maxHealing = 0f;
        }
    }

    void UpdateRows(Dictionary<BaseUnit, float> unitStats, float maxStatValue, float minStatValue)
    {
        foreach (Transform child in playerScrollView.content)
        {
            TMP_Text damageText = child.transform.Find("DamageText").GetComponent<TMP_Text>();
            Slider damageSlider = child.transform.Find("DamageSlider").GetComponent<Slider>();
            TMP_Text unitNameText = child.transform.Find("UnitNameText").GetComponent<TMP_Text>();

            string unitName = unitNameText.text.Replace("(Clone)", "").Trim(); // Remove (Clone)

            // Find the matching unit in playerUnits based on unitName
            BaseUnit matchingUnit = playerUnits.Find(u => u.unitName == unitName);

            if (matchingUnit != null && unitStats.TryGetValue(matchingUnit, out float statValue))
            {
                damageText.text = statValue.ToString();
                damageSlider.minValue = minStatValue;
                damageSlider.maxValue = maxStatValue;
                damageSlider.value = statValue;
            }
            else
            {
                Debug.LogWarning($"No matching stat found for player unit: {unitName}");
            }
        }

        foreach (Transform child in enemyScrollView.content)
        {
            TMP_Text damageText = child.transform.Find("DamageText").GetComponent<TMP_Text>();
            Slider damageSlider = child.transform.Find("DamageSlider").GetComponent<Slider>();
            TMP_Text unitNameText = child.transform.Find("UnitNameText").GetComponent<TMP_Text>();

            string unitName = unitNameText.text.Replace("(Clone)", "").Trim(); // Remove (Clone)

            // Find the matching unit in enemyUnits based on unitName
            BaseUnit matchingUnit = enemyUnits.Find(u => u.unitName == unitName);

            if (matchingUnit != null && unitStats.TryGetValue(matchingUnit, out float statValue))
            {
                damageText.text = statValue.ToString();
                damageSlider.minValue = 1;
                damageSlider.maxValue = 1000;
                damageSlider.value = statValue;
            }
            else
            {
                Debug.LogWarning($"No matching stat found for enemy unit: {unitName}");
            }
        }
    }


    GameObject FindRowForUnit(BaseUnit unit, Transform scrollContent)
    {
        foreach (Transform child in scrollContent)
        {
            string rowUnitName = child.transform.Find("UnitNameText").GetComponent<TMP_Text>().text;
            if (rowUnitName == unit.unitName)
            {
                return child.gameObject;
            }
        }
        Debug.LogWarning($"Row not found for unit: {unit.unitName}");
        return null;
    }

    // Register damage dealt for units
    public void RegisterDamageDealt(BaseUnit unit, float damage)
    {
        if (!unitDamageStats.ContainsKey(unit))
        {
            unitDamageStats[unit] = 0f;
        }
        unitDamageStats[unit] += damage;

        // Update max damage for the unit
        maxDamageDealt = Mathf.Max(maxDamageDealt, unitDamageStats[unit]);
        minDamageDealt = Mathf.Min(minDamageDealt, unitDamageStats[unit]);

        Debug.Log($"Updated Damage for {unit.unitName}: {unitDamageStats[unit]} (Max: {maxDamageDealt})");
    }

    // Register damage taken for units
    public void RegisterDamageTaken(BaseUnit unit, float damage)
    {
        if (!unitDamageTakenStats.ContainsKey(unit))
        {
            unitDamageTakenStats[unit] = 0f;
        }
        unitDamageTakenStats[unit] += damage;
        maxDamageTaken = Mathf.Max(maxDamageTaken, unitDamageTakenStats[unit]);
        minDamageTaken = Mathf.Min(minDamageTaken, unitDamageTakenStats[unit]);

        // Call UpdateUnitStats to refresh the UI immediately
        UpdateUnitStats();
    }

    // Register healing for units
    public void RegisterHealing(BaseUnit unit, float healing)
    {
        if (!unitHealingStats.ContainsKey(unit))
        {
            unitHealingStats[unit] = 0f;
        }
        unitHealingStats[unit] += healing;
        maxHealing = Mathf.Max(maxHealing, unitHealingStats[unit]);
        minHealing = Mathf.Min(minHealing, unitHealingStats[unit]);

        // Call UpdateUnitStats to refresh the UI immediately
        UpdateUnitStats();
    }

    // Change the stat displayed
    public void ChangeStatToDamageDealt()
    {
        currentStat = "DamageDealt";
        UpdateUnitStats();
    }

    public void ChangeStatToDamageTaken()
    {
        currentStat = "DamageTaken";
        UpdateUnitStats();
    }

    public void ChangeStatToHealing()
    {
        currentStat = "Healing";
        UpdateUnitStats();
    }
}
