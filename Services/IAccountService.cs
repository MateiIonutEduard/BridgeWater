﻿using BridgeWater.Data;
using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IAccountService
    {
        Task<Account?> GetAccountAsync(int id);
        Task<bool> RemoveAccountAsync(int userId);
        Task<AccountResponseModel> GetAccountByWebcodeAsync(string webcode);
        Task<AccountResponseModel> UpdateAccountPasswordAsync(AccountRequestModel accountRequestModel);
        Task<AccountResponseModel> UpdateAccountPreferencesAsync(AccountRequestModel accountRequestModel);
        Task<AccountResponseModel> SignInAsync(AccountRequestModel accountRequestModel);
        Task<AccountResponseModel> SignUpAsync(AccountRequestModel accountRequestModel);
        Task<AccountResponseModel> SendWebcodeAsync(string address);
    }
}
