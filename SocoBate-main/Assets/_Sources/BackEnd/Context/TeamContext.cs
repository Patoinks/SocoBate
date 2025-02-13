using System;
using System.Collections.Generic;
using Models;

namespace Context
{
    public static class TeamContext
    {
        // Store the current team setup for the logged-in account
        public static List<TeamSetup> teamSetup { get; set; } = new List<TeamSetup>();

        // Method to set the team setup (store in TeamContext)
        public static void SetTeam(List<TeamSetup> team)
        {
            teamSetup = team;
        }

        // Method to clear the current team setup (e.g., when logging out or resetting)
        public static void ClearTeam()
        {
            teamSetup.Clear(); // Clear the team setup when logging out or resetting
        }

        // Method to retrieve the current team setup
        public static List<TeamSetup> GetTeam()
        {
            return teamSetup;
        }

        // Method to retrieve the unit name for a specific hex (position)
        public static string GetUnitByHex(int hexId)
        {
            // Find the unit in the teamSetup based on the HexId
            var unit = teamSetup.Find(t => t.HexId == hexId);
            return unit?.UnitName; // Return null if not found
        }

        // Method to update a unit in a specific hex position
        public static void SetUnitInHex(int hexId, string unitName)
        {
            // Check if the unit already exists in the team setup
            var index = teamSetup.FindIndex(t => t.HexId == hexId);

            if (index >= 0)
            {
                // Update the existing unit in that hex
                teamSetup[index].UnitName = unitName;
            }
            else
            {
                // Add a new unit to that hex if it doesn't exist
                teamSetup.Add(new TeamSetup(Guid.Empty, hexId, unitName)); // Assuming Guid.Empty for now, update this as needed
            }
        }
    }
}
