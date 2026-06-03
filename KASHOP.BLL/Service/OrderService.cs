using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository; 
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<List<OrderResponse>> GetUserOrders(string userId)
        {
            var orders = await _orderRepository.GetAllAsync(
                filter: o => o.UserId == userId
                );
            return orders.Adapt<List<OrderResponse>>();
        }

        public async Task<OrderDetailsResponse> GetOrderItems(string userId, int orderId)
        {
            var items = await _orderRepository.GetOne(
                filter: o => o.Id == orderId && o.UserId == userId,
                includes: new[]
                {
                   nameof(Order.orderItems),
                   $"{nameof(Order.orderItems)}.{nameof(OrderItem.Product)}",
                   $"{nameof(Order.orderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}"
                } 
                );

            if(items == null) return null;

            return items.Adapt<OrderDetailsResponse>();
        }

        public async Task<bool> CancelOrder(string userId, int orderId)
        {
            var order = await _orderRepository.GetOne(
                filter: o => o.UserId == userId && o.Id == orderId
                );

            if(order is null) return false;

            if(order.orderStatus != OrderStatusEnum.Pending)
                return false;

            order.orderStatus = OrderStatusEnum.Cancelled;
            return await _orderRepository.UpdateAsync( order );

        }

        public async Task<List<OrderResponse>> GetAllOrders(OrderStatusEnum status)
        {
            var order = await _orderRepository.GetAllAsync(
                filter : o=> o.orderStatus == status
                );

            return order.Adapt<List<OrderResponse>>();
        }

        public async Task<bool> ChangeOrderStatus(int orderId, ChangeOrderStatueRequest request)
        {
            var order = await _orderRepository.GetOne(
                filter: o => o.Id == orderId
                );

            if(order is null) return false;
            if (order.orderStatus == OrderStatusEnum.Cancelled ||
                order.orderStatus == OrderStatusEnum.Delivered)
                return false;

            if ((int)request.status != (int)order.orderStatus + 1)
                return false;
            
            order.orderStatus = request.status;
            return await _orderRepository.UpdateAsync(order);
        }
    }
}
