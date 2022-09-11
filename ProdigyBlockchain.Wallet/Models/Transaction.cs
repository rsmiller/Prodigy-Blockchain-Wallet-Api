using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    public class Transaction
    {
        public string txn { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public decimal amount { get; set; }
        public long created_on { get; set; }
        public string cert_block_id { get; set; }
        public string note { get; set; }

        public Transaction()
        {

        }
        public Transaction(string from, string to, decimal amount, string cert_block_id, long created_on, string note)
        {
            this.from = from;
            this.to = to;
            this.amount = amount;
            this.cert_block_id = cert_block_id;
            this.created_on = created_on;
            this.note = note;
        }
    }
}
