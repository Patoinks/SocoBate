using System;

namespace Models
{
    [Serializable]
    public class OwnedUnits
    {
        public string unitId;  // Unique identifier for the owned unit
        public Guid accountId;  // Unique identifier for the account that owns the unit

        // Constructor
        public OwnedUnits(string unitId, Guid accountId)
        {
            this.unitId = unitId;
            this.accountId = accountId;
        }
    }
}
