using System;
using System.Collections.Generic;
using System.Text;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    public interface IAuthenticationSettings
    {
        string APIPrivateKey { get; set; }
        string APIUsername { get; set; }
        string APIPassword { get; set; }
    }

    public class AuthenticationSettings : IAuthenticationSettings
    {
        public string APIPrivateKey { get; set; }
        public string APIUsername { get; set; }
        public string APIPassword { get; set; }
    }
}
