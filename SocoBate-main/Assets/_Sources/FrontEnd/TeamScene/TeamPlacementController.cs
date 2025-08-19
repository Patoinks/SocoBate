// TeamPlacementController.cs (Definitive, with Y-Offset Control)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Models;
using TMPro;
using Context;
using Database;
using System.Threading.Tasks;

public class TeamPlacementController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameObject ownedUnitRowPrefab;
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private List<GameObject> hexes;
    [SerializeField] private Button saveButton;

    [Header("Placement Settings")]
    [Tooltip("A multiplier applied to the unit prefab's original scale when placed.")]
    [SerializeField] private float placementScaleMultiplier = 2.1f;
    [Tooltip("The vertical (Y) offset of the unit relative to the hex's center.")]
    [SerializeField] private float placementYOffset = 10f; // Added this line

    private string _selectedUnitForPlacement;
    private readonly HashSet<string> _placedUnitNames = new HashSet<string>();

    void Start()
    {
        GenerateUnitRowsFromContext();
        LoadExistingTeam();
        if (saveButton != null) { saveButton.onClick.AddListener(OnSaveClicked); }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        if (results.Count > 0)
        {
            GameObject clickedObject = results[0].gameObject;
            if (clickedObject.CompareTag("PlayerHex"))
            {
                if (!string.IsNullOrEmpty(_selectedUnitForPlacement))
                {
                    PlaceUnitOnHex(_selectedUnitForPlacement, clickedObject);
                }
            }
            else if (clickedObject.CompareTag("PlacedUnit"))
            {
                RemoveUnitFromHex(clickedObject.transform.parent.gameObject);
            }
        }
    }
    
    // --- THIS IS THE MODIFIED METHOD ---
    private void PlaceUnitOnHex(string unitName, GameObject hex, bool isNewPlacement = true)
    {
        if (hex.transform.childCount > 0) { return; }
        GameObject unitPrefab = Resources.Load<GameObject>($"UnitsPrefabs/{unitName}");
        if (unitPrefab == null) { return; }

        GameObject spawnedUnit = Instantiate(unitPrefab, hex.transform);
        
        // --- THIS IS THE FIX ---
        // 1. Set the unit's localPosition to be exactly (0, 10, 0) relative to the hex.
        spawnedUnit.transform.localPosition = new Vector3(0, placementYOffset, 0);

        // 2. Multiply the prefab's scale by our desired multiplier.
        spawnedUnit.transform.localScale *= placementScaleMultiplier;
        
        _placedUnitNames.Add(unitName);
        if(isNewPlacement)
        {
            _selectedUnitForPlacement = null;
        }
        DisableButtonInScrollView(unitName);
    }
    
    #region Unchanged Methods
    public void GenerateUnitRowsFromContext()
    {
        foreach (Transform child in scrollViewContent) { Destroy(child.gameObject); }
        List<OwnedUnits> ownedUnits = UnitContext.ownedUnits;
        if (ownedUnits == null) return;
        foreach (OwnedUnits unit in ownedUnits)
        {
            GameObject row = Instantiate(ownedUnitRowPrefab, scrollViewContent);
            TMP_Text unitNameText = row.transform.Find("Nome")?.GetComponent<TMP_Text>();
            Button selectButton = row.transform.Find("SelectUnit")?.GetComponent<Button>();
            RawImage splashImage = row.transform.Find("Splash")?.GetComponent<RawImage>();
            if (unitNameText != null) unitNameText.text = unit.unitId;
            if (splashImage != null)
            {
                string splashImageName = unit.unitId + "Splash";
                splashImage.texture = Resources.Load<Texture2D>($"Sprites/SplashUnits/{splashImageName}");
            }
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(() => SelectUnitToPlace(unit.unitId, selectButton));
            }
        }
    }
    
    public void LoadExistingTeam()
    {
        List<TeamSetup> teamUnits = TeamContext.GetPlayerTeam();
        if (teamUnits == null || teamUnits.Count == 0) return;
        foreach (var unit in teamUnits)
        {
            GameObject hex = hexes.FirstOrDefault(h => h.name == $"Hex{unit.HexId}");
            if (hex != null)
            {
                PlaceUnitOnHex(unit.UnitName, hex, false);
            }
        }
    }

    public void SelectUnitToPlace(string unitName, Button clickedButton)
    {
        if (_placedUnitNames.Count >= 5) { return; }
        _selectedUnitForPlacement = unitName;
        clickedButton.interactable = false;
    }
    
    public void RemoveUnitFromHex(GameObject hex)
    {
        if (hex.transform.childCount == 0) return;
        GameObject unitToRemove = hex.transform.GetChild(0).gameObject;
        string unitName = unitToRemove.name.Replace("(Clone)", "");
        _placedUnitNames.Remove(unitName);
        EnableButtonInScrollView(unitName);
        Destroy(unitToRemove);
    }
    
    private void EnableButtonInScrollView(string unitName)
    {
        foreach (Transform row in scrollViewContent)
        {
            TMP_Text unitNameText = row.Find("Nome")?.GetComponent<TMP_Text>();
            if (unitNameText != null && unitNameText.text == unitName)
            {
                Button selectButton = row.Find("SelectUnit")?.GetComponent<Button>();
                if (selectButton != null)
                {
                    selectButton.interactable = true;
                    return;
                }
            }
        }
    }

    private void DisableButtonInScrollView(string unitName)
    {
        foreach (Transform row in scrollViewContent)
        {
            TMP_Text unitNameText = row.Find("Nome")?.GetComponent<TMP_Text>();
            if (unitNameText != null && unitNameText.text == unitName)
            {
                Button selectButton = row.Find("SelectUnit")?.GetComponent<Button>();
                if (selectButton != null)
                {
                    selectButton.interactable = false;
                    return;
                }
            }
        }
    }

    public async void OnSaveClicked()
    {
        saveButton.interactable = false;
        List<(int HexId, string UnitName)> teamToSave = new List<(int, string)>();
        for (int i = 0; i < hexes.Count; i++)
        {
            GameObject hex = hexes[i];
            if (hex.transform.childCount > 0)
            {
                Transform unitObject = hex.transform.GetChild(0);
                string unitName = unitObject.name.Replace("(Clone)", "");
                teamToSave.Add((i + 1, unitName));
            }
        }
        bool deleteSuccess = await TeamController.DeleteTeam(UserContext.account.AccountId);
        if (!deleteSuccess) { saveButton.interactable = true; return; }
        if (teamToSave.Any())
        {
            await TeamController.SaveTeam(UserContext.account.AccountId, teamToSave);
        }
        await RefreshTeamContext();
        saveButton.interactable = true;
    }

    private async Task RefreshTeamContext()
    {
        List<(int HexId, string UnitName)> teamSetupFromDb = await TeamController.LoadTeam(UserContext.account.AccountId);
        List<TeamSetup> newTeamContext = new List<TeamSetup>();
        foreach (var (hexId, unitName) in teamSetupFromDb)
        {
            newTeamContext.Add(new TeamSetup(UserContext.account.AccountId, hexId, unitName));
        }
        TeamContext.SetPlayerTeam(newTeamContext);
    }
    #endregion
}