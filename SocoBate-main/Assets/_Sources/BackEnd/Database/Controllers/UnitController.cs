using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Context;
using UnityEngine;
using Models;

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
            string insertQuery = @"
                INSERT INTO OwnedUnits (account_id, unit_id)
                VALUES (@accountId, @unitName)";

            var insertParams = new Dictionary<string, object>
            {
                { "@accountId", accountId },
                { "@unitName", unitName } // Now passing the unit's name (string) instead of an integer unitId
            };

            // Insert the new owned unit into the database
            await DatabaseConnector.QueryDatabaseAsync(insertQuery, insertParams);

            // After inserting into the database, we update the in-memory list in UnitContext
            BaseUnit unit = UnitContext.GetUnitByName(unitName);  // Fetch the BaseUnit object by its unitName

            if (unit != null)
            {
                // Add the new owned unit to the in-memory ownedUnits list in UnitContext
                OwnedUnits ownedUnit = new OwnedUnits(unit.name, accountId);
                UnitContext.ownedUnits.Add(ownedUnit);

                Debug.Log($"New hero unlocked: {unit.unitName} for account {accountId}");
            }
        }
    }
}
