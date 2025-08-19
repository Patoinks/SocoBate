// BattleManager.cs (Definitive, with UI Raycast Input)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems; // Required for the UI Raycast system
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("UI & Settings")]
    [SerializeField] private TMP_Text turnCounterText;
    [SerializeField] private GameObject endBattleMenu;
    [SerializeField] private float timeBetweenTurns = 1.0f;
    
    [Header("Visuals")]
    [SerializeField] private GameObject turnIndicatorPrefab;
    
    [Header("Debug Toggles")]
    [SerializeField] private bool enableSpecialAttacks = true;

    public string WinnerTeam { get; private set; }
    public List<UnitFacade> PlayerUnitsAtEnd { get; private set; } = new List<UnitFacade>();
    public List<UnitFacade> EnemyUnitsAtEnd { get; private set; } = new List<UnitFacade>();

    private int _turnCounter = 0;
    private bool _isBattleOver = false;
    private readonly HashSet<UnitFacade> _defeatedUnitsThisTurn = new HashSet<UnitFacade>();
    private GameObject _turnIndicatorInstance;

    void Awake() { if (Instance != null && Instance != this) { Destroy(gameObject); return; } Instance = this; }

    void Start()
    {
        if (endBattleMenu != null) endBattleMenu.SetActive(false);
        if (turnIndicatorPrefab != null)
        {
            _turnIndicatorInstance = Instantiate(turnIndicatorPrefab);
            _turnIndicatorInstance.SetActive(false);
        }
        StartCoroutine(StartBattleSequence());
    }

    // --- NEW METHOD: Handles all clicks in the scene ---
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

            // Check if the clicked object has a UnitFacade, which means it's a unit.
            UnitFacade clickedUnit = clickedObject.GetComponent<UnitFacade>();
            if (clickedUnit != null)
            {
                // If we clicked a unit, tell the UIManager to show its stats.
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowStatsFor(clickedUnit);
                }
            }
        }
    }
    
    // The rest of the file is correct and remains the same.
    #region Unchanged Code
    private IEnumerator StartBattleSequence()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        while (!_isBattleOver)
        {
            _turnCounter++;
            turnCounterText.text = $"Turn: {_turnCounter}";
            _defeatedUnitsThisTurn.Clear();
            var unitsInTurnOrder = UnitRegistry.Instance.GetPlayerUnits().Concat(UnitRegistry.Instance.GetEnemyUnits()).OrderByDescending(u => u.UnitData.baseSpeed).ToList();
            foreach (var unit in unitsInTurnOrder)
            {
                if (_defeatedUnitsThisTurn.Contains(unit)) continue;
                if (_turnIndicatorInstance != null)
                {
                    _turnIndicatorInstance.transform.position = unit.transform.position;
                    _turnIndicatorInstance.SetActive(true);
                }
                yield return StartCoroutine(ExecuteUnitTurn(unit));
                if (CheckForVictory()) { _isBattleOver = true; break; }
            }
        }
    }
    
    public void SimulateToEndOfBattle()
    {
        StopAllCoroutines();
        while (!_isBattleOver)
        {
            _turnCounter++;
            turnCounterText.text = $"Turn: {_turnCounter}";
            _defeatedUnitsThisTurn.Clear();
            var unitsInTurnOrder = UnitRegistry.Instance.GetPlayerUnits().Concat(UnitRegistry.Instance.GetEnemyUnits()).OrderByDescending(u => u.UnitData.baseSpeed).ToList();
            foreach (var unit in unitsInTurnOrder)
            {
                if (_defeatedUnitsThisTurn.Contains(unit)) continue;
                ExecuteUnitTurnSynchronous(unit);
                if (CheckForVictory()) { _isBattleOver = true; break; }
            }
        }
    }

    private void ExecuteUnitTurnSynchronous(UnitFacade unit)
    {
        StatusEffectManager.ProcessStartOfTurnEffectsSynchronous(unit);
        if (unit.UnitData.baseHp <= 0) return;
        if (unit.UnitData.isStunned) return;
        AbilityExecutor.ExecuteSynchronous(unit, unit.UnitData.passiveAbility);
        if (_isBattleOver) return;
        BaseUnit.AttackData chosenAttack = unit.UnitData.normalAttack;
        if (enableSpecialAttacks && unit.UnitData.specialAttack != null && unit.UnitData.specialAttack.effects.Count > 0 && unit.UnitData.specialAttack.turnsToSpecial <= 0)
        {
            chosenAttack = unit.UnitData.specialAttack;
            unit.UnitData.specialAttack.turnsToSpecial = chosenAttack.turnsToSpecial;
        }
        AbilityExecutor.ExecuteSynchronous(unit, chosenAttack);
        if (unit.UnitData.specialAttack != null && unit.UnitData.specialAttack.turnsToSpecial > 0)
        {
            unit.UnitData.specialAttack.turnsToSpecial--;
        }
    }

    private IEnumerator ExecuteUnitTurn(UnitFacade unit)
    {
        yield return StartCoroutine(StatusEffectManager.ProcessStartOfTurnEffects(unit));
        if (unit.UnitData.baseHp <= 0) yield break;
        if (unit.UnitData.isStunned) yield break;
        yield return AbilityExecutor.Execute(unit, unit.UnitData.passiveAbility);
        if (_isBattleOver) yield break;
        BaseUnit.AttackData chosenAttack = unit.UnitData.normalAttack;
        if (enableSpecialAttacks && unit.UnitData.specialAttack != null && unit.UnitData.specialAttack.effects.Count > 0 && unit.UnitData.specialAttack.turnsToSpecial <= 0)
        {
            chosenAttack = unit.UnitData.specialAttack;
            unit.UnitData.specialAttack.turnsToSpecial = chosenAttack.turnsToSpecial;
        }
        yield return AbilityExecutor.Execute(unit, chosenAttack);
        if (unit.UnitData.specialAttack != null && unit.UnitData.specialAttack.turnsToSpecial > 0)
        {
            unit.UnitData.specialAttack.turnsToSpecial--;
        }
        if (!UIManager.IsFightSkipped) yield return new WaitForSeconds(timeBetweenTurns);
    }
    
    public void HandleUnitDeath(UnitFacade deadUnit)
    {
        if (_defeatedUnitsThisTurn.Contains(deadUnit)) return;
        _defeatedUnitsThisTurn.Add(deadUnit);
        UnitRegistry.Instance.UnregisterUnit(deadUnit.UniqueID);
        StartCoroutine(UnitAnimationController.Instance.AnimateDeath(deadUnit, 1.0f));
    }

    private bool CheckForVictory()
    {
        bool playersHaveUnits = UnitRegistry.Instance.GetPlayerUnits().Any();
        bool enemiesHaveUnits = UnitRegistry.Instance.GetEnemyUnits().Any();
        if (!playersHaveUnits) { EndBattle("Enemies"); return true; }
        if (!enemiesHaveUnits) { EndBattle("Players"); return true; }
        return false;
    }

    private void EndBattle(string winner)
    {
        if (_isBattleOver) return;
        _isBattleOver = true;
        WinnerTeam = winner;
        PlayerUnitsAtEnd = UnitRegistry.Instance.GetPlayerUnits(true);
        EnemyUnitsAtEnd = UnitRegistry.Instance.GetEnemyUnits(true);
        Time.timeScale = 1f;
        if (_turnIndicatorInstance != null) _turnIndicatorInstance.SetActive(false);
        StopAllCoroutines();
        DisableAllUnitGameObjects();
        if (endBattleMenu != null) endBattleMenu.SetActive(true);
    }

    private void DisableAllUnitGameObjects()
    {
        var allUnits = PlayerUnitsAtEnd.Concat(EnemyUnitsAtEnd);
        foreach (var unitFacade in allUnits)
        {
            if (unitFacade != null && unitFacade.transform.parent != null)
            {
                unitFacade.transform.parent.gameObject.SetActive(false);
            }
        }
    }
    #endregion
}