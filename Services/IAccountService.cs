﻿using BridgeWater.Data;
using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IAccountService
    {
        Task<Account?> GetAccountAsync(int id);
        Task<AccountResponseModel> SignInAsync(AccountRequestModel accountRequestModel);
        Task<AccountResponseModel> SignUpAsync(AccountRequestModel accountRequestModel);
        Task<AccountResponseModel> RecoverPasswordAsync(string address);
    }
}
