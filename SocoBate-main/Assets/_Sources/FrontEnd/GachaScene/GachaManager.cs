// GachaManagerCS.cs (Definitive, No CanvasGroup)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Models;
using Context;
using Database;
using System;
using System.Collections;

public class GachaManagerCS : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject oddsPanelPrefab;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Transform unitSpawnPoint;
    [SerializeField] private Image splashEffectImage; // The Image component for the splash art
    [SerializeField] private Canvas mainCanvas;

    // The CanvasGroup field has been removed.

    // --- Private State ---
    private GameObject _currentUnitInstance;
    private bool _isPulling = false;

    private readonly Dictionary<int, int> _rarityChances = new Dictionary<int, int>
    {
        { 2, 55 },
        { 3, 35 },
        { 4, 7 },
        { 5, 3 },
    };

    void Start()
    {
        // Set the initial state of the UI elements
        if (splashEffectImage != null) splashEffectImage.enabled = false; // Start with the splash image hidden
        if (resultText != null) resultText.text = "Pull to get a new hero!";
        UnitContext.LoadAllUnitsFromSerializedData();
    }
    
    public Dictionary<int, int> GetRarityChances()
    {
        return _rarityChances;
    }

    public void OnClickInstantiateOdds()
    {
        if (oddsPanelPrefab != null && mainCanvas != null)
        {
            GameObject oddsPanelInstance = Instantiate(oddsPanelPrefab, mainCanvas.transform);
            oddsPanelInstance.SetActive(true);
            Button closeButton = oddsPanelInstance.transform.Find("CloseButton")?.GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(() => Destroy(oddsPanelInstance));
            }
        }
    }

    public void OnClickPullGacha()
    {
        if (_isPulling) return;
        StartCoroutine(PullGachaCoroutine());
    }

    private IEnumerator PullGachaCoroutine()
    {
        // --- 1. LOCK ---
        _isPulling = true;
        SetAllButtonsInteractable(false);

        // --- 2. PREPARE ---
        if (_currentUnitInstance != null) Destroy(_currentUnitInstance);
        if (resultText != null) resultText.text = "";
        if (splashEffectImage != null) splashEffectImage.enabled = false; // Hide old splash art

        // --- 3. PULL LOGIC ---
        BaseUnit pulledUnit = GetRandomUnitFromPool();
        if (pulledUnit == null)
        {
            if (resultText != null) resultText.text = "Error: Unit pool is empty!";
            _isPulling = false;
            SetAllButtonsInteractable(true);
            yield break;
        }

        // --- 4. REVEAL & ANIMATE ---
        if (resultText != null) resultText.text = $"You pulled: {pulledUnit.unitName}!";
        
        LoadAndPositionUnitPrefab(pulledUnit);
        LoadAndShowSplashArt(pulledUnit); // This now just enables the image
        _currentUnitInstance.transform.localScale = Vector3.zero;
        splashEffectImage.transform.localScale = Vector3.zero;

        // Start both animations at the same time
        StartCoroutine(AnimateRectTransform(
            _currentUnitInstance.GetComponent<RectTransform>(), 
            Vector3.one * 4,
            new Vector2(150, 0),
            0.5f
        ));
        
        StartCoroutine(AnimateRectTransform(
            splashEffectImage.GetComponent<RectTransform>(), 
            Vector3.one,
            new Vector2(-150, 0),
            0.5f
        ));

        yield return new WaitForSeconds(0.7f);
        yield return new WaitForSeconds(1.5f);

        // --- 5. SAVE TO DATABASE ---
        Task saveTask = AddUnitToAccount(pulledUnit);
        yield return new WaitUntil(() => saveTask.IsCompleted);

        // --- 6. UNLOCK ---
        _isPulling = false;
        SetAllButtonsInteractable(true);
    }

    private BaseUnit GetRandomUnitFromPool()
    {
        var availableUnits = UnitContext.allUnits.Where(u => _rarityChances.ContainsKey(u.rarity)).ToList();
        if (!availableUnits.Any()) return null;
        int totalWeight = availableUnits.Sum(u => _rarityChances[u.rarity]);
        int randomRoll = UnityEngine.Random.Range(0, totalWeight);
        foreach (var unit in availableUnits)
        {
            int weight = _rarityChances[unit.rarity];
            if (randomRoll < weight) return unit;
            randomRoll -= weight;
        }
        return null;
    }
    
    private void LoadAndPositionUnitPrefab(BaseUnit unit)
    {
        GameObject unitPrefab = Resources.Load<GameObject>($"UnitsPrefabs/{unit.unitName}");
        if (unitPrefab == null) { Debug.LogError($"Prefab for unit '{unit.unitName}' not found!"); return; }
        _currentUnitInstance = Instantiate(unitPrefab, unitSpawnPoint);
        _currentUnitInstance.transform.localPosition = Vector3.zero;
    }
    
    private void LoadAndShowSplashArt(BaseUnit unit)
    {
        if (splashEffectImage == null) return;
        
        Texture2D splashTexture = Resources.Load<Texture2D>($"Sprites/SplashUnits/{unit.unitName}Splash");
        if (splashTexture != null)
        {
            splashEffectImage.sprite = Sprite.Create(splashTexture, new Rect(0, 0, splashTexture.width, splashTexture.height), new Vector2(0.5f, 0.5f));
            splashEffectImage.enabled = true; // Make the splash image visible
        }
    }

    private async Task AddUnitToAccount(BaseUnit unit)
    {
        if (unit != null)
        {
            await UnitController.NewHeroUnlocked(UserContext.account.AccountId, unit.unitName);
        }
    }

    private void SetAllButtonsInteractable(bool isInteractable)
    {
        if (mainCanvas == null) return;
        foreach (var button in mainCanvas.GetComponentsInChildren<Button>(true))
        {
            button.interactable = isInteractable;
        }
    }

    private IEnumerator AnimateRectTransform(RectTransform rect, Vector3 targetScale, Vector2 targetPosition, float duration)
    {
        if (rect == null) yield break;

        Vector3 startScale = rect.localScale;
        Vector2 startPosition = rect.anchoredPosition;
        float time = 0;

        while (time < duration)
        {
            float progress = time / duration;
            rect.localScale = Vector3.Lerp(startScale, targetScale, progress);
            rect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, progress);
            time += Time.deltaTime;
            yield return null;
        }
        
        rect.localScale = targetScale;
        rect.anchoredPosition = targetPosition;
    }
}