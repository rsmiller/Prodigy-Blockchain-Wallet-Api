using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Dto;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProdigyBlockchain.Wallet.BusinessLayer.Services
{
    public interface INodeDataService
    {
        Task<Response<List<BlockDto>>> GetLatestBlocks();
        Task<Response<List<Transaction>>> GetLatestTransactions();
        Task<Response<BlockDto>> GetBlockById(string block_id);
        Task<Response<List<BlockDto>>> GetBlocksByIdentifier(string identifier, string wildcard, bool include_data = false);
        Task<PagedResult<List<Transaction>>> GetWalletTransactions(string wallet_id, int page = 1);
        Task<PagedResult<List<Transaction>>> GetWalletTransactionsBySessionId(string session_id, int page = 1);
        Task<Response<Transaction>> GetTransaction(string txn);
        Response<MemoryStream> GetDocument(string block_id);
        Task<Response<GeneralSearchDto>> GeneralSearch(string wildcard);
        Task<Response<BlockPage>> SearchByCustomer(Guid customer_id, string wildcard, int page = 0);
    }


    public class NodeDataService : INodeDataService
    {
        private IWalletContext _IContext;
        private INodeSettings _NodeSettings;
        private IWalletSettings _WalletSettings;
        private RestClient _Client;

        private string _BaseUrl;

        public NodeDataService(IWalletContext context, INodeSettings nodeSettings, IWalletSettings walletSettings)
        {
            _IContext = context;
            _NodeSettings = nodeSettings;
            _WalletSettings = walletSettings;
            _BaseUrl = _NodeSettings.PrimaryNode + ":" + _NodeSettings.PrimaryNodePort + "/";
        }


        public async Task<Response<List<BlockDto>>> GetLatestBlocks()
        {
            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/GetLatestBlocks", Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            var response = await _Client.ExecuteAsync<Response<List<BlockDto>>>(request);

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Data;

            return new Response<List<BlockDto>>();
        }

        public async Task<Response<List<Transaction>>> GetLatestTransactions()
        {
            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/GetLatestTransactions", Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            var response = await _Client.ExecuteAsync<Response<List<Transaction>>>(request);

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Data;

            return new Response<List<Transaction>>();
        }

        public async Task<PagedResult<List<Transaction>>> GetWalletTransactions(string wallet_id, int page = 1)
        {
            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/GetWalletTransactions?wallet_id=" + wallet_id + "&page=" + page, Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            var response = await _Client.ExecuteAsync<PagedResult<List<Transaction>>>(request);

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Data;

            return new PagedResult<List<Transaction>>();
        }

        public async Task<PagedResult<List<Transaction>>> GetWalletTransactionsBySessionId(string session_id, int page = 1)
        {
            var session = await _IContext.UserSessions.Where(m => m.session_id == session_id).SingleOrDefaultAsync();
            if(session == null)
                return new PagedResult<List<Transaction>>("Session not found.");

            var user = await _IContext.Users.SingleAsync(m => m.id == session.user_id);


            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/GetWalletTransactions?wallet_id=" + user.wallet_address + "&page=" + page, Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            var response = await _Client.ExecuteAsync<PagedResult<List<Transaction>>>(request);

            if (response.StatusCode == HttpStatusCode.OK)
                return response.Data;

            return new PagedResult<List<Transaction>>();
        }

        public async Task<Response<BlockDto>> GetBlockById(string block_id)
        {
            Response<BlockDto> response = new Response<BlockDto>();

            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/GetBlockById?block_id="+ block_id, Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            var rest_response = await _Client.ExecuteAsync<Response<BlockDto>>(request);

            if (rest_response.StatusCode == HttpStatusCode.OK)
            {
                if(rest_response.Data.Data != null)
                    rest_response.Data.Data.Data = "";

                return rest_response.Data;
            }
            else
            {
                return new Response<BlockDto>("Not found");
            }
        }

        public async Task<Response<List<BlockDto>>> GetBlocksByIdentifier(string identifier, string wildcard, bool include_data = false)
        {
            Response<List<BlockDto>> response = new Response<List<BlockDto>>();
            response.Data = new List<BlockDto>();

            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/GetBlocksByIdentifier?identifier=" + identifier + "&wildcard=" + wildcard + "&include_data=" + include_data, Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            var rest_response = await _Client.ExecuteAsync<Response<List<BlockDto>>>(request);

            if (rest_response.StatusCode == HttpStatusCode.OK)
            {
                if(rest_response.Data.Success)
                {
                    foreach (var block in rest_response.Data.Data)
                    {
                        block.Data = "";
                        response.Data.Add(block);
                    }
                }
                else
                {
                    return rest_response.Data;
                }
            }
            else
            {
                return new Response<List<BlockDto>>("Not found");
            }

            return response;
        }

        public Response<MemoryStream> GetDocument(string block_id)
        {
            Response<MemoryStream> response = new Response<MemoryStream>();

            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/GetDocument?block_id=" + block_id, Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            try
            {
                var pdf_binary = _Client.DownloadData(request);

                var ms = new MemoryStream(pdf_binary);
                ms.Flush();
                ms.Position = 0;

                response.Data = ms;
            }
            catch(Exception ex)
            {
                return new Response<MemoryStream>(ex.Message);
            }

            return response;
        }

        public async Task<Response<Transaction>> GetTransaction(string txn)
        {
            Response<Transaction> response = new Response<Transaction>();

            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/GetTransaction?txn=" + txn, Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            var rest_response = await _Client.ExecuteAsync<Response<Transaction>>(request);

            if (rest_response.StatusCode == HttpStatusCode.OK)
                return rest_response.Data;
            else
                return new Response<Transaction>("Not found");
        }


        public async Task<Response<GeneralSearchDto>> GeneralSearch(string wildcard)
        {
            Response<GeneralSearchDto> response = new Response<GeneralSearchDto>();
            response.Data = new GeneralSearchDto();

            // Check the type of data then call the appropriate webservice
            Guid guid = Guid.Empty;

            if(Guid.TryParse(wildcard, out guid))
            {
                // Is guid so it could a node transaction or a block
                var block_response = await this.GetBlockById(guid.ToString());

                if(block_response != null)
                {
                    if(block_response.Success)
                    {
                        response.Data.return_type = "block";
                        response.Data.blocks.Add(block_response.Data);
                    }
                }

                if(response.Data.blocks.Count == 0)
                {
                    // Must be a transaction
                    var transaction_response = await this.GetWalletTransactions(guid.ToString());
                    if (transaction_response != null)
                    {
                        if (transaction_response.Success)
                        {
                            response.Data.return_type = "node-wallet";
                            if (transaction_response.Data != null)
                                response.Data.transactions = transaction_response.Data;
                        }
                    }
                }
            }
            else
            {
                // Not a guid so they are searching on a wallet id or txn or attribute
                if(!String.IsNullOrEmpty(wildcard) && wildcard.StartsWith(_WalletSettings.Prefix))
                {
                    // Wallet
                    var transaction_response = await this.GetWalletTransactions(wildcard);
                    if (transaction_response != null)
                    {
                        if (transaction_response.Success)
                        {
                            response.Data.return_type = "wallet";
                            response.Data.transactions = transaction_response.Data;
                        }
                    }
                }
                else
                {
                   
                    if(wildcard.Length > 50)
                    {
                        // Transaction
                        var transaction_response = await this.GetTransaction(wildcard);
                        if (transaction_response != null)
                        {
                            if (transaction_response.Success)
                            {
                                response.Data.return_type = "transaction";
                                
                                if(transaction_response.Data != null)
                                    response.Data.transactions.Add(transaction_response.Data);
                            }
                        }
                    }
                    else
                    {
                        // Attribute
                        var attribute_response = await this.GetBlocksByIdentifier("1", wildcard);
                        if (attribute_response != null)
                        {
                            if (attribute_response.Success)
                            {
                                response.Data.return_type = "attribute";
                                response.Data.blocks = attribute_response.Data;
                            }
                        }

                        if(response.Data.blocks.Count == 0)
                        {
                            // Try one more attribute search
                            var another_attribute_response = await this.GetBlocksByIdentifier("2", wildcard);
                            if (another_attribute_response != null)
                            {
                                if (another_attribute_response.Success)
                                {
                                    response.Data.return_type = "attribute";
                                    response.Data.blocks = attribute_response.Data;
                                }
                            }
                        }
                    }
                }
            }


            return response;
        }


        public async Task<Response<BlockPage>> SearchByCustomer(Guid customer_id, string wildcard, int page = 0)
        {
            Response<BlockPage> response = new Response<BlockPage>();
            response.Data = new BlockPage();

            _Client = new RestClient(_BaseUrl);
            var request = new RestRequest("api/Blockchain/SearchByCustomer?customer_id=" + customer_id + "&wildcard=" + wildcard + "&page=" + page, Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

            var rest_response = await _Client.ExecuteAsync<BlockPage>(request);

            if (rest_response.StatusCode == HttpStatusCode.OK)
            {
                if (rest_response.Data != null)
                    response.Data = rest_response.Data;
            }
            else
            {
                return new Response<BlockPage>("Not found");
            }

            return response;
        }
    }
}
