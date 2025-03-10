using System.Collections;
using UnityEngine;
using Context;
using System.Collections.Generic;
using Models;
using UnityEngine.UI;

public class SquadManager : MonoBehaviour
{
    public GameObject[] playerHexes;  // Assign in Unity Editor
    public GameObject[] enemyHexes;   // Assign in Unity Editor

    private Dictionary<int, Transform> playerHexPositions = new Dictionary<int, Transform>();
    private Dictionary<int, Transform> enemyHexPositions = new Dictionary<int, Transform>();

    public GameObject statsPanelPrefab; // Assign in Unity Editor
    public Canvas uiCanvas; // Assign in Unity Editor
    public HealthBar healthbar;
    public GameObject healthBarPrefab;
    public List<BaseUnit> playerUnits = new List<BaseUnit>(); // Add this to hold player units for the battle
    public List<BaseUnit> enemyUnits = new List<BaseUnit>();  // Add this to hold enemy units for the battle
    private Dictionary<BaseUnit, GameObject> unitPrefabs = new Dictionary<BaseUnit, GameObject>(); // Store unit prefabs
    private Dictionary<HealthBar, GameObject> healthbarPrefabs = new Dictionary<HealthBar, GameObject>();
    public Dictionary<BaseUnit, HealthBar> unitHealthBars = new Dictionary<BaseUnit, HealthBar>();
    void Start()
    {
        InitializeHexPositions();
        SpawnSquad(TeamContext.GetPlayerTeam(), playerHexPositions, false);
        SpawnSquad(TeamContext.GetEnemyTeam(), enemyHexPositions, true);

        // Link the player and enemy units to the DuelScript (assuming the DuelScript is on a GameObject)
        var duelScript = FindObjectOfType<DuelScript>();
        if (duelScript != null)
        {
            duelScript.playerUnits = playerUnits;
            duelScript.enemyUnits = enemyUnits;
        }
        else
        {
            Debug.LogError("DuelScript not found in the scene.");
        }
    }

    private void InitializeHexPositions()
    {
        for (int i = 0; i < playerHexes.Length; i++)
        {
            playerHexPositions[i + 1] = playerHexes[i].transform;
        }
        for (int i = 0; i < enemyHexes.Length; i++)
        {
            enemyHexPositions[i + 1] = enemyHexes[i].transform;
        }
    }

    private void SpawnSquad(List<TeamSetup> team, Dictionary<int, Transform> hexPositions, bool isEnemy)
    {
        foreach (var unit in team)
        {
            if (!hexPositions.ContainsKey(unit.HexId))
            {
                Debug.LogError($"Invalid HexID {unit.HexId} for unit {unit.UnitName}");
                continue;
            }

            // Find the unit prefab based on the unit's name
            GameObject unitPrefab = FindUnitPrefab(unit.UnitName);
            if (unitPrefab == null)
            {
                Debug.LogError($"No prefab found for unit {unit.UnitName}");
                continue;
            }

            Transform hexTransform = hexPositions[unit.HexId];
            GameObject spawnedUnit = Instantiate(unitPrefab, hexTransform.position + new Vector3(isEnemy ? 5f : -5f, 58f, 0), Quaternion.identity, hexTransform);

            // Use 'isEnemy' directly to set the unit name
            spawnedUnit.name = unit.UnitName + (isEnemy ? "_Enemy" : "_Player");

            // Instantiate and link health bar
            GameObject healthBar = Instantiate(healthBarPrefab, spawnedUnit.transform.position, Quaternion.identity, spawnedUnit.transform);
            HealthBar healthBarScript = healthBar.GetComponent<HealthBar>();
            healthBarScript.healthSlider = healthBar.GetComponentInChildren<Slider>();

            // Clone and load BaseUnit data
            BaseUnit clonedUnit = LoadBaseUnit(unit.UnitName, unit.HexId);

            if (clonedUnit != null)
            {
                unitHealthBars[clonedUnit] = healthBarScript;

                // Set max health for the unit
                healthBarScript.SetMaxHealth(clonedUnit.baseHp);

                // Add the unit to the correct list (player or enemy)
                if (isEnemy)
                {
                    enemyUnits.Add(clonedUnit);
                    Debug.Log($"Added enemy unit: {clonedUnit.name}");
                }
                else
                {
                    playerUnits.Add(clonedUnit);
                    Debug.Log($"Added player unit: {clonedUnit.name}");
                }
            }

            // Rotate enemy units
            if (isEnemy)
            {
                spawnedUnit.transform.rotation = Quaternion.Euler(0f, -180f, 0f);
            }

            // Add a button to spawn unit on click
            AddButtonToUnit(spawnedUnit, clonedUnit);
        }
    }

