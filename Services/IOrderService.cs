using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IOrderService
    {
        Task<List<OrderViewModel>> GetProductOrdersAsync(int userId);
        Task<bool> CreateOrderAsync(OrderModel orderModel);
        Task<bool> CancelOrderAsync(int userId, int orderId);
    }
}
