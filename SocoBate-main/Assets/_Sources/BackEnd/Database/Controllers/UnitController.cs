using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Context;
using UnityEngine;
using Models;
using System.Linq;

namespace Database
{
    public static class UnitController
    {
        // Get all owned units for a given account
        public static async Task<List<OwnedUnits>> GetOwnedUnits(Guid accountId)
        {
            string query = "SELECT unit_id, lvl, StatRerrolToken FROM OwnedUnits WHERE account_id = @accountId";
            var parameters = new Dictionary<string, object>
    {
        { "@accountId", accountId }
    };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            if (result == null || result.Count == 0)
            {
                Debug.LogWarning($"No units found for account {accountId}");
                return new List<OwnedUnits>(); // Return empty list if no data
            }

            List<OwnedUnits> ownedUnits = new List<OwnedUnits>();

            foreach (var row in result)
            {
                string unitName = Convert.ToString(row[0]);
                int lvl = Convert.ToInt32(row[1]);
                int statRerrolToken = Convert.ToInt32(row[2]);

                Debug.Log($"Fetched unitName: {unitName}, lvl: {lvl}, StatRerrolToken: {statRerrolToken}");

                BaseUnit unit = UnitContext.GetUnitByName(unitName);

                if (unit != null)
                {
                    Debug.Log($"Unit found in context: {unit.unitName}");

                    OwnedUnits ownedUnit = new OwnedUnits(unit.name, accountId, lvl, statRerrolToken);
                    ownedUnits.Add(ownedUnit);
                }
                else
                {
                    Debug.LogWarning($"Unit with name {unitName} not found in memory.");
                }
            }

            UnitContext.ownedUnits = ownedUnits;

            Debug.Log($"Total owned units in memory: {UnitContext.ownedUnits.Count}");

            return ownedUnits;
        }



        // Add a new unit to the owned units for the account (e.g., when pulling from gacha)
        public async static Task NewHeroUnlocked(Guid accountId, string unitName)
        {
            // Check if the unit is already owned in context
            if (UnitContext.GetOwnedUnitsByAccount(accountId).Exists(u => u.unitId == unitName))
            {
                Debug.Log($"Hero {unitName} is already unlocked for account {accountId}, skipping gacha and database insert.");
                return; // Exit early, no need for database insertion
            }

            // Get the unit from the context
            BaseUnit unit = UnitContext.GetUnitByName(unitName);
            if (unit == null)
            {
                Debug.LogWarning($"Unit {unitName} not found in UnitContext.");
                return;
            }

            // Insert into the database
            string insertQuery = @"
        INSERT INTO OwnedUnits (account_id, unit_id)
        VALUES (@accountId, @unitName)";

            var insertParams = new Dictionary<string, object>
    {
        { "@accountId", accountId },
        { "@unitName", unitName }
    };

            await DatabaseConnector.QueryDatabaseAsync(insertQuery, insertParams);

            // Add to in-memory context
            UnitContext.AddUnitToOwnedUnits(unit.name, accountId);

            Debug.Log($"New hero unlocked: {unit.unitName} for account {accountId}");
        }


    }
}
