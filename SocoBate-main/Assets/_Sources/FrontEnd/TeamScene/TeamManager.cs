using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Models;
using TMPro;
using Context;
using Database;

public class TeamManager : MonoBehaviour
{
    public GameObject ownedUnitRowPrefab;
    public Transform scrollViewContent;
    public List<GameObject> hexes;

    private bool isRowSelected = false;
    private List<string> selectedUnitIds = new List<string>();
    private int unitsPlaced = 0; // Track the number of units placed

    void Start()
    {
        Debug.Log("Owned units count: " + Context.UnitContext.ownedUnits.Count);

        if (Context.UnitContext.ownedUnits.Count > 0)
        {
            GenerateUnitRows();
        }
        else
        {
            Debug.Log("No units available in the ownedUnits list.");
        }

        Debug.Log("Hexes in the scene:");
        foreach (GameObject hex in hexes)
        {
            Debug.Log(hex.name);
        }
    }

    public void GenerateUnitRows()
    {
        Debug.Log("Generating unit rows: Owned Units vs. Team Setup...");

        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        List<OwnedUnits> ownedUnits = UnitContext.ownedUnits;
        List<TeamSetup> teamUnits = TeamContext.GetPlayerTeam();

        if (ownedUnits == null || ownedUnits.Count == 0)
        {
            Debug.Log("No units owned by the player.");
            return;
        }

        Debug.Log($"Total owned units: {ownedUnits.Count}");

        foreach (OwnedUnits unit in ownedUnits)
        {
            GameObject row = Instantiate(ownedUnitRowPrefab, scrollViewContent);

            if (row == null)
            {
                Debug.LogError("Failed to instantiate ownedUnitRowPrefab!");
                continue;
            }

            TMP_Text unitNameText = row.transform.Find("Nome")?.GetComponent<TMP_Text>();
            Button selectButton = row.transform.Find("SelectUnit")?.GetComponent<Button>();
            RawImage splashImage = row.transform.Find("Splash")?.GetComponent<RawImage>();

            if (unitNameText != null)
            {
                unitNameText.text = unit.unitId;
            }
            else
            {
                Debug.LogError("Unit name text not found in prefab!");
            }

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => OnRowButtonClicked(unit.unitId, row));
            }
            else
            {
                Debug.LogError($"Select button not found for unit {unit.unitId}.");
            }

