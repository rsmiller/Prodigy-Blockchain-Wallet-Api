using System;
using System.Collections.Generic;
using System.Text;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    public interface IDatabaseConnectionSettings
    {
        string ConnectionString { get; set; }
        string MSSQLConnectionString { get; set; }
        string CertsConnectionString { get; set; }
        string DatabaseName { get; set; }
    }

    public class DatabaseConnectionSettings : IDatabaseConnectionSettings
    {
        public string ConnectionString { get; set; }
        public string MSSQLConnectionString { get; set; }
        public string CertsConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
