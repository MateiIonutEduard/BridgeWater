using BridgeWater.Data;
using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BridgeWater.Controllers
{
    public class HomeController : Controller
    {
        readonly IPostService postService;
        readonly IProductService productService;
        readonly IAccountService accountService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IAccountService accountService, IPostService postService, IProductService productService, ILogger<HomeController> logger)
        {
            this.accountService = accountService;
            this.productService = productService;
            this.postService = postService;
            _logger = logger;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreatePost(PostRatingModel postRatingModel)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if (userId != null)
            {
                // Creates new post with rating
                int AccountId = Convert.ToInt32(userId);
                postRatingModel.accountId = AccountId;

                int res = await postService.CreatePostAsync(postRatingModel);
                return Redirect($"/Home/About/?id={postRatingModel.productId}");
            }
            else
                return Redirect("/Account/");
        }

        // show product image, logo or poster
        public async Task<IActionResult> Show(int id, bool? logo)
        {
            Product? product = await productService.GetProductAsync(id);

            if (product != null && logo != null)
            {
                if (logo.Value)
                {
                    int index = product.LogoImage.LastIndexOf(".");
                    byte[] data = System.IO.File.ReadAllBytes(product.LogoImage);
                    return File(data, $"image/{product.LogoImage.Substring(index + 1)}");
                }
                else
                {
                    int index = product.PosterImage.LastIndexOf(".");
                    byte[] data = System.IO.File.ReadAllBytes(product.PosterImage);
                    return File(data, $"image/{product.PosterImage.Substring(index + 1)}");
                }
            }
            
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> ByCategory(string? category)
        {
            await accountService.RecoverPasswordAsync("eduardmatei@outlook.com");
            /* Load products list filtered by category */
            return Redirect($"/?type={category}");
        }

        // Represents product description page view
        public IActionResult About()
        {
            return View();
        }

        // Main page that offers product list view
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}