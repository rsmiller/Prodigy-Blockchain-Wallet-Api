using System;
using System.Collections.Generic;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models.Dto
{
    public class BlockDto
    {
		public Guid BlockId { get; set; }
		public Guid CustomerId { get; set; }
		public string PreviousHash { get; set; }
		public int Nonce { get; set; }
		public long CreatedOn { get; set; }
		public long MinedOn { get; set; }
		public string Data { get; set; }
		public string Hash { get; set; }
		public string Identifier1 { get; set; }
		public string Identifier2 { get; set; }
		public string Identifier3 { get; set; }
		public string Identifier4 { get; set; }
		public string Identifier5 { get; set; }
		public List<Transaction> TransactionList { get; set; } = new List<Transaction>();
	}
}
