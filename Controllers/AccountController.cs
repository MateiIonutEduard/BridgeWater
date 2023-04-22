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

        [HttpPost]
        public async Task<IActionResult> Signin(AccountRequestModel accountRequestModel)
        {
            AccountResponseModel accountResponseModel = await accountService.SignInAsync(accountRequestModel);
            return View();
        }
    }
}
