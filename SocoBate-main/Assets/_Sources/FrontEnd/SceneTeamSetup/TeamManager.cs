using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Models;
using TMPro;
using Database;

public class TeamManager : MonoBehaviour
{
    public GameObject ownedUnitRowPrefab;  // Reference to the OwnedUnitRow prefab
    public Transform scrollViewContent;    // The Content section of the ScrollView (where rows will be instantiated)
    public List<GameObject> hexes;         // List of hexes where the unit can be spawned
    public GameObject unitPrefab;          // The unit prefab to spawn

    private bool isRowSelected = false;    // Flag to check if the row is selected
    private HashSet<string> selectedUnitIds = new HashSet<string>(); // To track which units have already been selected

    void Start()
    {
        // Debug: Check if ownedUnits is populated
        Debug.Log("Owned units count: " + Context.UnitContext.ownedUnits.Count);

        // Check if there are any units to display
        if (Context.UnitContext.ownedUnits.Count > 0)
        {
            // Instantiate the OwnedUnitRows based on the ownedUnits list
            GenerateUnitRows();
        }
        else
        {
            Debug.Log("No units available in the ownedUnits list.");
        }
    }

    // Function to generate unit rows based on the ownedUnits list
    public void GenerateUnitRows()
    {
        // Debug: Log when GenerateUnitRows is called
        Debug.Log("GenerateUnitRows called.");

        // Clear any existing rows in the scroll view content
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Debug: Log how many units are in the ownedUnits list
        Debug.Log("Number of owned units: " + Context.UnitContext.ownedUnits.Count);

        // Loop through ownedUnits and instantiate a row for each unit
        foreach (OwnedUnits unit in Context.UnitContext.ownedUnits)
        {
            // Debug: Log each unit being processed
            Debug.Log("Processing unit: " + unit.unitId);

            // Instantiate the row inside the scroll view content
            GameObject ownedUnitRow = Instantiate(ownedUnitRowPrefab, scrollViewContent);
            Debug.Log("OwnedUnitRow instantiated: " + ownedUnitRow.name);

            // Find the Text component in the row to display the unit's name
            TextMeshProUGUI unitNameText = ownedUnitRow.transform.Find("Nome").GetComponent<TextMeshProUGUI>();

            if (unitNameText != null)
            {
                unitNameText.text = unit.unitId; // Assuming `unitId` is the unit's name or identifier
            }
            else
            {
                Debug.LogError("UnitNameText not found in prefab!");
            }

            // Now, find the button inside the row and add the listener for clicking
            Button selectButton = ownedUnitRow.GetComponentInChildren<Button>();
            if (selectButton != null)
            {
                // Add listener and pass the unitId to track the selection
                selectButton.onClick.AddListener(() => OnRowButtonClicked(unit.unitId, ownedUnitRow));
            }
            else
            {
                Debug.LogError("Button not found in ownedUnitRow prefab.");
            }
        }
    }

    // Function that gets called when the RowButton inside OwnedUnitRow is clicked
    public void OnRowButtonClicked(string unitId, GameObject row)
    {
        // Check if the unit has already been selected
        if (selectedUnitIds.Contains(unitId))
        {
            Debug.Log("Unit already selected. Cannot select again.");
            return;
        }

        // Mark the unit as selected
        selectedUnitIds.Add(unitId);

        Debug.Log($"Row button clicked. Now selecting hex for unit: {unitId}");
        isRowSelected = true;  // Activate hex selection
        ActivateHexesForSelection();

        // Optionally, disable the button to prevent further selections of this unit
        Button selectButton = row.GetComponentInChildren<Button>();
        if (selectButton != null)
        {
            selectButton.interactable = false; // Disable the button to indicate it's already selected
        }
    }

    // Function to activate the hexes for selection
    public void ActivateHexesForSelection()
    {
        foreach (GameObject hex in hexes)
        {
            // Enable the buttons on hexes to be clickable
            HexButton hexButton = hex.GetComponentInChildren<HexButton>();
            if (hexButton != null)
            {
                hexButton.SetButtonInteractable(true);
            }
        }
    }

    // Function to spawn a unit on the selected hex
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

        // Check if there is already a unit inside the hex
        if (selectedHex.transform.childCount > 0)
        {
            Debug.LogError("Hex already contains a unit.");
            return;
        }

        // Spawn the unit at the selected hex position slightly higher
        Vector3 spawnPosition = selectedHex.transform.position + new Vector3(0, 50.0f, 0);
        GameObject spawnedUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);

        // Set global scale to 1,1,1 to avoid shrinking due to hierarchy scaling
        spawnedUnit.transform.localScale = new Vector3(1f, 1f, 1f);

        // Now, parent it to the hex to keep hierarchy
        spawnedUnit.transform.SetParent(selectedHex.transform, true);

        Debug.Log("Unit spawned on hex: " + selectedHex.name);
        isRowSelected = false;  // Reset the row selection

        // Add a Button component to the spawned unit and set up the remove unit listener
        Button unitButton = spawnedUnit.AddComponent<Button>();
        unitButton.onClick.AddListener(() => RemoveUnitFromHex(spawnedUnit)); // Remove the unit on click
    }

    // Function to remove the unit from the hex (called when right-clicking on the unit)
    public void RemoveUnitFromHex(GameObject unit)
    {
        if (unit == null)
        {
            Debug.LogError("No unit to remove.");
            return;
        }

        // Destroy the unit
        Destroy(unit);
        Debug.Log("Unit removed from hex: " + unit.name);
    }

public void OnSaveClicked()
{
    // Create a list to hold the team setup (HexId, UnitName) pairs
    List<(int HexId, string UnitName)> teamSetup = new List<(int, string)>();

    // Loop through each hex in the hexes list
    for (int i = 0; i < hexes.Count; i++)
    {
        GameObject hex = hexes[i];

        // Check if the hex contains a unit (meaning the hex has a child)
        if (hex.transform.childCount > 0)
        {
            // Assuming that the unit's name is stored in the first child object of the hex
            string unitName = hex.transform.GetChild(0).name;  // You can replace this with actual logic for fetching unit name

            // Add the HexId (index of the hex) and the UnitName to the team setup list
            teamSetup.Add((i, unitName)); // `i` is the HexId, `unitName` is the name of the unit
        }
    }

    // Ensure that you have a valid AccountId (You should already have the account info available)
        

    // Call the SaveTeam method to save the team layout to the database
    bool success = TeamController.SaveTeam(Context.UserContext.account.AccountId, teamSetup).Result;  // Wait for the save operation to complete

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
