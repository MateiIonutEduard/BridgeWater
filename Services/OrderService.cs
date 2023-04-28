using BridgeWater.Data;
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

        public async Task<List<OrderViewModel>> GetProductOrdersAsync(int userId)
        {
            List<OrderViewModel> productOrders = (
                from p in await bridgeContext.Product.ToListAsync()
                join o in await bridgeContext.Order.ToListAsync()
                on p.Id equals o.ProductOrderId
                where o.IsCanceled == null || (o.IsCanceled.HasValue && !o.IsCanceled.Value)
                select new OrderViewModel
                {
                    Id = o.Id,
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Price = p.Price,
                    Stock = o.Stock
                }
            ).ToList();

            return productOrders;
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
