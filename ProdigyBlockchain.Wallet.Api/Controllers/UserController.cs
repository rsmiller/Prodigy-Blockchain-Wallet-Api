using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Commands;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Dto;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Response;
using ProdigyBlockchain.Wallet.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdigyBlockchain.Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserDataService _DataService;

        public UserController(IUserDataService dataService)
        {
            _DataService = dataService;
        }


        [HttpPost("CreateUser", Name = "CreateUser")]
        [ProducesResponseType(typeof(Response<CreateUserResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _DataService.CreateUser(command);

            return Ok(result);
        }

        [HttpPost("GetUser", Name = "GetUser")]
        [ProducesResponseType(typeof(Response<UserDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetUser([FromBody] UserLoginCommand command)
        {
            var result = await _DataService.LoadUser(command);

            return new JsonResult(result);
        }

        [HttpGet("RefreshUser", Name = "RefreshUser")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> RefreshUser([FromQuery] string session_id)
        {
            var response = await _DataService.RefreshUser(session_id);

            return Ok(response);
        }

        [HttpGet("GetWalletStatistics", Name = "GetWalletStatistics")]
        [ProducesResponseType(typeof(Response<WalletStatisticsDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetWalletStatistics([FromQuery] string wallet_id)
        {
            var response = await _DataService.RefreshUser(wallet_id);

            return Ok(response);
        }


        [HttpGet("GetWalletStatisticsBySessionId", Name = "GetWalletStatisticsBySessionId")]
        [ProducesResponseType(typeof(Response<WalletStatisticsDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetWalletStatisticsBySessionId([FromQuery] string session_id)
        {
            var response = await _DataService.GetWalletStatisticsBySessionId(session_id);

            return Ok(response);
        }

        [HttpPost("RequestSpend", Name = "RequestSpend")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> RequestSpend([FromBody] RequestSpendBySessionId command)
        {
            var result = await _DataService.RequestSpend(command);

            return new JsonResult(result);
        }
    }
}
