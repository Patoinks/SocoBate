using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Models;
using TMPro;
using Context;
using Database;

public class LoadTeam : MonoBehaviour
{
    public List<GameObject> hexes;
    public Transform scrollViewContent;
    private int unitsPlaced = 0; // Track the number of units placed

    void Start()
    {
        SpawnUnitsOnStart();
    }

    private void SpawnUnitsOnStart()
    {
        List<TeamSetup> teamUnits = TeamContext.GetPlayerTeam();

        if (teamUnits == null || teamUnits.Count == 0)
        {
            Debug.LogError("No team setup found in TeamContext.");
            return;
        }

        foreach (var unit in teamUnits)
        {
            string unitId = unit.UnitName;
            int hexId = unit.HexId;

            if (hexId < 1 || hexId > hexes.Count)
            {
                Debug.LogError($"Invalid hexId {hexId} for unit {unitId}");
                continue;
            }

            if (unitsPlaced >= 5) 
            {
                Debug.Log("Maximum of 5 units already placed. Cannot place more units.");
                break;
            }

            GameObject hex = hexes[hexId - 1];

            if (hex == null)
            {
                Debug.LogError($"Hex with id {hexId} not found.");
                continue;
            }

            GameObject unitPrefab = Resources.Load<GameObject>("UnitsPrefabs/" + unitId);

            if (unitPrefab == null)
            {
                Debug.LogError($"Prefab for unit {unitId} not found in Resources/UnitsPrefabs/");
                continue;
            }

            Vector3 spawnPosition = hex.transform.position + new Vector3(0, 50.0f, 0);
            GameObject spawnedUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity, hex.transform);

            spawnedUnit.transform.localPosition = Vector3.zero;
            spawnedUnit.transform.localScale = Vector3.one;

            unitsPlaced++; // Increment the count of units placed

            // Add right-click removal functionality
            Button removeButton = spawnedUnit.GetComponentInChildren<Button>();
            if (removeButton != null)
            {
                removeButton.onClick.RemoveAllListeners();
                removeButton.onClick.AddListener(() => RemoveUnitFromHex(hex));
            }
            else
            {
                Debug.LogError("Remove button not found in the unit prefab.");
            }

            DisableSelectButtonIfPlaced(unitId);

            Debug.Log($"Unit {unitId} spawned on hex {hexId}.");
        }
    }

    private void DisableSelectButtonIfPlaced(string unitId)
    {
        foreach (Transform child in scrollViewContent)
        {
            TMP_Text unitNameText = child.Find("Nome").GetComponent<TMP_Text>();
            if (unitNameText != null && unitNameText.text == unitId)
            {
                Button selectButton = child.GetComponentInChildren<Button>();
                if (selectButton != null)
                {
                    selectButton.interactable = false;
                    Debug.Log($"Select button for {unitId} disabled.");
                }
            }
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
