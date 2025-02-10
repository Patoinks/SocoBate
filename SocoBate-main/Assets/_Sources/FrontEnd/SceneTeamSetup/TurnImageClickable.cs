using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableUnitImage : MonoBehaviour, IPointerClickHandler
{
    public UnitSelector unitSelector;  // Reference to UnitSelector to call the spawn method

    // Function called when the unit image is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug to show in the console that the image was clicked
        Debug.Log("The unit image was clicked!");

        if (unitSelector != null)
        {
            unitSelector.SpawnUnitInRandomHex();  // Call the method in UnitSelector to spawn the unit
        }
    }
}
