﻿using Braintree;
using BridgeWater.Models;
using BridgeWater.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BridgeWater.Controllers
{
    public class OrderController : Controller
    {
        readonly IOrderService orderService;
        readonly IBraintreeService braintreeService;

        public OrderController(IOrderService orderService, IBraintreeService braintreeService)
        { 
            this.orderService = orderService;
            this.braintreeService = braintreeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, Authorize]
        public IActionResult Search(OrderSearchFilter orderSearchFilter)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                int UserId = Convert.ToInt32(userId);
                ViewData["filter"] = orderSearchFilter;
                return View("Views/Order/Index.cshtml", ViewData["filter"]);
            }
            else
                return Redirect("/Account/");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> UpdateOrder(OrderModel orderModel)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if(!string.IsNullOrEmpty(userId))
            {
                orderModel.AccountId = Convert.ToInt32(userId);
                await orderService.ModifyOrderAsync(orderModel);
                return Redirect("/Order/");
            }
            else
                return Redirect("/Account/");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Create(OrderModel orderModel)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                orderModel.AccountId = Convert.ToInt32(userId);
                await orderService.CreateOrderAsync(orderModel);
                return Redirect("/Order/");
            }
            else
                return Redirect("/Account/");
        }

        public IActionResult Payment()
        {
            var gateway = braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Pay(int orderId)
        {
            var gateway = braintreeService.GetGateway();
            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal("250"),
                PaymentMethodNonce = HttpContext.Request.Form["nonce"],
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            bool isPayed = result.IsSuccess();

            await orderService.ModifyOrderAsync(orderId, isPayed);
            return Redirect($"/Order/Payment/?orderId={orderId}&isPayed={isPayed}");
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Cancel([FromQuery]int orderId)
        {
            string? userId = HttpContext.User?.Claims?
                .FirstOrDefault(u => u.Type == "id")?.Value;
            
            if(!string.IsNullOrEmpty(userId))
            {
                int uid = Convert.ToInt32(userId);
                await orderService.CancelOrderAsync(uid, orderId);
                return Redirect("/Order/");
            }
            else
                return Redirect("/Account/");
        }
    }
}
