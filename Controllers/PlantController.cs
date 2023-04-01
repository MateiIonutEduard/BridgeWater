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
