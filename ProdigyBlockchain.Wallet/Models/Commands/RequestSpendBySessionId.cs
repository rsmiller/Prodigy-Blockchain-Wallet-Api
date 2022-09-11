
namespace ProdigyBlockchain.Wallet.BusinessLayer.Models.Commands
{
    public class RequestSpendBySessionId
    {
        public string session_id { get; set; }
        public decimal amount { get; set; }
        public string note { get; set; }
    }
}
