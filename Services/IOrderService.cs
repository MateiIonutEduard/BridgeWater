using BridgeWater.Models;

namespace BridgeWater.Services
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(OrderModel orderModel);
        Task<bool> CancelOrderAsync(int userId, int orderId);
    }
}
