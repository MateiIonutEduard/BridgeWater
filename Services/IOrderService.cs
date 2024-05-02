using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IOrderService
    {
        Task<OrderViewModel?> GetOrderAsync(int orderId);
        Task<bool> ModifyOrderAsync(int orderId, bool IsPayed);
        Task<bool> ModifyOrderAsync(OrderModel orderModel);
        Task<ProductOrderViewList> GetProductOrdersAsync(OrderSearchFilter orderSearchFilter, int userId, int? page);
        Task<bool> CreateOrderAsync(OrderModel orderModel);
        Task<bool> CancelOrderAsync(int userId, int orderId);
    }
}
