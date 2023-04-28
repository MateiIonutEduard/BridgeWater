using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IOrderService
    {
        Task<bool> ModifyOrderAsync(OrderModel orderModel);
        Task<ProductOrderViewList> GetProductOrdersAsync(OrderSearchFilter orderSearchFilter, int userId, int? page);
        Task<bool> CreateOrderAsync(OrderModel orderModel);
        Task<bool> CancelOrderAsync(int userId, int orderId);
    }
}
