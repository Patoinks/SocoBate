using System;

namespace Models
{
    public class TeamSetup
    {
        public Guid AccountId { get; set; }  // The unique ID of the account (FK)
        public Guid TeamId { get; set; }     // Unique identifier for the team (FK)
        public int HexId { get; set; }       // Hex position (1-9)
        public string UnitName { get; set; } // The name of the unit placed in that hex

        // Default Constructor (Required for Serialization)
        public TeamSetup() { }

        // Constructor
        public TeamSetup(Guid accountId, Guid teamId, int hexId, string unitName)
        {
            AccountId = accountId;
            TeamId = teamId;
            HexId = hexId;
            UnitName = unitName;
        }

        // Debugging helper
        public override string ToString()
        {
            return $"[TeamSetup] AccountId: {AccountId}, TeamId: {TeamId}, HexId: {HexId}, UnitName: {UnitName}";
        }
    }
}
