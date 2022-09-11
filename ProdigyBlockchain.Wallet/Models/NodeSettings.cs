using System;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    public interface INodeSettings
    {
        string PrimaryNode { get; set; }
        int PrimaryNodePort { get; set; }
        string NodePassword { get; set; }
    }

    public class NodeSettings : INodeSettings
    {
        public string PrimaryNode { get; set; }
        public int PrimaryNodePort { get; set; }
        public string NodePassword { get; set; }
    }
}
