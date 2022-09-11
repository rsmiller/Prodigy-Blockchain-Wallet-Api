using System;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    public class UserSession
    {
        public int id { get; set; }
        public string session_id { get; set; }
        public int user_id { get; set; }
        public DateTime expires_on { get; set; }
    }
}
