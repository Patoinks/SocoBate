using System;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Unity.VisualScripting;

namespace Context
{
    public static class UnitContext
    {
        // In-memory list of all units (BaseUnit ScriptableObjects)
        public static List<BaseUnit> allUnits = new List<BaseUnit>();

        // In-memory list of owned units (OwnedUnit model containing ownership info)
        public static List<OwnedUnits> ownedUnits = new List<OwnedUnits>();

        // Method to load all BaseUnit data from the Resources folder (Units/UnitData)
        public static void LoadAllUnitsFromSerializedData()
        {
            // Load all BaseUnit assets from the "Units/UnitData" folder
            BaseUnit[] units = Resources.LoadAll<BaseUnit>("Units/UnitData");

            if (units.Length > 0)
            {
                allUnits.Clear();
                allUnits.AddRange(units); // Add all loaded units to the list
                Debug.Log($"{units.Length} units loaded from Resources/Units/UnitData.");
            }
            else
            {
                Debug.LogWarning("No BaseUnit data found in the Resources/Units/UnitData folder.");
            }
        }

        // Method to find a unit by its name
        public static BaseUnit GetUnitByName(string unitName)
        {
            // Find and return the unit by matching name
            return allUnits.Find(unit => unit != null && unit.unitName == unitName);
        }

        // Method to add an owned unit to the list (takes unitId and accountId as input)
        public static void AddUnitToOwnedUnits(string unitId, Guid accountId)
        {
            if (unitId != null && !ownedUnits.Exists(u => u.unitId == unitId && u.accountId == accountId))
            {
                OwnedUnits ownedUnit = new OwnedUnits(unitId, accountId);
                ownedUnits.Add(ownedUnit);
                Debug.Log($"Unit with ID {unitId} added to account {accountId}.");
            }
        }

        // Method to get all owned units for a specific account
        public static List<OwnedUnits> GetOwnedUnitsByAccount(Guid accountId)
        {
            return ownedUnits.FindAll(u => u.accountId == accountId);
        }
    }
}
