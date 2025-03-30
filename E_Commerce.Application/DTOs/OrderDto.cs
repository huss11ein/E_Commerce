using System;
using System.Collections.Generic;
using E_Commerce.Domain.Common;

namespace E_Commerce.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public double TotalPrice { get; set; }
        public int ProductCount { get; set; }
        public List<OrderProductDto> Products { get; set; } = new List<OrderProductDto>();
        public DateTime? UpdatedAt { get; set; }
    }
    
    public class OrderProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Subtotal => Price * Quantity;
    }
    
    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
    
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public int ProductCount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderProductDto> Products { get; set; } = new List<OrderProductDto>();
        public double TotalPrice { get; set; }
    }
    
    public class OrderStatusUpdateDto
    {
        public OrderStatus Status { get; set; }
    }
} 
