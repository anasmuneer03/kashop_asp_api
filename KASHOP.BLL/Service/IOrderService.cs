using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetUserOrders(string userId);
        Task<OrderDetailsResponse?> GetOrderItems(string userId, int orderId);
        Task<bool> CancelOrder(string userId, int orderId);

        //for admin
        Task<List<OrderResponse>> GetAllOrders(OrderStatusEnum status);     
        Task<bool> ChangeOrderStatus(int orderId, ChangeOrderStatueRequest request);
    }
}
