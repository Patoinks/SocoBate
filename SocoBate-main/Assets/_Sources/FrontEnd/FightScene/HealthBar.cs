using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Assign in Inspector
    public Image healthFill; // Assign in Inspector
    
    private int maxHealth;

    public void SetHealth(int currentHp)
    {
        healthSlider.value = (float)currentHp / maxHealth;
    }

    public int SetMaxHealth(int maxHp)
    {
        maxHealth = maxHp;
        return maxHp;
    }
}
