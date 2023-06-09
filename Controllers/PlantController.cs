﻿using BridgeWater.Data;
using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost, Authorize]
        public async Task<IActionResult> AddComment(CommentRatingModel commentRatingModel)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if (userId != null)
            {
                // Creates new post with rating
                int AccountId = Convert.ToInt32(userId);
                commentRatingModel.accountId = AccountId;

                await plantService.CreatePostCommentAsync(commentRatingModel);
                return Redirect($"/Plant/Details/?id={commentRatingModel.plantId}");
            }
            else
                return Redirect("/Account/");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> SendPostReply(CommentRatingModel commentRatingModel)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if (userId != null)
            {
                // Creates new post with rating
                int AccountId = Convert.ToInt32(userId);
                commentRatingModel.accountId = AccountId;

                if (string.IsNullOrEmpty(commentRatingModel.message))
                {
                    string? key = HttpContext.Request.Form.Keys
                        .FirstOrDefault(key => key.StartsWith("body"));

                    if (!string.IsNullOrEmpty(key))
                        commentRatingModel.message = HttpContext.Request.Form[key];
                }

                await plantService.CreateReplyPostAsync(commentRatingModel);
                return Redirect($"/Plant/Details/?id={commentRatingModel.plantId}");
            }
            else
                return Redirect("/Account/");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> RemoveComment(string plantId, string commentId)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                int AccountId = Convert.ToInt32(userId);
                Comment? comment = await plantService.GetCommentAsync(plantId, commentId);

                /* comment exists, so remove it */
                if(comment != null) 
                    await plantService.RemoveCommentAsync(AccountId, plantId, commentId);

                return Redirect($"/Plant/Details/?id={plantId}");
            }
            else
                return Redirect("/Account/");
        }

        [HttpPost]
        public async Task<IActionResult> SearchPlants(string? name)
        {
            // filter store products only by name, not by using category
            Plant[] plants = await plantService.GetProductsByNameAsync(name, null);

            // route page by case
            if (!string.IsNullOrEmpty(name))
            {
                if (plants.Length == 0) return Redirect("/Plant/");
                else if (plants.Length == 1) 
                    return Redirect($"/Plant/Details/?id={plants[0].Id}");
                else
                    return Redirect($"/Plant/?name={name}");
            }
            else
                return Redirect("/Plant/");
        }

        public  async Task<IActionResult> Show(string id, bool? face)
        {
            PlantViewModel plant = await plantService.GetProductAsync(id);

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
