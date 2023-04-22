using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IAccountService
    {
        Task<AccountResponseModel> SignInAsync(AccountRequestModel accountRequestModel);
        Task<AccountResponseModel> SignUpAsync(AccountRequestModel accountRequestModel);
        Task<AccountResponseModel> RecoverPasswordAsync(string address);
    }
}
