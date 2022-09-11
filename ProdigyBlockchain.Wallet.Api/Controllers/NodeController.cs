using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Dto;
using ProdigyBlockchain.Wallet.BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProdigyBlockchain.Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NodeController : ControllerBase
    {
        public INodeDataService _DataService;

        public NodeController(INodeDataService dataService)
        {
            _DataService = dataService;
        }

        [HttpGet("GetLatestBlocks", Name = "GetLatestBlocks")]
        [ProducesResponseType(typeof(Response<List<BlockDto>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetLatestBlocks()
        {
            var result = await _DataService.GetLatestBlocks();

            return new JsonResult(result);
        }

        [HttpGet("GetLatestTransactions", Name = "GetLatestTransactions")]
        [ProducesResponseType(typeof(Response<List<Transaction>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetLatestTransactions()
        {
            var result = await _DataService.GetLatestTransactions();

            return new JsonResult(result);
        }

        [HttpGet("GetBlockById", Name = "GetBlockById")]
        [ProducesResponseType(typeof(Response<BlockDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetBlockById([FromQuery] string block_id)
        {
            var result = await _DataService.GetBlockById(block_id);

            return new JsonResult(result);
        }

        [HttpGet("GetBlocksByIdentifier", Name = "GetBlocksByIdentifier")]
        [ProducesResponseType(typeof(Response<List<BlockDto>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetBlocksByIdentifier([FromQuery] string identifier, string wildcard, bool include_data = false)
        {
            var result = await _DataService.GetBlocksByIdentifier(identifier, wildcard, include_data);

            return new JsonResult(result);
        }

        [HttpGet("GetWalletTransactions", Name = "GetWalletTransactions")]
        [ProducesResponseType(typeof(PagedResult<List<Transaction>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetWalletTransactions([FromQuery] string wallet_id, int page = 0)
        {
            var result = await _DataService.GetWalletTransactions(wallet_id, page);

            return new JsonResult(result);
        }

        [HttpGet("GetWalletTransactionsBySessionId", Name = "GetWalletTransactionsBySessionId")]
        [ProducesResponseType(typeof(PagedResult<List<Transaction>>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetWalletTransactionsBySessionId([FromQuery] string session_id, int page = 0)
        {
            var result = await _DataService.GetWalletTransactionsBySessionId(session_id, page);

            return new JsonResult(result);
        }

        [HttpGet("GetTransaction", Name = "GetTransaction")]
        [ProducesResponseType(typeof(Response<Transaction>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GetTransaction([FromQuery] string txn)
        {
            var result = await _DataService.GetTransaction(txn);

            return new JsonResult(result);
        }

        [HttpGet("GetDocument", Name = "GetDocument")]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public ActionResult GetDocument([FromQuery] string block_id)
        {
            var result = _DataService.GetDocument(block_id);

            if (!result.Success)
                return BadRequest(result);
           
            return File(result.Data, "application/pdf");
        }

        [HttpGet("GeneralSearch", Name = "GeneralSearch")]
        [ProducesResponseType(typeof(Response<GeneralSearchDto>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GeneralSearch([FromQuery] string wildcard)
        {
            var result = await _DataService.GeneralSearch(wildcard);

            return new JsonResult(result);
        }

        [HttpGet("SearchByCustomer", Name = "SearchByCustomer")]
        [ProducesResponseType(typeof(Response<BlockPage>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> SearchByCustomer([FromQuery] Guid customer_id, string wildcard, int page = 0)
        {
            var result = await _DataService.SearchByCustomer(customer_id, wildcard, page);

            return new JsonResult(result);
        }
    }
}
