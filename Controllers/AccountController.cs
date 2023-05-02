using BridgeWater.Data;
using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
#pragma warning disable

namespace BridgeWater.Controllers
{
    public class AccountController : Controller
    {
        readonly IAccountService accountService;

		public AccountController(IAccountService accountService)
        { this.accountService = accountService; }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        public IActionResult Recover()
        {
            return View();
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Remove()
        {
			string? userId = HttpContext.User?.Claims?
	            .FirstOrDefault(u => u.Type == "id")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                /* removes specified account */
                int UserId = Convert.ToInt32(userId);
                await accountService.RemoveAccountAsync(UserId);
				await HttpContext.SignOutAsync();
			}

            return Redirect("/Account/");
		}

        [Authorize]
        public async Task<IActionResult> Preferences()
        {
			string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            int UserId = Convert.ToInt32(userId);
            Account account = await accountService.GetAccountAsync(UserId);

            bool failCode = HttpContext.Request.Query.ContainsKey("FailCode") ? 
                Convert.ToBoolean(HttpContext.Request.Query["FailCode"]) : false;

			if (!failCode)
                HttpContext.Session.Clear();

			ViewData["state"] = account;
			return View("Views/Account/Preferences.cshtml", ViewData["state"]);
		}

        [HttpPost, Authorize]
		public async Task<IActionResult> Preferences(AccountRequestModel accountRequestModel)
        {
			string? userId = HttpContext.User?.Claims?
	            .FirstOrDefault(u => u.Type == "id")?.Value;

			int UserId = Convert.ToInt32(userId);
            accountRequestModel.Id = UserId;
			AccountResponseModel accountResponseModel = await accountService.UpdateAccountPreferencesAsync(accountRequestModel);

            if (accountResponseModel.status < 0)
            {
                // password does not match
                HttpContext.Session.SetString("confirmPassword", accountRequestModel.confirmPassword);
				return Redirect("/Account/Preferences/?FailCode=true");
            }
            /* User are not logged in */
            else if (accountResponseModel.status == 0)
                return Redirect("/Account/");

            // account preferences was updated successfully
            await HttpContext.SignOutAsync();
            var claims = new Claim[]
{
                new Claim("id", accountResponseModel.id.Value.ToString()),
                new Claim(ClaimTypes.Name, accountResponseModel.username),
                new Claim("admin", accountResponseModel.admin.Value.ToString())
};

            var identity = new ClaimsIdentity(claims, "User Identity");
            var userPrincipal = new ClaimsPrincipal(new[] { identity });
            await HttpContext.SignInAsync(userPrincipal);
            return Redirect("/Home/");
        }


		public async Task<IActionResult> Show(int id)
        {
            Account? account = await accountService.GetAccountAsync(id);
            int index = account.Avatar.LastIndexOf(".");

            byte[] data = System.IO.File.ReadAllBytes(account.Avatar);
            return File(data, $"image/{account.Avatar.Substring(index + 1)}");
        }

        [HttpPost]
        public async Task<IActionResult> Signin(AccountRequestModel accountRequestModel)
        {
            AccountResponseModel accountResponseModel = await accountService.SignInAsync(accountRequestModel);

            if (accountResponseModel.status < 0) return Redirect("/Account/Signup");
            else if (accountResponseModel.status == 0) return Redirect("/Account/?FailCode=true");
			var claims = new Claim[]
            {
				new Claim("id", accountResponseModel.id.Value.ToString()),
				new Claim(ClaimTypes.Name, accountResponseModel.username),
				new Claim("admin", accountResponseModel.admin.Value.ToString())
            };

			var identity = new ClaimsIdentity(claims, "User Identity");
			var userPrincipal = new ClaimsPrincipal(new[] { identity });
			await HttpContext.SignInAsync(userPrincipal);
			return Redirect("/Home/");
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountRequestModel accountRequestModel)
        {
            AccountResponseModel accountResponseModel = await accountService.SignUpAsync(accountRequestModel);
            if (accountResponseModel.status == -1)
            {
				ViewData["state"] = accountRequestModel;
                return View("Views/Account/Signup.cshtml", ViewData["state"]);
			}

            if (accountResponseModel.status <= 0) return Redirect("/Account/Signup/?FailCode=0");

			var claims = new Claim[]
			{
				new Claim("id", accountResponseModel.id.Value.ToString()),
				new Claim(ClaimTypes.Name, accountResponseModel.username),
				new Claim("admin", accountResponseModel.admin.Value.ToString())
            };

			var identity = new ClaimsIdentity(claims, "User Identity");
			var userPrincipal = new ClaimsPrincipal(new[] { identity });
			await HttpContext.SignInAsync(userPrincipal);
			return Redirect("/Home/");
		}

        [HttpPost]
        public async Task<IActionResult> Send(string address)
        {
            AccountResponseModel accountResponseModel = await accountService.RecoverPasswordAsync(address);
            if(accountResponseModel.status == -1) Redirect("/Account/Signup");
            return Redirect("/Account/");
        }

        public async Task<IActionResult> Signout()
        {
			await HttpContext.SignOutAsync();
            return Redirect("/Account/");
		}
    }
}
