// HealthBar.cs (Definitive, Robust Version)
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider healthSlider;

    void Awake()
    {
        // --- THIS IS THE FIX ---
        // GetComponentInChildren will search this object AND all of its children
        // to find the Slider component. This is much safer.
        healthSlider = GetComponentInChildren<Slider>();

        // This new error message will tell you immediately if your prefab is set up incorrectly.
        if (healthSlider == null)
        {
            Debug.LogError($"FATAL ERROR: The HealthBar script on '{gameObject.name}' could not find a Slider component on itself or any of its children. Health will not be displayed.");
        }
    }

    /// <summary>
    /// Sets the maximum value of the health bar and initializes it to be full.
    /// </summary>
    public void SetMaxHealth(int maxHp)
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = maxHp;
        healthSlider.value = maxHp;
    }

    /// <summary>
    /// Updates the health bar to a new current value.
    /// </summary>
    public void SetHealth(int currentHp)
    {
        if (healthSlider == null) return;
        
        healthSlider.value = currentHp;
    }
}