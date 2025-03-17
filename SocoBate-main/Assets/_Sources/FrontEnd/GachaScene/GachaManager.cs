using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Models;
using Context;
using Database;
using System;
using UnityEngine.UI;
using System.Collections;
public class GachaManagerCS : MonoBehaviour
{
    public GameObject oddsPanel;
    public TextMeshProUGUI resultText;
    public GameObject heroSpritePrefab;
    public GameObject rotatingImagePrefab;
    public Transform spawnPoint;
    public CanvasGroup splashEffectCanvas;
    public Image splashEffectImage;
    public Canvas canvas;  // Add this reference to the Canvas

    private GameObject previousUnitImage = null;  // Reference to the previous unit prefab
    private GameObject previousRotatingImage = null;  // Reference to the previous rotating image
    private GameObject previousSplashEffect = null;

    private Dictionary<int, int> rarityChances = new Dictionary<int, int>
    {
        { 2, 55 },
        { 3, 35 },
        { 4, 7 },
        { 5, 3 },
    };

    private void Start()
    {
        UnitContext.LoadAllUnitsFromSerializedData();
        Debug.Log($"Total Units Loaded: {UnitContext.allUnits.Count}");
        foreach (var unit in UnitContext.allUnits)
        {
            Debug.Log($"Loaded Unit: {unit.unitName} | Rarity: {unit.rarity}");
        }
    }

    public void OnClickInstantiateOdds()
    {
        Instantiate(oddsPanel, canvas.transform);
    }

    public async void PullGacha()
    {
        // Lock all buttons
        SetButtonsInteractable(false);

        // Perform the gacha pull
        BaseUnit pulledUnit = GetRandomUnitFromPool();
        ShowResult(pulledUnit);
        await AddUnitToAccount(pulledUnit);

        // Wait for 1 second
        await Task.Delay(2000);

        // Unlock all buttons
        SetButtonsInteractable(true);
    }

    public Dictionary<int, int> GetRarityChances()
    {
        return rarityChances;
    }

    private void SetButtonsInteractable(bool isInteractable)
    {
        foreach (var button in canvas.GetComponentsInChildren<Button>())
        {
            button.interactable = isInteractable;
        }
    }


    private BaseUnit GetRandomUnitFromPool()
    {
        List<BaseUnit> weightedPool = new List<BaseUnit>();
        foreach (var unit in UnitContext.allUnits)
        {
            if (rarityChances.ContainsKey(unit.rarity))
            {
                int weight = rarityChances[unit.rarity];
                for (int i = 0; i < weight; i++)
                {
                    weightedPool.Add(unit);
                }
            }
        }

        if (weightedPool.Count == 0)
        {
            Debug.LogWarning("No valid units available for gacha pull!");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, weightedPool.Count);
        return weightedPool[randomIndex];
    }

    private void ShowResult(BaseUnit unit)
    {
        if (unit != null)
        {
            resultText.text = $"You pulled: {unit.unitName} (Rarity: {unit.rarity})";

            // Remove the previous splash effect and unit image if they exist
            if (previousUnitImage != null)
            {
                Destroy(previousUnitImage);
            }
            if (previousRotatingImage != null)
            {
                Destroy(previousRotatingImage);
            }
            if (previousSplashEffect != null)
            {
                Destroy(previousSplashEffect);
            }

            // Find the prefab for the unit using the unit's name
            GameObject unitPrefab = FindUnitPrefab(unit.unitName);

            if (unitPrefab != null)
            {
                // Instantiate the unit prefab and parent it to the canvas
                GameObject unitImage = Instantiate(unitPrefab, canvas.transform);
                RectTransform unitImageRectTransform = unitImage.GetComponent<RectTransform>();
                unitImageRectTransform.anchoredPosition = new Vector2(150, 0); // Position it at (50, 0) to the right

                StartCoroutine(ScaleIn(unitImage.transform));

                // Instantiate rotating image and parent it to the canvas
                GameObject rotatingImage = Instantiate(rotatingImagePrefab, canvas.transform);
                RectTransform rotatingImageRectTransform = rotatingImage.GetComponent<RectTransform>();
                rotatingImageRectTransform.anchoredPosition = Vector2.zero; // Position it at (0, 0)

                rotatingImage.transform.SetParent(unitImage.transform, false); // Parent it to the unit image


                // Store references to the newly created objects
                previousUnitImage = unitImage;
                previousRotatingImage = rotatingImage;
            }
            else
            {
                Debug.LogError($"No prefab found for unit: {unit.unitName}");
            }

            StartCoroutine(ShowSplashEffect(unit.unitName));
        }
        else
        {
            resultText.text = "No unit pulled!";
        }
    }


    private GameObject FindUnitPrefab(string unitName)
    {
        // Load prefab from Resources/UnitsPrefabs/
        GameObject loadedPrefab = Resources.Load<GameObject>($"UnitsPrefabs/{unitName}");

        if (loadedPrefab == null)
        {
            Debug.LogError($"Prefab '{unitName}' not found in Resources/UnitsPrefabs/.");
        }

        return loadedPrefab;
    }




    private IEnumerator ScaleIn(Transform obj)
    {
        float duration = 0.5f;
        float time = 0;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        while (time < duration)
        {
            time += Time.deltaTime;
            obj.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            yield return null;
        }
        obj.localScale = endScale;
    }
    private IEnumerator ShowSplashEffect(string unitName)
    {
        // Remove previous splash effect if it exists
        if (previousSplashEffect != null)
        {
            Destroy(previousSplashEffect);
        }

        string splashPath = $"Sprites/SplashUnits/{unitName}Splash";
        Texture2D loadedTexture = Resources.Load<Texture2D>(splashPath);

        if (loadedTexture != null)
        {
            // Create a new GameObject for the splash effect
            GameObject splashEffect = new GameObject("SplashEffect");

            splashEffect.transform.SetParent(canvas.transform, false);  // Parent it to the canvas
            splashEffectImage.gameObject.SetActive(true);
            splashEffectImage.sprite = Sprite.Create(loadedTexture, new Rect(0, 0, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));

            // Set the new splash effect
            previousSplashEffect = splashEffect;

            splashEffectCanvas.gameObject.SetActive(true);

            // Fade in the splash effect
            float fadeDuration = 0.5f;
            float time = 0;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                splashEffectCanvas.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
                yield return null;
            }

            // After splash effect fades in, make sure it's visible and keep rotating
            // Do not fade out, let it stay visible


        }
        else
        {
            Debug.LogError($"Splash image not found: {splashPath}");
        }
    }





    private async Task AddUnitToAccount(BaseUnit unit)
    {
        if (unit != null)
        {
            Guid accountId = UserContext.account.AccountId;
            Debug.Log($"Unit drawn: {unit.unitName} | Account ID: {accountId} | Rarity: {unit.rarity}");
            await UnitController.NewHeroUnlocked(accountId, unit.unitName);
        }
    }
}
