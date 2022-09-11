using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models.Commands
{
    public class CreateUserCommand
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
