using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Context;
using Models;

namespace Database
{
    public static class UnitController
    {
        // Get all owned units for a given account
        public async static Task<List<BaseUnit>> GetOwnedUnits(Guid accountId)
        {
            string query = "SELECT unit_id FROM OwnedUnits WHERE account_id = @accountId";
            var parameters = new Dictionary<string, object>
            {
                { "@accountId", accountId }
            };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

            List<BaseUnit> ownedUnits = new List<BaseUnit>();

            foreach (var row in result)
            {
                string unitName = Convert.ToString(row[0]); // Assuming unit_id is the first column, now a string (unit name)
                
                // Fetch the unit by its name from the in-memory units list
                BaseUnit unit = UnitContext.GetUnitByName(unitName); 

                if (unit != null)
                {
                    ownedUnits.Add(unit);
                }
            }

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

            await DatabaseConnector.QueryDatabaseAsync(insertQuery, insertParams);

            // After inserting into the database, we update the in-memory list in UnitContext
            BaseUnit unit = UnitContext.GetUnitByName(unitName);  // Fetch the BaseUnit object by its unitName

            if (unit != null)
            {
                UnitContext.AddUnitToOwnedUnits(unit);
            }
        }
    }
}
