using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitStatsRow : MonoBehaviour
{
    public TMP_Text unitNameText;
    public TMP_Text damageText;
    public Slider damageSlider;
    public BaseUnit unit;

    public void Initialize(BaseUnit unit, float maxValue, float minValue, string statType)
    {
        this.unit = unit; // Store reference to the unit
        unitNameText.text = unit.unitName;
        damageSlider.maxValue = maxValue;
        damageSlider.minValue = minValue;
        LoadSplashImage(unit.unitName);
        UpdateStat(GetStatValue(statType));
    }

    public void UpdateStat(float statValue)
    {
        damageText.text = statValue.ToString(); // Update the stat display
        damageSlider.value = statValue; // Update the slider value
    }

    void LoadSplashImage(string unitName)
    {
        string splashImageName = unitName + "Splash";
        Texture2D loadedTexture = Resources.Load<Texture2D>($"Sprites/SplashUnits/{splashImageName}");
        if (loadedTexture != null)
        {
            damageSlider.GetComponentInChildren<Image>().sprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
        }
    }

    float GetStatValue(string statType)
    {
        switch (statType)
        {
            case "DamageDealt":
                return unit.damageDealt;
            case "DamageTaken":
                return unit.damageTaken;
            case "Healing":
                return unit.healingDone;
            default:
                return unit.damageDealt;  // Default to DamageDealt
        }
    }
}
