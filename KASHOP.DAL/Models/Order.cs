using KASHOP.DAL.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public enum OrderStatusEnum
    {
        Pending = 1,
        Approved = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Paid = 6
    }
    public class Order
    {
        public int Id { get; set; } 
        public PaymentMethodEnum paymentMethod { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime ShippedDate {  get; set; }
        public OrderStatusEnum orderStatus { get; set; } = OrderStatusEnum.Pending;
        public string? StripeSessionId { get; set; }
        public decimal AmountPaid { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<OrderItem> orderItems { get; set; } = new List<OrderItem>();
    }
}
