using System;

namespace Models
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int Gems { get; set; } = 0;
        public int Tickets { get; set; } = 0;
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public bool Banned { get; set; } = false;
        public int ArenaElo { get; set; } = 1000;  // Default Elo rating
        public string Nickname { get; set; } = ""; // Default empty nickname

        // Constructor
        public Account(Guid accountId, string username, string email, string passwordHash, int gems, int tickets, int level, int experience, bool banned, int arenaElo, string nickname)
        {
            AccountId = accountId;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Gems = gems;
            Tickets = tickets;
            Level = level;
            Experience = experience;
            Banned = banned;
            ArenaElo = arenaElo;
            Nickname = nickname;
        }

        // Static method to create Account from database row
        public static Account FromRow(object[] row)
        {
            return new Account(
                Guid.Parse(Convert.ToString(row[0])),  // account_id as Guid
                Convert.ToString(row[1]),  // username
                Convert.ToString(row[2]),  // email
                Convert.ToString(row[3]),  // password_hash
                Convert.ToInt32(row[4]),   // gems
                Convert.ToInt32(row[5]),   // tickets
                Convert.ToInt32(row[6]),   // level
                Convert.ToInt32(row[7]),   // experience
                Convert.ToBoolean(row[8]),  // banned
                Convert.ToInt32(row[9]),  // arena_elo
                Convert.ToString(row[10])  // nickname
            );
        }
    }
}
