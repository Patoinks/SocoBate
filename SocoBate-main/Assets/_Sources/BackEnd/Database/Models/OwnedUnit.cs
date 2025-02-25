using System;

namespace Models
{
    [Serializable]
    public class OwnedUnits
    {
        public string unitId;  // Unique identifier for the owned unit
        public Guid accountId;  // Unique identifier for the account that owns the unit
        public int lvl;  // Level of the unit
        public int StatRerrolToken;  // Reroll tokens available for the unit

        // Constructor
        public OwnedUnits(string unitId, Guid accountId, int lvl = 1, int statRerrolToken = 0)
        {
            this.unitId = unitId;
            this.accountId = accountId;
            this.lvl = lvl;
            this.StatRerrolToken = statRerrolToken;
        }
    }
}