    private void AddButtonToUnit(GameObject unitPrefab, BaseUnit clonedUnit)
    {
        // Assuming each unit prefab has a Button component to trigger the action
        Button unitButton = unitPrefab.GetComponentInChildren<Button>();

        if (unitButton != null)
        {
            unitButton.onClick.AddListener(() => OnUnitButtonClicked(clonedUnit, unitPrefab));
        }
        else
        {
            Debug.LogError("No Button component found on unit prefab.");
        }
    }

    private GameObject statsPanel; // Reference to the UI panel
    private BaseUnit currentUnit;  // The current unit whose stats are displayed

    // When the unit button is clicked
    private void OnUnitButtonClicked(BaseUnit unit, GameObject unitPrefab)
    {
        // If statsPanel doesn't exist, instantiate it once
        if (statsPanel == null)
        {
            statsPanel = Instantiate(statsPanelPrefab, uiCanvas.transform);  // Parent directly to canvas

            // Get the RectTransform of the statsPanel and position it in the top-right
            RectTransform rectTransform = statsPanel.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                // Set the anchor to top-right
                rectTransform.anchorMin = new Vector2(1f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.pivot = new Vector2(1f, 1f);

                // Set the position relative to the canvas, with padding if needed (e.g., 10 pixels from top-right corner)
                rectTransform.anchoredPosition = new Vector2(-490f, -347f);  // Adjust as needed
            }
            else
            {
                Debug.LogError("RectTransform is missing on the statsPanelPrefab.");
            }
        }

        // Set the current unit
        currentUnit = unit;

        // Initialize stats in the panel initially
        UpdateStatsPanel(currentUnit);
    }

    // Method to update the stats in the UI panel
    private void UpdateStatsPanel(BaseUnit unit)
    {
        Debug.Log($"Updating stats for {unit.unitName}");

        // Find the necessary text components
        Text hpText = statsPanel.transform.Find("HP").GetComponent<Text>();
        Text strText = statsPanel.transform.Find("STR").GetComponent<Text>();
        Text defText = statsPanel.transform.Find("MDEF").GetComponent<Text>();
        Text nameText = statsPanel.transform.Find("NAME").GetComponent<Text>();
        Text speedText = statsPanel.transform.Find("SPEED").GetComponent<Text>();
        Text pDefText = statsPanel.transform.Find("PDEF").GetComponent<Text>();
        Text intText = statsPanel.transform.Find("INT").GetComponent<Text>();
        Text auraText = statsPanel.transform.Find("AURA").GetComponent<Text>();

        // Update the stats text dynamically
        if (hpText != null)
        {
            hpText.text = "HP: " + unit.baseHp.ToString() + "/" + unit.maxHp.ToString();
        }
        if (strText != null)
        {
            strText.text = "STR: " + unit.baseStr.ToString();
        }
        if (defText != null)
        {
            defText.text = "MDEF: " + unit.mDef.ToString();
        }
        if (nameText != null)
        {
            nameText.text = "NAME: " + unit.unitName;
        }
        if (speedText != null)
        {
            speedText.text = "SPD: " + unit.baseSpeed.ToString();
        }
        if (pDefText != null)
        {
            pDefText.text = "PDEF: " + unit.pDef.ToString();
        }
        if (intText != null)
        {
            intText.text = "INT: " + unit.baseInt.ToString();
        }
        if (auraText != null)
        {
            auraText.text = "AURA: " + unit.aura.ToString();
        }
    }


    // Update method to check stats and update panel dynamically
    private void Update()
    {
        // Check if the current unit exists and its stats have changed
        if (currentUnit != null)
        {
            // If the unit's stats have changed, update the panel
            UpdateStatsPanel(currentUnit);
        }
    }





    private GameObject FindUnitPrefab(string unitName)
    {
        // Load prefab from Resources/UnitsPrefabs/
        GameObject loadedPrefab = Resources.Load<GameObject>($"UnitsPrefabs/{unitName}");

        if (loadedPrefab == null)
        {
            Debug.LogError($"Prefab '{unitName}' not found in Resources/UnitsPrefabs/.");
        }

        return loadedPrefab;
    }

    private BaseUnit LoadBaseUnit(string unitName, int hexId)
    {
        // Load the original BaseUnit from Resources
        BaseUnit originalUnit = Resources.Load<BaseUnit>($"Units/UnitData/{unitName}");

        if (originalUnit == null)
        {
            Debug.LogError($"BaseUnit '{unitName}' not found in Resources/Units/UnitData/.");
            return null;
        }

        // Clone to ensure it's a fresh instance
        BaseUnit clonedUnit = Instantiate(originalUnit);
        clonedUnit.HexId = hexId; // Assign the HexId to the unit

        return clonedUnit;
    }

}
