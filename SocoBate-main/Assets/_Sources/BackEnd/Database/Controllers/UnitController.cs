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
            string query = "SELECT unit_id FROM OwnedUnits WHERE account_id = @accountId";
            var parameters = new Dictionary<string, object>
    {
        { "@accountId", accountId }
    };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            if (result == null || result.Count == 0)
            {
                Debug.LogWarning($"No units found for account {accountId}");
                return new List<OwnedUnits>(); // Return empty if no data
            }

            List<OwnedUnits> ownedUnits = new List<OwnedUnits>();

            foreach (var row in result)
            {
                // Assuming unit_id is in the first column of the result
                string unitName = Convert.ToString(row[0]);

                Debug.Log($"Fetched unitName from DB: {unitName}"); // Debugging the unitName fetched

                // Fetch the unit by its name from the in-memory units list
                BaseUnit unit = UnitContext.GetUnitByName(unitName);

                if (unit != null)
                {
                    // Log the successful unit fetching
                    Debug.Log($"Unit found in context: {unit.unitName}");

                    // Create an OwnedUnit for this entry
                    OwnedUnits ownedUnit = new OwnedUnits(unit.name, accountId);
                    ownedUnits.Add(ownedUnit);
                }
                else
                {
                    // Log if the unit was not found in memory
                    Debug.LogWarning($"Unit with name {unitName} not found in memory.");
                }
            }

            // Assign to the in-memory ownedUnits list in UnitContext
            UnitContext.ownedUnits = ownedUnits;

            // Log to confirm the units in memory
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
