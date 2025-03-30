using System;
using System.Collections.Generic;
using E_Commerce.Domain.Common;

namespace E_Commerce.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public double TotalPrice { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
} 
