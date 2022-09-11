using ProdigyBlockchain.Wallet.BusinessLayer.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    [Serializable]
    public class CryptoWallet
    {
        public string wallet_address { get; private set; }

        private string _pk { get; set; }

        private ICryptoService _CryptoService;

        private List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public decimal Amount
        {
            get
            {
                var toTransAmount = Transactions.Where(m => m.to == wallet_address).Sum(m => m.amount);
                var fromTransAmount = Transactions.Where(m => m.from == wallet_address).Sum(m => m.amount);

                return toTransAmount - fromTransAmount;
            }
        }

        public CryptoWallet(ICryptoService service)
        {
            _CryptoService = service;
        }

        public CryptoWallet(ICryptoService service, string address)
        {
            _CryptoService = service;
            wallet_address = address;
        }

        public CryptoWallet(ICryptoService service, WalletFile file, string address)
        {
            _CryptoService = service;
            wallet_address = address;
            _pk = file.pk;
        }

        public static CryptoWallet Load(ICryptoService service, string address, string passphrase)
        {
            try
            {
                CreateDirectoryIfNotExists();

                var file_name = Environment.CurrentDirectory + "\\Wallets\\" + address + ".json";

                if (!File.Exists(file_name))
                    return null;

                var encrypted_data = File.ReadAllText(file_name);

                var decrypted_data = service.DecrypData(encrypted_data, address);

                var wallet_file = JsonConvert.DeserializeObject<WalletFile>(decrypted_data);

                return new CryptoWallet(service, wallet_file, address);
            }
            catch(Exception)
            {
                throw new Exception("Could not load wallet");
            }

        }

        public static CryptoWallet Create(ICryptoService service, string passphrase)
        {
            var pair = service.CreateWalletKeyPair(passphrase);

            WalletFile wf = new WalletFile();
            wf.pk = pair.priv;

            service.Decrypt(pair.pub, pair.priv, passphrase);

            var new_wallet = new CryptoWallet(service, pair.pub);

            CreateDirectoryIfNotExists();

            var file_name = Environment.CurrentDirectory + "\\Wallets\\" + new_wallet.wallet_address + ".json";

            if (File.Exists(file_name))
                return null;


            var json = JsonConvert.SerializeObject(wf);

            var encrypted_data = service.EncryptData(json, pair.pub);

            File.WriteAllText(file_name, encrypted_data);

            return new_wallet;
        }

        private static void CreateDirectoryIfNotExists()
        {
            var directory = Environment.CurrentDirectory + "\\Wallets\\";

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
    }

    [Serializable]
    public class WalletFile
    {
        public string pk { get; set; }
    }

}
