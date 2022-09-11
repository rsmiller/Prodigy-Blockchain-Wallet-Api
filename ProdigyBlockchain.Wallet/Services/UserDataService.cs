using ProdigyBlockchain.Wallet.BusinessLayer;
using ProdigyBlockchain.Wallet.BusinessLayer.Models;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Commands;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Dto;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProdigyBlockchain.Wallet.BusinessLayer.Models.Response;
using RestSharp;
using System.Net;

namespace ProdigyBlockchain.Wallet.Services
{
    public interface IUserDataService
    {
        Task<Response<bool>> RefreshUser(string session_id);
        Task<Response<CreateUserResponse>> CreateUser(CreateUserCommand command);
        Task<Response<UserDto>> LoadUser(UserLoginCommand command);
        Task<Response<WalletStatisticsDto>> GetWalletStatistics(string wallet_id);
        Task<Response<WalletStatisticsDto>> GetWalletStatisticsBySessionId(string session_id);
        Task<Response<bool>> RequestSpend(RequestSpendBySessionId command);
    }


    public class UserDataService : IUserDataService
    {
        private IWalletContext _IContext;

        private INodeSettings _NodeSettings;
        private RestClient _Client;

        private string _BaseUrl;

        public UserDataService(IWalletContext context, INodeSettings nodeSettings)
        {
            _IContext = context;
            _NodeSettings = nodeSettings;

            _BaseUrl = _NodeSettings.PrimaryNode + ":" + _NodeSettings.PrimaryNodePort + "/";
        }

        public async Task<Response<bool>> RefreshUser(string session_id)
        {
            var existing_session = await _IContext.UserSessions.Where(m => m.session_id == session_id).FirstOrDefaultAsync();

            if (existing_session == null)
                return new Response<bool>("User not found");

            existing_session.expires_on = DateTime.Now.AddDays(1);
            _IContext.UserSessions.Update(existing_session);
            await _IContext.SaveChangesAsync();

            return new Response<bool>(true);
        }

        public async Task<Response<CreateUserResponse>> CreateUser(CreateUserCommand command)
        {
            Response<CreateUserResponse> response = new Response<CreateUserResponse>();
            response.Data = new CreateUserResponse();

            var existing = await _IContext.Users.Where(m => m.username.ToLower() == command.username.ToLower()).FirstOrDefaultAsync();

            if (existing != null)
                return new Response<CreateUserResponse>("Username already exists");

            try
            {
                _Client = new RestClient(_BaseUrl);
                var request = new RestRequest("api/Wallet/CreateWallet", Method.POST, DataFormat.Json);
                request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);
                request.AddJsonBody(new CreateUserCommand()
                {
                    username = command.username,
                    password = command.password
                });


                var create_wallet_response = await _Client.ExecuteAsync<Response<WalletCreatedDto>>(request);

                if (create_wallet_response.StatusCode == HttpStatusCode.OK && create_wallet_response.Data.Data != null)
                {
                    var new_user = new User()
                    {
                        username = command.username,
                        wallet_address = create_wallet_response.Data.Data.wallet_id,
                        created_on = DateTime.Now,
                    };

                    await _IContext.Users.AddAsync(new_user);

                    await _IContext.SaveChangesAsync();


                    var session = new UserSession()
                    {
                        session_id = Guid.NewGuid().ToString(),
                        user_id = new_user.id,
                        expires_on = DateTime.Now.AddDays(1)
                    };

                    await _IContext.UserSessions.AddAsync(session);
                    await _IContext.SaveChangesAsync();

                    response.Data.user = new UserDto()
                    {
                        wallet_address = new_user.wallet_address,
                        user_id = new_user.id,
                        session_id = session.session_id
                    };

                    response.Data.session_id = session.session_id;
                }
                else
                {
                    throw new Exception("Could not create wallet");
                }
            }
            catch(Exception e)
            {
                return new Response<CreateUserResponse>(e.Message);
            }


            return response;
        }


