using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Models;
using Context;

public class HeroStatsList : MonoBehaviour
{
    public GameObject heroStatsRowPrefab;
    public Transform scrollViewContent;

    public TMP_Text heroName;
    public TMP_Text heroHP;
    public TMP_Text heroSTR;
    public TMP_Text heroMDEF;
    public TMP_Text heroINT;
    public TMP_Text heroPDEF;
    public TMP_Text heroSpecialATKDescription;
    public TMP_Text heroNormalATKDescription;
    public TMP_Text heroPassiveDescription;
    
    public TMP_Text heroSpeed;
    private GameObject currentUnitInstance;

    void Start()
    {
        GenerateHeroStatsRows();
    }

    public void GenerateHeroStatsRows()
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        List<OwnedUnits> ownedHeroes = UnitContext.ownedUnits;

        if (ownedHeroes == null || ownedHeroes.Count == 0)
        {
            Debug.Log("No heroes owned by the player.");
            return;
        }

        foreach (OwnedUnits hero in ownedHeroes)
        {
            BaseUnit baseHero = UnitContext.GetUnitByName(hero.unitId);
            if (baseHero == null)
            {
                Debug.LogError($"BaseUnit data not found for {hero.unitId}.");
                continue;
            }

            GameObject row = Instantiate(heroStatsRowPrefab, scrollViewContent);

            TMP_Text unitNameText = row.transform.Find("Nome")?.GetComponent<TMP_Text>();
            Button selectButton = row.transform.Find("SelectUnit")?.GetComponent<Button>();
            RawImage splashImage = row.transform.Find("Splash")?.GetComponent<RawImage>();

            if (unitNameText != null)
            {
                unitNameText.text = baseHero.unitName;
            }
            else
            {
                Debug.LogError("Unit name text not found in prefab!");
            }

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => DisplayHeroStats(baseHero));
            }
            else
            {
                Debug.LogError($"Select button not found for unit {baseHero.unitName}.");
            }

            if (splashImage != null)
            {
                string splashImageName = baseHero.unitName + "Splash";
                Texture2D loadedTexture = Resources.Load<Texture2D>($"Sprites/SplashUnits/{splashImageName}");
                if (loadedTexture != null)
                {
                    splashImage.texture = loadedTexture;
                }
                else
                {
                    Debug.LogError($"Splash image not found for {splashImageName}.");
                }
            }
        }
    }

    public void DisplayHeroStats(BaseUnit hero)
    {
        heroName.text = hero.unitName;
        heroHP.text = "HP: " + hero.maxHp.ToString();
        heroSTR.text = "STR: " + hero.baseStr.ToString();
        heroMDEF.text = "MDEF: " + hero.mDef.ToString();
        heroINT.text = "INT: " + hero.baseInt.ToString();
        heroPDEF.text = "PDEF: " + hero.pDef.ToString();
        heroSpeed.text = "Speed: " + hero.baseSpeed.ToString();
        heroSpecialATKDescription.text = hero.specialAttack.description;
        heroNormalATKDescription.text = hero.normalAttack.description;
        heroPassiveDescription.text = hero.passiveAbility.description;

        GameObject hexObject = GameObject.Find("Hex");
        if (hexObject != null)
        {
            string prefabName = hero.unitName;
            GameObject unitPrefab = Resources.Load<GameObject>($"UnitsPrefabs/{prefabName}");
            if (unitPrefab != null)
            {
                if (currentUnitInstance != null)
                {
                    Destroy(currentUnitInstance);
                }
                currentUnitInstance = Instantiate(unitPrefab, hexObject.transform);
                currentUnitInstance.transform.rotation = Quaternion.Euler(0, 0, 0);

                // Set position relative to the hex object
                currentUnitInstance.transform.localPosition = new Vector3(0, 100, 0);
            }
            else
            {
                Debug.LogError($"Unit prefab not found for {prefabName}.");
            }
        }
        else
        {
            Debug.LogError("Hex object not found in the scene!");
        }
    }

}
