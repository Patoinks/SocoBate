using System;

namespace Models
{
    public class Friend
    {
        public string AccountId1 { get; set; }  // User 1 (as string GUID)
        public string AccountId2 { get; set; }  // User 2 (as string GUID)

        public Friend(string accountId1, string accountId2)
        {
            AccountId1 = accountId1;
            AccountId2 = accountId2;
        }
    }
}