        public async Task<Response<UserDto>> LoadUser(UserLoginCommand command)
        {
            Response<UserDto> response = new Response<UserDto>();

            var existing = await _IContext.Users.Where(m => m.username.ToLower() == command.username.ToLower()).FirstOrDefaultAsync();

            if (existing == null)
                return new Response<UserDto>("User doesn't exists");

            try
            {
                _Client = new RestClient(_BaseUrl);
                var wallet_request = new RestRequest("api/Wallet/GetWallet", Method.POST, DataFormat.Json);
                wallet_request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);
                wallet_request.AddJsonBody(new GetWalletCommand()
                {
                    wallet_address = existing.wallet_address,
                    username = existing.username,
                    passphrase = command.password
                });

                var get_wallet_response = await _Client.ExecuteAsync<Response<WalletStatisticsDto>>(wallet_request);

                if (get_wallet_response.StatusCode == HttpStatusCode.OK && get_wallet_response.Data.Data != null)
                {
                    _Client = new RestClient(_BaseUrl);
                    var request = new RestRequest("api/Wallet/GetWalletTransactions?wallet_id=" + existing.wallet_address, Method.GET, DataFormat.Json);
                    request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

                    var rest_response = await _Client.ExecuteAsync<Response<List<Transaction>>>(request);

                    if (rest_response.StatusCode == HttpStatusCode.OK)
                    {
                        if (rest_response.Data.Success == true)
                        {
                            response.Data = new UserDto()
                            {
                                wallet_address = existing.wallet_address,
                                user_id = existing.id,
                                transaction = rest_response.Data.Data,
                                wallet_total = get_wallet_response.Data.Data.balance,
                                pending = get_wallet_response.Data.Data.pending
                            };

                            var existing_session = await _IContext.UserSessions.Where(m => m.user_id == existing.id).SingleOrDefaultAsync();

                            if (existing_session != null)
                            {
                                if (existing_session.expires_on <= DateTime.Now)
                                {
                                    // Update existing session with a new date
                                    existing_session.expires_on = DateTime.Now.AddDays(1);
                                    _IContext.UserSessions.Update(existing_session);
                                    await _IContext.SaveChangesAsync();
                                }

                                response.Data.session_id = existing_session.session_id;
                            }
                            else
                            {
                                var session = new UserSession()
                                {
                                    session_id = Guid.NewGuid().ToString(),
                                    user_id = response.Data.user_id,
                                    expires_on = DateTime.Now.AddDays(1)
                                };

                                await _IContext.UserSessions.AddAsync(session);
                                await _IContext.SaveChangesAsync();

                                response.Data.session_id = session.session_id;
                            }
                        }
                        else
                        {
                            throw new Exception(rest_response.Data.Exception);
                        }
                    }
                    else
                    {
                        throw new Exception("Error communicating with node");
                    }


                    existing.last_login_on = DateTime.Now;
                    _IContext.Users.Update(existing);
                    await _IContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Could not get wallet");
                }
            }
            catch (Exception e)
            {
                return new Response<UserDto>(e.Message);
            }


            return response;
        }

        public async Task<Response<WalletStatisticsDto>> GetWalletStatisticsBySessionId(string session_id)
        {
            var session_record = await _IContext.UserSessions.Where(m => m.session_id == session_id).FirstOrDefaultAsync();
            if (session_record == null)
                return new Response<WalletStatisticsDto>("You need to log in?!");

            var user = await _IContext.Users.Where(m => m.id == session_record.user_id).FirstOrDefaultAsync();

            if (user == null)
                return new Response<WalletStatisticsDto>("User doesn't exists");

            return await GetWalletStatistics(user.wallet_address);
        }

        public async Task<Response<WalletStatisticsDto>> GetWalletStatistics(string wallet_id)
        {
            Response<WalletStatisticsDto> response = new Response<WalletStatisticsDto>();

            try
            {
                _Client = new RestClient(_BaseUrl);
                var wallet_request = new RestRequest("api/Wallet/GetWalletStatistics?wallet_id=" + wallet_id, Method.GET, DataFormat.Json);
                wallet_request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);

                var rest_response = await _Client.ExecuteAsync<Response<WalletStatisticsDto>>(wallet_request);

                if (rest_response.StatusCode == HttpStatusCode.OK && rest_response.Data.Data != null)
                {
                    response = rest_response.Data;
                }
                else
                {
                    throw new Exception("Could not get wallet stats");
                }
            }
            catch (Exception e)
            {
                return new Response<WalletStatisticsDto>(e.Message);
            }

            return response;
        }

        public async Task<Response<bool>> RequestSpend(RequestSpendBySessionId command)
        {
            try
            {
                var existing_session = await _IContext.UserSessions.Where(m => m.session_id == command.session_id).FirstOrDefaultAsync();

                if (existing_session == null)
                    return new Response<bool>("Session not found");

                var existing_user = await _IContext.Users.Where(m => m.id == existing_session.user_id).FirstOrDefaultAsync();

                if (existing_user == null)
                    return new Response<bool>("User not found");

                _Client = new RestClient(_BaseUrl);
                var request = new RestRequest("api/Wallet/RequestSpend", Method.POST, DataFormat.Json);
                request.AddHeader("Authorization", "JWT " + _NodeSettings.NodePassword);
                request.AddJsonBody(new RequestSpendCommand()
                {
                    wallet_id = existing_user.wallet_address,
                    amount = command.amount,
                    note = command.note
                });

                var rest_response = await _Client.ExecuteAsync<Response<bool>>(request);

                if (rest_response.StatusCode == HttpStatusCode.OK)
                {
                    return rest_response.Data;
                }
                else
                {
                    throw new Exception("Could not spend");
                }
            }
            catch (Exception e)
            {
                return new Response<bool>(e.Message);
            }
        }
    }
}
