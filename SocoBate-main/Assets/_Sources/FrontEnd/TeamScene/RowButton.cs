using UnityEngine;
using UnityEngine.UI;

public class RowButton : MonoBehaviour
{
    private TeamManager teamManager;  // Reference to TeamManager
    public string unitId;            // The unit's ID (make sure you set this in the Unity inspector or via code)
    public GameObject row;           // The row object representing the unit (set this as well)

    void Start()
    {
        // Get the TeamManager reference
        teamManager = FindObjectOfType<TeamManager>();

        // Get the Button component and add listener for when the RowButton is clicked
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnRowButtonClicked);
        }
    }

    // This method is triggered when the RowButton is clicked
    void OnRowButtonClicked()
    {
        // Pass the unitId and row when selecting the unit
        if (teamManager != null)
        {
            teamManager.OnRowButtonClicked(unitId, row);  // This triggers hex selection and tracks the unitId
        }
    }
}
