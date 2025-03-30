using System;
using System.Collections.Generic;

namespace E_Commerce.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation property for orders
        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
} 
