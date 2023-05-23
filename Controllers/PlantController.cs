using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable

namespace BridgeWater.Controllers
{
    public class PlantController : Controller
    {
        PlantService plantService;

        public PlantController(PlantService plantService)
        { this.plantService = plantService; }

        public IActionResult Index(int? id)
        {
            return View();
        }

        public IActionResult Details(string? id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchPlants(string? name)
        {
            string?[] categories = plantService.GetCategories();
            string index = HttpContext.Request.Query["id"];
            if (string.IsNullOrEmpty(index)) index = "1";

            string? category = categories[Convert.ToInt32(index) - 1];
            Plant[] plants = await plantService.GetProductsByNameAsync(name, category);

            // route page by case
            if (!string.IsNullOrEmpty(name))
            {
                if (plants.Length == 0) return Redirect("/Plant/");
                else if (plants.Length == 1) 
                    return Redirect($"/Plant/Details/?id={plants[0].Id}");
                else
                    return Redirect("/Plant/");
            }
            else
                return Redirect("/Plant/");
        }

        public  async Task<IActionResult> Show(string id, bool? face)
        {
            Plant plant = await plantService.GetProductAsync(id);

            if (face != null)
            {
                if (face.Value)
                {
                    int index = plant.imageUrl.LastIndexOf(".");
                    byte[] data = System.IO.File.ReadAllBytes(plant.imageUrl);
                    return File(data, $"image/{plant.imageUrl.Substring(index + 1)}");
                }
                else
                {
                    int index = plant.description.posterImage.LastIndexOf(".");
                    byte[] data = System.IO.File.ReadAllBytes(plant.description.posterImage);
                    return File(data, $"image/{plant.description.posterImage.Substring(index + 1)}");
                }
            }
            else
                return NotFound();
        }
    }
}
