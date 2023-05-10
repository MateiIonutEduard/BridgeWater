using System.IO;
using System.Net;
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

        public async Task<AccountResponseModel> UpdateAccountPreferencesAsync(AccountRequestModel accountRequestModel)
        {
            Account? account = await bridgeContext.Account
                .FirstOrDefaultAsync(e => e.Id == accountRequestModel.Id.Value);

            AccountResponseModel accountResponseModel = new AccountResponseModel();
            if (accountRequestModel.password.CompareTo(accountRequestModel.confirmPassword) != 0)
            {
                // passwords do not match
                accountResponseModel.status = -1;
                return accountResponseModel;
            }

            // account exists, so update it
            if (account != null)
            {
                string encryptedPassword = cryptoService.Encrypt(accountRequestModel.password);
                string avatarPath = "./Storage/Account/avatar.png";
                bool updateAvatar = false;

                // copy avatar image first
                if (accountRequestModel.avatar != null)
                {
                    avatarPath = $"./Storage/Account/{accountRequestModel.avatar.FileName}";
                    MemoryStream ms = new MemoryStream();

                    /* save avatar logo at disk, when hash 
                       functions have different values. */
                    await accountRequestModel.avatar.CopyToAsync(ms);
                    byte[] oldData = await File.ReadAllBytesAsync(account.Avatar);

                    string lhash = cryptoService.ComputeHash(oldData);
                    string rhash = cryptoService.ComputeHash(ms.ToArray());

                    if(lhash.CompareTo(rhash) != 0)
                    {
                        // remove if avatar is not default image
                        if (!account.Avatar.EndsWith("avatar.png"))
                            File.Delete(account.Avatar);

                        // write new image file at disk
                        System.IO.File.WriteAllBytes(avatarPath, ms.ToArray());
                        updateAvatar = true;
                    }
                }

                account.Address = accountRequestModel.address;
                account.Username = accountRequestModel.username;

                account.Password = encryptedPassword;
                account.IsAdmin = accountRequestModel.admin;

                // update success
                if(updateAvatar) account.Avatar = avatarPath;
                await bridgeContext.SaveChangesAsync();

                accountResponseModel.id = account.Id;
                accountResponseModel.username = account.Username;
                
                accountResponseModel.admin = accountRequestModel.admin;
                accountResponseModel.status = 1;
            }
            else
            {
                // account does not exists!
                accountResponseModel.status = 0;
                return accountResponseModel;
            }

            return accountResponseModel;
        }

        public async Task<bool> RemoveAccountAsync(int userId)
        {
            Account? account = await bridgeContext.Account
                .FirstOrDefaultAsync(e => e.Id == userId);

            if(account != null)
            {
                List<Post> posts = await bridgeContext.Post.Where(e => e.AccountId == userId)
                    .ToListAsync();

                // remove all post rating of this account
                bridgeContext.Post.RemoveRange(posts);
                await bridgeContext.SaveChangesAsync();

                List<Order> orders = await bridgeContext.Order.Where(e => e.AccountId == userId)
                    .ToListAsync();

                for(int k = 0; k < orders.Count; k++)
                {
                    Product? product = await bridgeContext.Product
                        .FirstOrDefaultAsync(e => e.Id == orders[k].ProductOrderId);

                    // update product quantity before removes
                    if ((orders[k].IsCanceled == null) || (orders[k].IsCanceled != null && !orders[k].IsCanceled.Value))
                    {
                        if(product != null)
                        {
                            product.Stock += orders[k].Stock;
                            bridgeContext.Product.Remove(product);
                            await bridgeContext.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        // remove without change quantity, because was updated in the past 
                        bridgeContext.Product.Remove(product);
                        await bridgeContext.SaveChangesAsync();
                    }

                }

                // remove if avatar is not default image
                if (!account.Avatar.EndsWith("avatar.png"))
                    File.Delete(account.Avatar);

                // now remove account entity from database
                bridgeContext.Account.Remove(account);
                await bridgeContext.SaveChangesAsync();
            }

            // account does not exists
            return false;
        }

        public async Task<Account?> GetAccountAsync(int id)
        {
            /* get user account */
            Account? account = await bridgeContext.Account
                .FirstOrDefaultAsync(u => u.Id == id);

            account.Password = cryptoService.Decrypt(account.Password);
            return account;
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
                    accountResponseModel.admin = account.IsAdmin;
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
                    Password = cryptoService.Encrypt(accountRequestModel.password),
                    Address = accountRequestModel.address,
                    IsAdmin = accountRequestModel.admin,
                    Avatar = avatarPath
                };

                bridgeContext.Account.Add(account);
                await bridgeContext.SaveChangesAsync();

                // new account was created
                accountResponseModel.id = account.Id;
                accountResponseModel.username = account.Username;
                accountResponseModel.admin = account.IsAdmin;
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

        public async Task<AccountResponseModel> UpdateAccountPasswordAsync(AccountRequestModel accountRequestModel)
        {
            AccountResponseModel accountResponseModel = new AccountResponseModel();

            // passwords does not match
            if (accountRequestModel.password.CompareTo(accountRequestModel.confirmPassword) != 0)
                accountResponseModel.status = -1;
            else
            {
                Account? account = await bridgeContext.Account
                    .FirstOrDefaultAsync(e => e.Id == accountRequestModel.Id);

                if (account != null)
                {
                    // update account info
                    accountResponseModel.id = account.Id;
                    accountResponseModel.username = account.Username;
                    accountResponseModel.admin = account.IsAdmin;

                    // update password successfully
                    account.Password = cryptoService.Encrypt(accountRequestModel.password);
                    await bridgeContext.SaveChangesAsync();
                    accountResponseModel.status = 1;
                }
                else
                    /* account not found */
                    accountResponseModel.status = 0;
            }

            // returns response model to maintains the logic
            return accountResponseModel;
        }

        public async Task<AccountResponseModel> GetAccountByWebcodeAsync(string webcode)
        {
            Account? account = await bridgeContext.Account
                .FirstOrDefaultAsync(e => e.Webcode.CompareTo(webcode) == 0);

            if(account != null)
            {
                AccountResponseModel accountResponseModel = new AccountResponseModel();
                accountResponseModel.username = account.Username;

                accountResponseModel.id = account.Id;
                accountResponseModel.status = 1;

                accountResponseModel.admin = account.IsAdmin;
                return accountResponseModel;
            }

            return null;
        }

        public async Task<AccountResponseModel> SendWebcodeAsync(string address)
        {
            Account? account = await bridgeContext.Account
                .FirstOrDefaultAsync(e => e.Address.CompareTo(address) == 0);

            var accountResponseModel = new AccountResponseModel();

            if(account != null)
            {
                string webcode = Guid.NewGuid().ToString();
                account.Webcode = webcode;
                await bridgeContext.SaveChangesAsync();

                string body = $@"
                    Hi {account.Username}!<br> 
                    <p style='color: #272a35;'>
                        Your webcode is <b style='color: #5f9ea0 !important;'>{webcode}</b>.<br>
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
