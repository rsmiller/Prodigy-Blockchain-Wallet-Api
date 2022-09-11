using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models.Dto
{
    public class UserDto
    {
        public int user_id { get; set; }
        public string wallet_address { get; set; }
        public decimal wallet_total { get; set; }
        public decimal pending { get; set; }
        public string session_id { get; set; }
        public List<Transaction> transaction { get; set; } = new List<Transaction>();
    }
}
