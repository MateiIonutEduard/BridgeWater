﻿using BridgeWater.Data;
using BridgeWater.Models;
using Microsoft.EntityFrameworkCore;
#pragma warning disable

namespace BridgeWater.Services
{
    public class OrderService : IOrderService
    {
        readonly BridgeContext bridgeContext;

        public OrderService(BridgeContext bridgeContext)
        { this.bridgeContext = bridgeContext; }

        public async Task<OrderViewModel?> GetOrderAsync(int orderId)
        {
            Order? order = await bridgeContext.Order.Where(o => o.Id == orderId)
                .FirstOrDefaultAsync();

            /* If product order does not exists. */
            if (order == null) return null;
            var product = await bridgeContext.Product.Where(p => p.Id == order.ProductOrderId)
                .FirstOrDefaultAsync();

            /* found it! */
            var obj = new OrderViewModel
            {
                Id = order.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                IsCanceled = (order.IsCanceled != null ? order.IsCanceled.Value : false),
                IsPayed = (order.IsPayed != null ? order.IsPayed.Value : false),
                Price = product.Price,
                Stock = order.Stock
            };

            return obj;
        }

        public async Task<bool> ModifyOrderAsync(int orderId, bool IsPayed)
        {
            Order? order = await bridgeContext.Order
                .FirstOrDefaultAsync(e => e.Id == orderId && (e.IsCanceled != null ? !e.IsCanceled.Value : false));

            // check if order exists already, thus is updated
            if (order != null)
            {
                order.IsPayed = IsPayed;
                await bridgeContext.SaveChangesAsync();
                return true;
            }

            // cannot update
            return false;
        }

        public async Task<bool> ModifyOrderAsync(OrderModel orderModel)
        {
            Order? order = await bridgeContext.Order
                .FirstOrDefaultAsync(e => e.AccountId == orderModel.AccountId && e.ProductOrderId == orderModel.ProductOrderId && (e.IsCanceled != null ? !e.IsCanceled.Value : false));

            // check if order exists already, thus is updated
            if(order != null)
            {
                order.Stock = orderModel.Stock;
                await bridgeContext.SaveChangesAsync();
                return true;
            }

            // cannot update
            return false;
        }

        public async Task<ProductOrderViewList> GetProductOrdersAsync(OrderSearchFilter? orderSearchFilter, int userId, int? page)
        {
            List<OrderViewModel> productOrders = new List<OrderViewModel>();

            if (orderSearchFilter != null && orderSearchFilter.IsCanceled != null)
            {
                productOrders = (
                    from p in await bridgeContext.Product.ToListAsync()
                    join o in await bridgeContext.Order.ToListAsync()
                    on p.Id equals o.ProductOrderId
                    where o.AccountId == userId && orderSearchFilter.IsCanceled.Value == (o.IsCanceled != null ? o.IsCanceled.Value : false)
                    select new OrderViewModel
                    {
                        Id = o.Id,
                        ProductId = p.Id,
                        ProductName = p.Name,
                        IsCanceled = (o.IsCanceled != null ? o.IsCanceled.Value : false),
                        IsPayed = (o.IsPayed != null ? o.IsPayed.Value : false),
                        Price = p.Price,
                        Stock = o.Stock
                    }
                ).ToList();
            }
            else
            {
                productOrders = (
                    from p in await bridgeContext.Product.ToListAsync()
                    join o in await bridgeContext.Order.ToListAsync()
                    on p.Id equals o.ProductOrderId
                    select new OrderViewModel
                    {
                        Id = o.Id,
                        ProductId = p.Id,
                        ProductName = p.Name,
                        IsCanceled = (o.IsCanceled != null ? o.IsCanceled.Value : false),
                        Price = p.Price,
                        Stock = o.Stock
                    }
                ).ToList();
            }

            if (orderSearchFilter != null && !string.IsNullOrEmpty(orderSearchFilter.ProductName))
            {
                productOrders = productOrders.Where(p => p.ProductName.Contains(orderSearchFilter.ProductName))
                    .ToList();
            }

            if(orderSearchFilter != null && orderSearchFilter.CategoryId != null && orderSearchFilter.CategoryId.Value > 0)
            {
                for(int k = 0; k < productOrders.Count; k++)
                {
                    Product? product = await bridgeContext.Product
                        .FirstOrDefaultAsync(p => p.Id == productOrders[k].ProductId);

                    if (product != null && product.CategoryId != orderSearchFilter.CategoryId.Value)
                        productOrders.RemoveAt(k);
                }
            }

            int totalPages = productOrders.Count >> 3;
            if ((productOrders.Count & 7) != 0) totalPages++;
            int index = page != null ? page.Value - 1 : 0;

            ProductOrderViewList productOrderViewList = new ProductOrderViewList
            {
                pages = totalPages,
                results = productOrders.Count,
                orderViewModels = productOrders.Skip(8 * index).Take(8).ToArray()
            };

            return productOrderViewList;
        }

        public async Task<bool> CreateOrderAsync(OrderModel orderModel)
        {
            Order? order = await bridgeContext.Order
                .FirstOrDefaultAsync(e => e.AccountId == orderModel.AccountId && e.ProductOrderId == orderModel.ProductOrderId && (e.IsCanceled != null ? !e.IsCanceled.Value : false));

            Product? product = await bridgeContext.Product
                .FirstOrDefaultAsync(e => e.Id == orderModel.ProductOrderId);

            // new order
            if (order == null)
            {
                order = new Order
                {
                    AccountId = orderModel.AccountId,
                    ProductOrderId = orderModel.ProductOrderId,
                    Stock = orderModel.Stock,
                    IsCanceled = false
                };

                bridgeContext.Order.Add(order);
                product.Stock -= orderModel.Stock;

                await bridgeContext.SaveChangesAsync();
                return true;
            }

            // update order
            order.Stock += orderModel.Stock;
            product.Stock -= orderModel.Stock;

            await bridgeContext.SaveChangesAsync();
            return false;
        }

        public async Task<bool> CancelOrderAsync(int userId, int orderId)
        {
            Order? order = await bridgeContext.Order
                .FirstOrDefaultAsync(e => e.AccountId == userId && e.Id == orderId && (e.IsCanceled != null ? !e.IsCanceled.Value : false));

            if(order != null)
            {
                order.IsCanceled = true;
                Product? product = await bridgeContext.Product.FirstOrDefaultAsync(p => p.Id == order.ProductOrderId);

                product.Stock += order.Stock;
                await bridgeContext.SaveChangesAsync();
            }

            return false;
        }
    }
}
