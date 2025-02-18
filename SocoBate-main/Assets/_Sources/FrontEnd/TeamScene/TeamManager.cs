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
    public GameObject unitPrefab;

    private bool isRowSelected = false;
    private HashSet<string> selectedUnitIds = new HashSet<string>();

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
    }
    public void GenerateUnitRows()
    {
        Debug.Log("Generating unit rows: Owned Units vs. Team Setup...");

        // Clear existing UI rows before generating new ones
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Get lists
        List<OwnedUnits> ownedUnits = UnitContext.ownedUnits; // Player's owned units
        List<TeamSetup> teamUnits = TeamContext.GetPlayerTeam(); // Units currently placed

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

            // Find UI elements within the row
            TMP_Text unitNameText = row.transform.Find("Nome")?.GetComponent<TMP_Text>();
            Button selectButton = row.transform.Find("SelectUnit")?.GetComponent<Button>();

            // Set data
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
                selectButton.onClick.RemoveAllListeners(); // Avoid duplicate listeners
                selectButton.onClick.AddListener(() => OnRowButtonClicked(unit.unitId, row));
            }
            else
            {
                Debug.LogError($"Select button not found for unit {unit.unitId}.");
            }
        }

        Debug.Log("Finished generating all unit rows.");
    }


    public void OnRowButtonClicked(string unitId, GameObject row)
    {
        // Check if the unit is already selected
        if (selectedUnitIds.Contains(unitId))
        {
            Debug.Log("Unit already selected. Cannot select again.");
            return;
        }

        // Check if the unit is already placed in any hex
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

        // Mark the unit as selected
        selectedUnitIds.Add(unitId);
        Debug.Log($"Row button clicked. Now selecting hex for unit: {unitId}");
        isRowSelected = true;  // Activate hex selection
        ActivateHexesForSelection();

        // Disable button to prevent further selections
        Button selectButton = row.GetComponentInChildren<Button>();
        if (selectButton != null)
        {
            selectButton.interactable = false;
        }
    }


    public void ActivateHexesForSelection()
    {
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

        if (unitPrefab == null)
        {
            Debug.LogError("Unit prefab not assigned in TeamManager.");
            return;
        }

        // Check if the hex already contains a unit
        if (selectedHex.transform.childCount > 0)
        {
            Debug.LogError("Hex already contains a unit.");
            return;
        }

        // Spawn the unit slightly above the hex
        Vector3 spawnPosition = selectedHex.transform.position + new Vector3(0, 50.0f, 0);
        GameObject spawnedUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity, selectedHex.transform);

        // Reset local position & scale to avoid unexpected offsets
        spawnedUnit.transform.localPosition = Vector3.zero;
        spawnedUnit.transform.localScale = Vector3.one;

        Debug.Log($"Unit spawned on hex: {selectedHex.name}");

        isRowSelected = false; // Reset row selection

        // Ensure the unit has a Button component and a click event
        Button unitButton = spawnedUnit.GetComponent<Button>();
        if (unitButton == null)
        {
            unitButton = spawnedUnit.AddComponent<Button>();
        }

        // Ensure the button triggers removal
        unitButton.onClick.RemoveAllListeners(); // Remove existing listeners to prevent duplicates
        unitButton.onClick.AddListener(() => RemoveUnitFromHex(spawnedUnit));
    }


    public void RemoveUnitFromHex(GameObject unit)
    {
        if (unit == null)
        {
            Debug.LogError("No unit to remove.");
            return;
        }

        // Get the unit name without the "(Clone)" suffix
        string unitName = unit.name.Replace("(Clone)", "").Trim();

        // Remove from selected list
        if (selectedUnitIds.Contains(unitName))
        {
            selectedUnitIds.Remove(unitName);
        }

        // Enable the corresponding row button
        EnableRowButton(unitName);

        // Destroy the unit
        Destroy(unit);
        Debug.Log($"Unit {unitName} removed from hex.");
    }

    private void EnableRowButton(string unitId)
    {
        foreach (Transform child in scrollViewContent)
        {
            TextMeshProUGUI unitNameText = child.Find("Nome").GetComponent<TextMeshProUGUI>();
            if (unitNameText != null && unitNameText.text == unitId)
            {
                Button selectButton = child.GetComponentInChildren<Button>();
                if (selectButton != null)
                {
                    selectButton.interactable = true; // Reactivate button
                    Debug.Log($"Selection button for {unitId} re-enabled.");
                }
                return;
            }
        }
    }



    public async void OnSaveClicked()
    {
        Debug.Log("OnSaveClicked called.");

        List<(int HexId, string UnitName)> teamSetup = new List<(int, string)>();

        for (int i = 0; i < hexes.Count; i++)
        {
            GameObject hex = hexes[i];
            Debug.Log($"Checking hex {i} with {hex.transform.childCount} children.");

            if (hex.transform.childCount > 0)
            {
                Transform child = hex.transform.GetChild(0);
                Debug.Log($"Found child in hex {i}: {child.name}");

                string unitName = child.name.Replace("(Clone)", "").Trim();
                Debug.Log($"Processed unit name: {unitName}");

                teamSetup.Add((i + 1, unitName));
            }
        }

        if (teamSetup.Count == 0)
        {
            Debug.LogError("No units placed on hexes. Nothing to save.");
            return;
        }

        Debug.Log("Saving team to database...");

        bool success = await TeamController.SaveTeam(Context.UserContext.account.AccountId, teamSetup);

        if (success)
        {
            Debug.Log("Team layout successfully saved!");
        }
        else
        {
            Debug.LogError("Failed to save team layout.");
        }
    }
}