            if (splashImage != null)
            {
                string splashImageName = unit.unitId + "Splash";
                Texture2D loadedTexture = Resources.Load<Texture2D>($"Sprites/SplashUnits/{splashImageName}");
                if (loadedTexture != null)
                {
                    splashImage.texture = loadedTexture;
                }
                else
                {
                    Debug.LogError($"Splash image not found for {splashImageName}.");
                }
            }
        }

        Debug.Log("Finished generating all unit rows.");
    }

    public void OnRowButtonClicked(string unitId, GameObject row)
    {
        Debug.Log($"Row button clicked for unit: {unitId}");

        string prefabName = unitId.EndsWith(".asset") ? unitId.Substring(0, unitId.Length - 6) : unitId;

        GameObject unitPrefab = Resources.Load<GameObject>("UnitsPrefabs/" + prefabName);

        if (unitPrefab == null)
        {
            Debug.LogError($"Prefab for unit {unitId} not found in Resources/UnitsPrefabs/");
            return;
        }

        Debug.Log($"Prefab for unit {unitId} loaded successfully.");

        if (selectedUnitIds.Contains(unitId))
        {
            Debug.Log("Unit already selected. Cannot select again.");
            return;
        }

        if (unitsPlaced >= 5)
        {
            Debug.Log("Maximum of 5 units already placed. Cannot place more units.");
            return;
        }

        foreach (GameObject hex in hexes)
        {
            if (hex.transform.childCount > 0)
            {
                Transform child = hex.transform.GetChild(0);
                string placedUnitName = child.name.Replace("(Clone)", "").Trim();

                if (placedUnitName == unitId)
                {
                    Debug.Log("This unit is already placed in a hex. Cannot select again.");
                    return;
                }
            }
        }

        selectedUnitIds.Add(unitId);
        Debug.Log($"Now selecting hex for unit: {unitId}");
        isRowSelected = true;
        ActivateHexesForSelection();

        Button selectButton = row.GetComponentInChildren<Button>();
        if (selectButton != null)
        {
            selectButton.interactable = false;
            Debug.Log($"Select button for {unitId} disabled.");
        }
    }

    public void ActivateHexesForSelection()
    {
        Debug.Log("Activating hexes for selection...");
        foreach (GameObject hex in hexes)
        {
            HexButton hexButton = hex.GetComponentInChildren<HexButton>();
            if (hexButton != null)
            {
                hexButton.SetButtonInteractable(true);
            }
        }
    }

    public void SpawnUnitOnHex(GameObject selectedHex)
    {
        if (!isRowSelected || selectedHex == null)
        {
            Debug.LogError("No hex selected or row not selected.");
            return;
        }

        string unitId = selectedUnitIds[selectedUnitIds.Count - 1];
        string prefabName = unitId.EndsWith(".asset") ? unitId.Substring(0, unitId.Length - 6) : unitId;

        GameObject unitPrefab = Resources.Load<GameObject>("UnitsPrefabs/" + prefabName);

        if (unitPrefab == null)
        {
            Debug.LogError($"Prefab for unit {unitId} not found in Resources/Units/");
            return;
        }

        if (selectedHex.transform.childCount > 0)
        {
            Debug.LogError($"Hex {selectedHex.name} already contains a unit. Cannot place {unitId}.");
            return;
        }

        Vector3 spawnPosition = selectedHex.transform.position + new Vector3(0, 50.0f, 0);
        GameObject spawnedUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity, selectedHex.transform);

        spawnedUnit.transform.localPosition = Vector3.zero;
        spawnedUnit.transform.localScale = Vector3.one;

        Debug.Log($"Unit {unitId} spawned on hex: {selectedHex.name}");

        unitsPlaced++; // Increment the count of units placed
        isRowSelected = false;

        // Add listener for the remove button
        Button removeButton = spawnedUnit.GetComponentInChildren<Button>();
        if (removeButton != null)
        {
            removeButton.onClick.RemoveAllListeners();
            removeButton.onClick.AddListener(() => RemoveUnitFromHex(selectedHex));
        }
        else
        {
            Debug.LogError("Remove button not found in the unit prefab.");
        }
    }

    public void RemoveUnitFromHex(GameObject hex)
    {
        if (hex == null)
        {
            Debug.LogError("Hex is null, cannot remove unit.");
            return;
        }

        if (hex.transform.childCount == 0)
        {
            Debug.Log("No unit found on this hex.");
            return;
        }

        // Get the unit GameObject (assumes unit is the first child of the hex)
        GameObject unit = hex.transform.GetChild(0).gameObject;
        if (unit == null)
        {
            Debug.LogError("Unit GameObject is null.");
            return;
        }

        // Get the unit name without "(Clone)"
        string unitName = unit.name.Replace("(Clone)", "").Trim();

        // Log for debugging
        Debug.Log($"Removing unit {unitName} from hex {hex.name}");

        // Remove from selected list
        if (selectedUnitIds.Contains(unitName))
        {
            selectedUnitIds.Remove(unitName);
        }

        // Enable the corresponding row button
        EnableRowButton(unitName);

        // Destroy the unit object
        Destroy(unit);
        Debug.Log($"Unit {unitName} removed from hex {hex.name}.");

        unitsPlaced--; // Decrease the count when a unit is removed
    }

    private void EnableRowButton(string unitId)
    {
        foreach (Transform child in scrollViewContent)
        {
            TMP_Text unitNameText = child.Find("Nome").GetComponent<TMP_Text>();
            if (unitNameText != null && unitNameText.text == unitId)
            {
                Button selectButton = child.GetComponentInChildren<Button>();
                if (selectButton != null)
                {
                    selectButton.interactable = true;
                    Debug.Log($"Selection button for {unitId} re-enabled.");
                }
                return;
            }
        }
    }
}
