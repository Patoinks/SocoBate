using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Assign in Inspector
    public Image healthFill; // Assign in Inspector
    public GameObject healthBarPrefab; // Assign in Inspector

    private int maxHealth;
    private GameObject instantiatedHealthBar;

    public void Initialize(int maxHp, Transform unitTransform)
    {
        Debug.Log("Initializing HealthBar");

        // Check if healthBarPrefab is assigned
        if (healthBarPrefab == null)
        {
            Debug.LogError("Health Bar Prefab is not assigned in the Inspector!");
            return;
        }

        // Check the unitTransform to make sure it's valid
        if (unitTransform == null)
        {
            Debug.LogError("Unit Transform is null. Unable to instantiate health bar.");
            return;
        }

        maxHealth = maxHp;
        Debug.Log($"Max Health set to: {maxHealth}");

        // Check the spawn position
        Vector3 spawnPosition = unitTransform.position + new Vector3(0, 2f, 0);
        Debug.Log($"Health Bar spawn position: {spawnPosition}");

        instantiatedHealthBar = Instantiate(healthBarPrefab, spawnPosition, Quaternion.identity, unitTransform);

        if (instantiatedHealthBar == null)
        {
            Debug.LogError("Failed to instantiate Health Bar!");
            return;
        }

        Debug.Log("Health Bar instantiated successfully.");

        // Check if the Slider is assigned
        if (healthSlider == null)
        {
            Debug.LogError("HealthSlider is not assigned in the Inspector!");
            return;
        }

        healthFill = healthSlider.fillRect.GetComponent<Image>();
        if (healthFill == null)
        {
            Debug.LogError("Failed to find Image component in healthSlider's fillRect.");
            return;
        }

        healthSlider.value = maxHealth;
        Debug.Log("Health Slider initialized with max health.");
    }

    public void UpdateHealth(int currentHp)
    {
        Debug.Log($"Updating Health. Current HP: {currentHp}");

        if (healthSlider != null)
        {
            float healthPercentage = CalculateHealthToSlider(currentHp);
            Debug.Log($"Health percentage: {healthPercentage}");

            healthSlider.value = healthPercentage;
            Debug.Log($"Slider value updated to: {healthSlider.value}");
        }
        else
        {
            Debug.LogError("HealthSlider is null. Cannot update health.");
        }
    }

    public float CalculateHealthToSlider(int currentHp)
    {
        if (maxHealth == 0)
        {
            Debug.LogError("Max health is 0, cannot calculate health percentage.");
            return 0f;
        }

        float percentage = (float)currentHp / (float)maxHealth;
        Debug.Log($"Calculated health percentage: {percentage}");
        return percentage;
    }
}
