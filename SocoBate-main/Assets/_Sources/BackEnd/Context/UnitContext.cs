using System;
using System.Collections.Generic;
using UnityEngine;
using Models;

namespace Context
{
    public static class UnitContext
    {
        // In-memory list of all units (BaseUnit ScriptableObjects)
        public static List<BaseUnit> allUnits = new List<BaseUnit>();

        // In-memory list of owned units (BaseUnit ScriptableObjects)
        public static List<BaseUnit> ownedUnits = new List<BaseUnit>();

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

        // Method to add a unit to the ownedUnits list
        public static void AddUnitToOwnedUnits(BaseUnit unit)
        {
            if (unit != null && !ownedUnits.Contains(unit))
            {
                ownedUnits.Add(unit);
                Debug.Log($"Unit {unit.unitName} added to owned units.");
            }
        }
    }
}
