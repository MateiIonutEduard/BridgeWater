using System.IO;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using BridgeWater.Data;
using BridgeWater.Models;
using Microsoft.EntityFrameworkCore;
#pragma warning disable

namespace BridgeWater.Services
{
    public class AccountService : IAccountService
    {
        readonly IAdminService adminService;
        readonly ICryptoService cryptoService;
        readonly BridgeContext bridgeContext;

        public AccountService(BridgeContext bridgeContext, IAdminService adminService, ICryptoService cryptoService)
        {
            this.adminService = adminService;
            this.cryptoService = cryptoService;
            this.bridgeContext = bridgeContext;
        }

        public async Task<AccountResponseModel> SignInAsync(AccountRequestModel accountRequestModel)
        {
            Account? account = await bridgeContext.Account
                .FirstOrDefaultAsync(e => e.Address == accountRequestModel.address);

            AccountResponseModel accountResponseModel = new AccountResponseModel();
            string encryptedPassword = cryptoService.Encrypt(accountRequestModel.password);

            if (account != null)
            {
                if(encryptedPassword.CompareTo(account.Password) != 0)
                {
                    /* lost password */
                    accountResponseModel.status = 0;
                }
                else
                {
                    /* account is signed in successfully */
                    accountResponseModel.status = 1;
                    accountResponseModel.username = account.Username;
                    accountResponseModel.id = account.Id;
                }
            }
            else
            {
                /* account does not exists */
                accountResponseModel.status = -1;
            }

            return accountResponseModel;
        }

        public async Task<AccountResponseModel> SignUpAsync(AccountRequestModel accountRequestModel)
        {
            AccountResponseModel accountResponseModel = new AccountResponseModel();
            if(accountRequestModel.password.CompareTo(accountRequestModel.confirmPassword) != 0)
            {
				// passwords do not match
				accountResponseModel.status = -1;
                return accountResponseModel;
            }
            string encryptedPassword = cryptoService.Encrypt(accountRequestModel.password);
            string avatarPath = "./Storage/Account/avatar.png";

            // check if username or password is taken
            Account account = await bridgeContext.Account
                .FirstOrDefaultAsync(e => e.Address == accountRequestModel.address || e.Username == accountRequestModel.username);
            
            if(account == null)
            {
                // copy avatar image first
                if (accountRequestModel.avatar != null)
                {
                    avatarPath = $"./Storage/Account/{accountRequestModel.avatar.FileName}";
                    MemoryStream ms = new MemoryStream();

                    // save avatar logo at disk
                    await accountRequestModel.avatar.CopyToAsync(ms);
                    System.IO.File.WriteAllBytes(avatarPath, ms.ToArray());
                }

                account = new Account
                {
                    Username = accountRequestModel.username,
                    Password = accountRequestModel.password,
                    Address = accountRequestModel.address,
                    IsAdmin = accountRequestModel.admin,
                    Avatar = avatarPath
                };

                bridgeContext.Account.Add(account);
                await bridgeContext.SaveChangesAsync();

                // new account was created
                accountResponseModel.id = account.Id;
                accountResponseModel.username = account.Username;
                accountResponseModel.status = 1;
            }
            else
            {
                // user account is registered
                accountResponseModel.status = 0;
            }

            // response object
            return accountResponseModel;
        }

        public async Task<AccountResponseModel> RecoverPasswordAsync(string address)
        {
            Account? account = await bridgeContext.Account
                .FirstOrDefaultAsync(e => e.Address.CompareTo(address) == 0);

            var accountResponseModel = new AccountResponseModel();

            if(account != null)
            {
                string password = cryptoService.Decrypt(account.Password);

                string body = $@"
                    Hi {account.Username}!<br> 
                    <p style='color: #272a35;'>
                        Your password is <b style='color: #5f9ea0 !important;'>{password}</b>.<br>
                        Have a nice day!
                    </p><br/>
                    <p style='color: #4a606d;'>
                        Best Regards,<br/>
                            <small style='color: #54669f !important; margin-left: 10px;'>BridgeWater Team Support</small>
                    </p>
                ";

                // status code is measured by admin service result code
                int res = adminService.SendEmail(account.Address, "BridgeWater Support", body);
                accountResponseModel.status = res < 1 ? 0 : 1;
            }
            else
            {
                // something is wrong, cannot find user account
                accountResponseModel.status = -1;
            }

            return accountResponseModel;
        }
    }
}
