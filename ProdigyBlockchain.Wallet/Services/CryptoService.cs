using Crypto.RIPEMD;
using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Services
{
    public interface ICryptoService
    {
		byte[] GenerateHash(byte[] hashingData);
		KeyPair CreateWalletKeyPair(string passphrase);
		bool Decrypt(string publicKey, string privateKey, string passphrase);
		string EncryptData(string Data, string publicKey);
		string DecrypData(string data, string publicKey);
	}

	public class CryptoService : ICryptoService
	{
		private string _PrivateKey = "pb";
		private IWalletSettings _Settings;

		public CryptoService(IWalletSettings settings)
        {
			_Settings = settings;
			_PrivateKey = settings.PrivateKey;
		}


		public byte[] GenerateHash(byte[] hashingData)
		{
			using (SHA256 sha256 = new SHA256Managed())
			{
				using (SHA512 sha512 = new SHA512Managed())
				{
					return sha512.ComputeHash(sha256.ComputeHash(hashingData));
				}
			}
		}

		public KeyPair CreateWalletKeyPair(string passphrase)
        {
			KeyPair pair = new KeyPair();

            RijndaelManaged objrij = new RijndaelManaged();

            objrij.Mode = CipherMode.CBC;
            objrij.Padding = PaddingMode.PKCS7;
            objrij.KeySize = 0x80;
            objrij.BlockSize = 0x80;

            objrij.GenerateKey();

            pair.priv = Convert.ToBase64String(objrij.Key);
            pair.pub = _Settings.Prefix + "0" + HashMe(pair.priv, passphrase);

			return pair;
		}

		public bool Decrypt(string publicKey, string privateKey, string passphrase)
        {
			string cleaned_public = publicKey.Replace(_Settings.Prefix + "0", "");

			string hashedString = HashMe(privateKey, passphrase);

			if(cleaned_public == hashedString)
            {
				return true;
            }

			return false;
		}

		public string EncryptData(string Data, string publicKey)
		{
			RijndaelManaged objrij = new RijndaelManaged();

			objrij.Mode = CipherMode.CBC;
			objrij.Padding = PaddingMode.PKCS7;
			objrij.KeySize = 0x80;
			objrij.BlockSize = 0x80;

			byte[] passBytes = Encoding.UTF8.GetBytes(_PrivateKey + "-:!:-" + publicKey);

			byte[] EncryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			int len = passBytes.Length;
			if (len > EncryptionkeyBytes.Length)
			{
				len = EncryptionkeyBytes.Length;
			}
			Array.Copy(passBytes, EncryptionkeyBytes, len);
			objrij.Key = EncryptionkeyBytes;
			objrij.IV = EncryptionkeyBytes;

			ICryptoTransform objtransform = objrij.CreateEncryptor();
			byte[] textDataByte = Encoding.UTF8.GetBytes(Data);

			return Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));
		}

		public string DecrypData(string data, string publicKey)
		{
			RijndaelManaged objrij = new RijndaelManaged();

			objrij.Mode = CipherMode.CBC;
			objrij.Padding = PaddingMode.PKCS7;
			objrij.KeySize = 0x80;
			objrij.BlockSize = 0x80;

			byte[] encryptedTextByte = Convert.FromBase64String(data);
			byte[] passBytes = Encoding.UTF8.GetBytes(_PrivateKey + "-:!:-" + publicKey);
			byte[] encryptionkeyBytes = new byte[0x10];
			int len = passBytes.Length;
			if (len > encryptionkeyBytes.Length)
			{
				len = encryptionkeyBytes.Length;
			}
			Array.Copy(passBytes, encryptionkeyBytes, len);
			objrij.Key = encryptionkeyBytes;
			objrij.IV = encryptionkeyBytes;
			byte[] TextByte = objrij.CreateDecryptor().TransformFinalBlock(encryptedTextByte, 0, encryptedTextByte.Length);

			return Encoding.UTF8.GetString(TextByte);
		}

		private string HashMe(string privateKey, string password)
        {
			RIPEMD160 r160 = new RIPEMD160Managed();

			byte[] myByte = System.Text.Encoding.ASCII.GetBytes(privateKey + "-:-" + password);
			// compute the byte to RIPEMD160 hash
			byte[] encrypted = r160.ComputeHash(myByte);
			// create a new StringBuilder process the hash byte
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < encrypted.Length; i++)
			{
				sb.Append(encrypted[i].ToString("X2"));
			}

			return sb.ToString();
		}
	}
}
