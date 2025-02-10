using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeamSetup : MonoBehaviour
{
    public GameObject teamSetupPanel; 
 /*   public Button[] hexButtons;       // Buttons representing hexes (9 hexes)
    public GameObject unitButtonPrefab; // Prefab for each unit in the list
    public Transform unitListContainer; // Container for the unit list (ScrollView content)
    public Text statusText; // Display text for feedback

    private List<int> playerUnits;     // List of units the player owns (example IDs)
    private List<int> occupiedHexes;   // List of occupied hexes
    private int selectedUnitId = -1;   // Track the currently selected unit ID

    private void Start()
    {
        // Example: Player's unit IDs
        playerUnits = new List<int> { 101, 102, 103, 104, 105 };  // Example unit IDs
        occupiedHexes = new List<int>();  // Initially, no hexes are occupied

        // Create buttons for each unit in the list
        foreach (int unitId in playerUnits)
        {
            CreateUnitButton(unitId);
        }

        // Add OnClick event listeners for hex buttons
        for (int i = 0; i < hexButtons.Length; i++)
        {
            int hexIndex = i;  // Capture the index for the lambda expression
            hexButtons[i].onClick.AddListener(() => OnHexClick(hexIndex));
        }
    }

    // Create a button for each unit in the list
    private void CreateUnitButton(int unitId)
    {
        // Instantiate the unit button and set its properties
//        GameObject unitButton = Instantiate(unitButtonPrefab, unitListContainer);
        unitButton.GetComponentInChildren<Text>().text = "Unit " + unitId; // Display unit ID as text
        unitButton.GetComponent<Button>().onClick.AddListener(() => OnUnitSelect(unitId));
    }

    // Handle when a unit is selected from the list
    public void OnUnitSelect(int unitId)
    {
        selectedUnitId = unitId; // Set the selected unit ID
        statusText.text = $"Selected Unit {unitId}. Now, tap a hex to place it.";
    }

    // Handle when a hex is clicked
    public void OnHexClick(int hexIndex)
    {
        // Check if the hex is already occupied
        if (occupiedHexes.Contains(hexIndex))
        {
            statusText.text = "This hex is already occupied. Please choose another one.";
        }
        else if (selectedUnitId == -1)
        {
            statusText.text = "Please select a unit first.";
        }
        else
        {
            // Place the selected unit in the chosen hex
            occupiedHexes.Add(hexIndex);
            statusText.text = $"Unit {selectedUnitId} placed in hex {hexIndex}.";
            // Optionally update the hex button text or icon to reflect the placed unit
            hexButtons[hexIndex].GetComponentInChildren<Text>().text = "Unit " + selectedUnitId;

            // Reset selected unit after placement
            selectedUnitId = -1;
        }
    }
*/

    // Update is called once per frame
    void Update()
    {
        // Listen for the ESC key to destroy the current unit prefab
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (teamSetupPanel != null)
            {
                Destroy(teamSetupPanel);  // Destroy the prefab
                teamSetupPanel = null;  // Reset the reference
            }
        }
    }

}
