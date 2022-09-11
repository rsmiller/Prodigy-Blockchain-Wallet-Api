using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using ProdigyBlockchain.Wallet.BusinessLayer.Services;
using System;

namespace ProdigyBlockchainWallet.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating new wallet");

            var walletSettings = new WalletSettings();
            walletSettings.PrivateKey = "PBC123456789!!!$%";
            walletSettings.Prefix = "pb";

            var service = new CryptoService(walletSettings);

            var wallet = CryptoWallet.Create(service, "password!1");

            Console.WriteLine(wallet.wallet_address);

            var loaded_wallet = CryptoWallet.Load(service, wallet.wallet_address, "123123123123");

            Console.WriteLine(loaded_wallet.wallet_address);
        }
    }
}
