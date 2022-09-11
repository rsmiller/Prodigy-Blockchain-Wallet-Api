using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models.Database
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string wallet_address { get; set; }
        public DateTime? last_login_on { get; set; }
        public DateTime created_on { get; set; }
    }
}
