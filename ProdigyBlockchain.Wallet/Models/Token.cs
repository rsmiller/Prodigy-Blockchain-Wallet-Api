using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models
{
    public class Token
    {
        private JwtSecurityToken token;

        public long ValidTo { get { return DateTimeOffset.Parse(token.ValidTo.ToString()).ToUnixTimeMilliseconds(); } }
        public string Value { get { return new JwtSecurityTokenHandler().WriteToken(token); } }

        public Token(JwtSecurityToken securityToken)
        {
            token = securityToken;
        }
    }
}
