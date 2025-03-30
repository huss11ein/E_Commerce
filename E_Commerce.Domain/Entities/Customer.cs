using System.Collections.Generic;

namespace E_Commerce.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
        // Navigation property for orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
} 
