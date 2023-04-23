using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> Signin(AccountRequestModel accountRequestModel)
        {
            AccountResponseModel accountResponseModel = await accountService.SignInAsync(accountRequestModel);

            if (accountResponseModel.status < 0) return Redirect("/Account/Signup");
            else if (accountResponseModel.status == 0) return Redirect("/Account/?FailCode=true");
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
            return Redirect("/Home/");
		}
    }
}
