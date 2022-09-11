using System;
using System.Collections.Generic;
using System.Text;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    public interface IWalletSettings
    {
        string Prefix { get; set; }
        string PrivateKey { get; set; }
    }

    public class WalletSettings : IWalletSettings
    {
        public string Prefix { get; set; }
        public string PrivateKey { get; set; }
    }
}
