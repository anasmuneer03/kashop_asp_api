using Microsoft.EntityFrameworkCore;
namespace KASHOP.DAL.Models
{
    [PrimaryKey(nameof(ProductId), nameof(OrderId))]
    public class OrderItem
    {
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
