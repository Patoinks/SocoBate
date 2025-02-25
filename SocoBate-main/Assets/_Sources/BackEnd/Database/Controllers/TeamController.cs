using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MySql.Data.MySqlClient;
using Models;
using Context;

namespace Database
{
    public static class TeamController
    {
        // Save the team to the database (this will overwrite the current setup)
        public async static Task<bool> SaveTeam(Guid accountId, List<(int HexId, string UnitName)> teamSetup)
        {
            if (teamSetup == null || teamSetup.Count == 0)
            {
                Debug.LogError("Team setup is empty. Nothing to save.");
                return false;
            }

            Debug.Log($"[SaveTeam] AccountId: {accountId}, Units to save: {teamSetup.Count}");

            // Remove old team setup
            string deleteQuery = "DELETE FROM TeamSetup WHERE account_id = @accountId";  // Corrected here
            var deleteParams = new Dictionary<string, object> { { "@accountId", accountId } };

            Debug.Log($"[SaveTeam] Deleting previous team setup for AccountId: {accountId}");
            await DatabaseConnector.QueryDatabaseAsync(deleteQuery, deleteParams);
            Debug.Log("[SaveTeam] Previous team setup deleted successfully.");

            // Insert new team setup
            string insertQuery = @"
            INSERT INTO TeamSetup (account_id, hex_id, unit_name)
            VALUES (@accountId, @hexId, @unitName)";  // Corrected here

            foreach (var (hexId, unitName) in teamSetup)
            {
                if (string.IsNullOrEmpty(unitName))
                {
                    Debug.LogError($"[SaveTeam] Invalid unit name at HexId: {hexId}. Skipping...");
                    continue;
                }

                Debug.Log($"[SaveTeam] Preparing to insert: AccountId={accountId}, HexId={hexId}, UnitName={unitName}");

                var insertParams = new Dictionary<string, object>
                {
                    { "@accountId", accountId.ToString() },  // Convert GUID to string
                    { "@hexId", hexId },
                    { "@unitName", unitName }
                };

                await DatabaseConnector.QueryDatabaseAsync(insertQuery, insertParams);
                Debug.Log($"[SaveTeam] Insert successful: AccountId={accountId}, HexId={hexId}, UnitName={unitName}");
            }

            Debug.Log("[SaveTeam] Team setup saved successfully.");
            return true;
        }

        // Load the team from the database for a given account
        public async static Task<List<(int HexId, string UnitName)>> LoadTeam(Guid accountId)
        {
            string query = "SELECT hex_id, unit_name FROM TeamSetup WHERE account_id = @accountId";
            var parameters = new Dictionary<string, object> { { "@accountId", accountId } };

            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            List<(int HexId, string UnitName)> teamSetup = new List<(int, string)>();

            if (result != null)
            {
                foreach (var row in result)
                {
                    int hexId = Convert.ToInt32(row[0]);
                    string unitName = Convert.ToString(row[1]);
                    teamSetup.Add((hexId, unitName));
                }
            }

            return teamSetup;
        }

        // Retrieve an enemy squad from the database by AccountId
        public async static Task GetSquadByNickname(string enemyNickname)
        {
            Debug.Log($"[GetSquadByNickname] Retrieving squad for Nickname: {enemyNickname}");

            // Query using the nickname instead of AccountId
            string query = "SELECT hex_id, unit_name FROM TeamSetup WHERE account_id = (SELECT account_id FROM Account WHERE nickname = @enemyNickname)";
            var parameters = new Dictionary<string, object> { { "@enemyNickname", enemyNickname } };

            // Fetch squad data from the database
            List<object[]> result = await DatabaseConnector.QueryDatabaseAsync(query, parameters);
            List<TeamSetup> enemySquad = new List<TeamSetup>();

            if (result != null)
            {
                foreach (var row in result)
                {
                    int hexId = Convert.ToInt32(row[0]);
                    string unitName = Convert.ToString(row[1]);

                    // Create a TeamSetup object from the tuple values and add it to the enemySquad list
                    enemySquad.Add(new TeamSetup(Guid.Empty, hexId, unitName));
                }
            }

            // Log the result for debugging
            Debug.Log($"[GetSquadByNickname] Retrieved {enemySquad.Count} units for enemy squad.");

            // Add the squad to the context (assuming enemy squad is stored in TeamContext)
            TeamContext.EnemyTeam = enemySquad;

            // Optionally log the added squad data
            Debug.Log("[GetSquadByNickname] Enemy squad added to context.");
        }



        // Delete the team setup for the given accountId
        public async static Task<bool> DeleteTeam(Guid accountId)
        {
            try
            {
                string deleteQuery = "DELETE FROM TeamSetup WHERE account_id = @accountId";  // Corrected here
                var parameters = new Dictionary<string, object> { { "@accountId", accountId } };

                // Execute the delete query
                await DatabaseConnector.QueryDatabaseAsync(deleteQuery, parameters);
                Debug.Log("[DeleteTeam] Existing team setup deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DeleteTeam] Failed to delete team: {ex.Message}");
                return false;
            }
        }
    }
}
