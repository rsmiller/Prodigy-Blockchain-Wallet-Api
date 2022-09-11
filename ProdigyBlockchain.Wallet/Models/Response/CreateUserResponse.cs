using ProdigyBlockchain.Wallet.BusinessLayer.Models.Dto;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Models.Response
{
    public class CreateUserResponse
    {
        public UserDto user { get; set; }
        public string session_id { get; set; }
    }
}
