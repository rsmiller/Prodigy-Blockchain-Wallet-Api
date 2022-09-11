using System;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    [Serializable]
    public class KeyPair
    {
        public string pub { get; set; }
        public string priv { get; set; }
    }
}
