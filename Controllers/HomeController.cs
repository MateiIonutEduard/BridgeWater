using BridgeWater.Data;
using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Security.Cryptography;
#pragma warning disable

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

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Results()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchProducts(string? name)
        {
            /* Search products list by using name filter */
            ProductResultModel result = await productService.GetProductsByNameAsync(name, null);
            if (!string.IsNullOrEmpty(name))
            {
                if (result.Results > 1)
                    return Redirect($"/Home/Results/?name={name}");
                else 
                if (result.Results == 1)
                    return Redirect($"/Home/About/?id={result.ProductViewModels[0].Id}");
                else
                    return Redirect("/");
            }
            else return Redirect("/");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Create(ProductModel productModel)
        {
            bool result = await productService.CreateProductAsync(productModel);
            if(result) return Redirect($"/Home/?id={productModel.categoryId}");
            else
            {
                ViewData["state"] = productModel;
                return View($"Views/Home/Create.cshtml", ViewData["state"]);
            }
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

                await postService.CreatePostAsync(postRatingModel);
                return Redirect($"/Home/About/?id={postRatingModel.productId}");
            }
            else
                return Redirect("/Account/");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> RemovePost(int pid)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if(!string.IsNullOrEmpty(userId))
            {
                int AccountId = Convert.ToInt32(userId);
                Post? post = await postService.GetPostAsync(pid);

                await postService.RemovePostAsync(AccountId, pid);
                return Redirect($"/Home/About/?id={post?.ProductId}");
            }
            else
                return Redirect("/Account/");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                int AccountId = Convert.ToInt32(userId);
                await productService.RemoveProductAsync(productId);
                return Redirect($"/Home/");
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
        public ActionResult ByCategory(string? category)
        {
            int CategoryId = Convert.ToInt32(category);
            /* Load products list filtered by category */
            if (CategoryId > 0) return Redirect($"/?type={category}");
            else return Redirect("/");
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