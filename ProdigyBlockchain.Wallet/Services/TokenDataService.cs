using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Services
{
    public interface ITokenDataService
    {
        Response<Token> Request(string username, string password);
    }

    public class TokenDataService : ITokenDataService
    {
        public static string Issuer { get { return "PBC"; } }
        public static int Expires { get { return 1000; } }

        private IAuthenticationSettings _AuthenticationSettings;

        public TokenDataService(IAuthenticationSettings settings)
        {
            _AuthenticationSettings = settings;
        }

        public Response<Token> Request(string username, string password)
        {
            Response<Token> response = new Response<Token>();

            try
            {
                if (username == _AuthenticationSettings.APIUsername && password == _AuthenticationSettings.APIPassword)
                    response.Data = CreateSecurityToken(_AuthenticationSettings.APIPrivateKey);
                else
                    response.Success = false;
            }
            catch (Exception e)
            {
                response.SetException(e);
            }

            return response;
        }

        public static SymmetricSecurityKey CreateSecurityKey(string privateKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
        }

        public static Token CreateSecurityToken(string privateKey)
        {
            return new Token(new JwtSecurityToken(
                issuer: TokenDataService.Issuer,
                expires: DateTime.Now.AddMinutes(TokenDataService.Expires),
                signingCredentials: new SigningCredentials(TokenDataService.CreateSecurityKey(privateKey), SecurityAlgorithms.HmacSha256Signature)));
        }

        public static string CreateAPIKey(string username, string password)
        {
            return HashString(username + ":" + password);
        }

        public static string HashPassword(string password)
        {
            return HashString(password);
        }

        private static string HashString(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
