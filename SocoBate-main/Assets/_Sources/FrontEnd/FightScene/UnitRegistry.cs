// UnitRegistry.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitRegistry : MonoBehaviour
{
    // Singleton instance for global access
    public static UnitRegistry Instance { get; private set; }

    // --- Private State ---
    // This dictionary holds ALL facades ever registered, including defeated ones.
    private readonly Dictionary<string, UnitFacade> _allRegisteredUnits = new Dictionary<string, UnitFacade>();
    
    // This dictionary holds only the CURRENTLY ACTIVE units.
    private readonly Dictionary<string, UnitFacade> _activeUnits = new Dictionary<string, UnitFacade>();

    private void Awake()
    {
        // Standard Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Adds a new unit to all registries. Called by the UnitSpawner.
    /// </summary>
    public void RegisterUnit(string uniqueID, UnitFacade unitFacade)
    {
        if (!_allRegisteredUnits.ContainsKey(uniqueID))
        {
            _allRegisteredUnits.Add(uniqueID, unitFacade);
            _activeUnits.Add(uniqueID, unitFacade);
            Debug.Log($"<color=green>Registered Unit:</color> {uniqueID}");
        }
        else
        {
            Debug.LogWarning($"Attempted to register a unit with a duplicate ID: {uniqueID}");
        }
    }

    /// <summary>
    /// Removes a unit from the ACTIVE list, but keeps it in the master list for stat tracking.
    /// Called by BattleManager's HandleUnitDeath.
    /// </summary>
    public void UnregisterUnit(string uniqueID)
    {
        if (_activeUnits.ContainsKey(uniqueID))
        {
            _activeUnits.Remove(uniqueID);
            Debug.Log($"<color=red>Deactivated Unit:</color> {uniqueID}");
        }
    }

    /// <summary>
    /// Gets a unit's facade by its unique ID from the master list.
    /// </summary>
    public UnitFacade GetUnitById(string uniqueID)
    {
        _allRegisteredUnits.TryGetValue(uniqueID, out var unit);
        return unit;
    }

    /// <summary>
    /// Gets a list of units.
    /// </summary>
    /// <param name="includeDefeated">If true, returns ALL units ever in the battle. If false, returns only active, living units.</param>
    public List<UnitFacade> GetPlayerUnits(bool includeDefeated = false)
    {
        var source = includeDefeated ? _allRegisteredUnits.Values : _activeUnits.Values;
        return source.Where(u => !u.IsEnemy).ToList();
    }

    /// <summary>
    /// Gets a list of units.
    /// </summary>
    /// <param name="includeDefeated">If true, returns ALL units ever in the battle. If false, returns only active, living units.</param>
    public List<UnitFacade> GetEnemyUnits(bool includeDefeated = false)
    {
        var source = includeDefeated ? _allRegisteredUnits.Values : _activeUnits.Values;
        return source.Where(u => u.IsEnemy).ToList();
    }
}