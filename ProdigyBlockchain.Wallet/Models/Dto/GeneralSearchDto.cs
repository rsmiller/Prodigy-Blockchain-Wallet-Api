using System.Collections.Generic;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models.Dto
{
    public class GeneralSearchDto
    {
        public string return_type { get; set; }
        public List<BlockDto> blocks { get; set; } = new List<BlockDto>();
        public List<Transaction> transactions { get; set; } = new List<Transaction>();
    }
}
