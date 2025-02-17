using System;
using System.Collections.Generic;
using Models;

namespace Context
{
    public static class TeamContext
    {
        // Store the current team setup for the logged-in account
        public static List<TeamSetup> PlayerTeam { get; set; } = new List<TeamSetup>();
        public static List<TeamSetup> EnemyTeam { get; set; } = new List<TeamSetup>();

        // Method to set the player team setup
        public static void SetPlayerTeam(List<TeamSetup> team)
        {
            PlayerTeam = team;
        }

        // Method to set the enemy team setup
        public static void SetEnemyTeam(List<TeamSetup> team)
        {
            EnemyTeam = team;
        }

        // Method to clear the player team setup
        public static void ClearPlayerTeam()
        {
            PlayerTeam.Clear();
        }

        // Method to clear the enemy team setup
        public static void ClearEnemyTeam()
        {
            EnemyTeam.Clear();
        }

        // Method to clear both teams
        public static void ClearAllTeams()
        {
            PlayerTeam.Clear();
            EnemyTeam.Clear();
        }

        // Method to retrieve the current player team setup
        public static List<TeamSetup> GetPlayerTeam()
        {
            return PlayerTeam;
        }

        // Method to retrieve the current enemy team setup
        public static List<TeamSetup> GetEnemyTeam()
        {
            return EnemyTeam;
        }

        // Method to retrieve the unit name for a specific hex in the player's team
        public static string GetPlayerUnitByHex(int hexId)
        {
            var unit = PlayerTeam.Find(t => t.HexId == hexId);
            return unit?.UnitName;
        }

        // Method to retrieve the unit name for a specific hex in the enemy's team
        public static string GetEnemyUnitByHex(int hexId)
        {
            var unit = EnemyTeam.Find(t => t.HexId == hexId);
            return unit?.UnitName;
        }

        // Method to update a unit in a specific hex position for the player's team
        public static void SetPlayerUnitInHex(int hexId, string unitName)
        {
            var index = PlayerTeam.FindIndex(t => t.HexId == hexId);

            if (index >= 0)
            {
                PlayerTeam[index].UnitName = unitName;
            }
            else
            {
                PlayerTeam.Add(new TeamSetup(Guid.Empty, hexId, unitName));
            }
        }

        // Method to update a unit in a specific hex position for the enemy's team
        public static void SetEnemyUnitInHex(int hexId, string unitName)
        {
            var index = EnemyTeam.FindIndex(t => t.HexId == hexId);

            if (index >= 0)
            {
                EnemyTeam[index].UnitName = unitName;
            }
            else
            {
                EnemyTeam.Add(new TeamSetup(Guid.Empty, hexId, unitName));
            }
        }
    }
}
 