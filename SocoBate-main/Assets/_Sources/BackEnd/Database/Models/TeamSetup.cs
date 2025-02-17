using System;

namespace Models
{
    public class TeamSetup
    {
        public Guid AccountId { get; set; } // ID da conta associada
        public int HexId { get; set; }     // ID do Hex (1 a 9)
        public string UnitName { get; set; } // Nome da unidade

        // Constructor
        public TeamSetup(Guid accountId, int hexId, string unitName)
        {
            AccountId = accountId;
            HexId = hexId;
            UnitName = unitName;
        }
    }
}
