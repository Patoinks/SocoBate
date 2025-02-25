
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using UnityEngine;
using Database;
using Context;

public static class FightController
{
    // Get enemy team by nickname (single query)
    public static async Task<List<OwnedUnits>> GetEnemyTeamByNickname(string enemyNickname)
    {
        string query = @"
            SELECT ou.unit_id, ou.lvl, a.account_id
            FROM OwnedUnits ou
            JOIN Account a ON ou.account_id = a.account_id
            WHERE a.nickname = @enemyNickname";

        var parameters = new Dictionary<string, object>
        {
            { "@enemyNickname", enemyNickname }
        };

        List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);

        if (result == null || result.Count == 0)
        {
            Debug.LogError($"No units found for enemy with nickname: {enemyNickname}");
            return new List<OwnedUnits>(); // Return empty list if no data
        }

        List<OwnedUnits> enemyTeam = new List<OwnedUnits>();

        foreach (var row in result)
        {
            string unitName = Convert.ToString(row[0]);
            int lvl = Convert.ToInt32(row[1]);
            Guid enemyAccountId = Guid.Parse(Convert.ToString(row[2])); // Account ID from the result

            Debug.Log($"Fetched enemy unit: {unitName}, lvl: {lvl}, AccountId: {enemyAccountId}");

            BaseUnit unit = UnitContext.GetUnitByName(unitName);

            if (unit != null)
            {
                Debug.Log($"Enemy unit found in context: {unit.unitName}");

                OwnedUnits enemyUnit = new OwnedUnits(unit.name, enemyAccountId, lvl, 0); // Default StatRerrolToken = 0
                enemyTeam.Add(enemyUnit);
            }
            else
            {
                Debug.LogWarning($"Enemy unit with name {unitName} not found in memory.");
            }
        }

        return enemyTeam;
    }
}
