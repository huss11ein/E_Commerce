using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce.Application.DTOs;
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Interfaces;

namespace E_Commerce.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDetailsDto> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(OrderCreateDto orderDto);
        Task<bool> UpdateOrderStatusAsync(int id, OrderStatusUpdateDto statusDto);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId);
        Task<Customer> GetCustomerByIdAsync(int customerId);
    }
    
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            return orders.Select(MapOrderToDto);
        }
        
        public async Task<OrderDetailsDto> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            
            if (order == null)
                return null;
            
            return new OrderDetailsDto
            {
                Id = order.Id,
                CustomerName = order.Customer.Name,
                Status = order.Status,
                ProductCount = order.OrderProducts.Count,
                OrderDate = order.OrderDate,
                UpdatedAt = order.UpdatedAt,
                Products = order.OrderProducts.Select(op => new OrderProductDto
                {
                    ProductId = op.ProductId,
                    Name = op.Product.Name,
                    Description = op.Product.Description,
                    Price = op.Product.Price,
                    Quantity = op.Quantity
                }).ToList(),
                TotalPrice = order.TotalPrice
            };
        }
        
        public async Task<OrderDto> CreateOrderAsync(OrderCreateDto orderDto)
        {
            // Check if customer exists
            var customer = await _unitOfWork.Customers.GetByIdAsync(orderDto.CustomerId);
            if (customer == null)
                throw new Exception("Customer not found");
                
            // Validate items
            if (orderDto.Items.Count == 0)
                throw new Exception("Order must contain at least one item");
                
            // Calculate total price and validate products
            double totalPrice = 0;
            var orderProducts = new List<OrderProduct>();
            
            foreach (var item in orderDto.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Quantity must be greater than zero");
                    
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found");
                    
                if (product.Stock < item.Quantity)
                    throw new Exception($"Insufficient stock for product '{product.Name}'. Available: {product.Stock}, Requested: {item.Quantity}");
                    
                if (product.Stock == 0)
                    throw new Exception($"Product '{product.Name}' is out of stock");
                    
                // Reduce stock
                product.Stock -= item.Quantity;
                product.UpdatedAt = DateTime.Now;
                await _unitOfWork.Products.UpdateAsync(product);
                
                // Add to order products
                orderProducts.Add(new OrderProduct 
                { 
                    ProductId = product.Id,
                    Product = product,
                    Quantity = item.Quantity
                });
                
                totalPrice += product.Price * item.Quantity;
            }
            
            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                TotalPrice = totalPrice,
                OrderProducts = orderProducts
            };
            
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            
            return MapOrderToDto(order);
        }
        
        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatusUpdateDto statusDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
                return false;
            
            order.Status = statusDto.Status;
            order.UpdatedAt = DateTime.Now;
            
            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByCustomerIdAsync(customerId);
            return orders.Select(MapOrderToDto);
        }
        
        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await _unitOfWork.Customers.GetByIdAsync(customerId);
        }
        
        private OrderDto MapOrderToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer.Name,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                ProductCount = order.OrderProducts.Count,
                UpdatedAt = order.UpdatedAt,
                Products = order.OrderProducts.Select(op => new OrderProductDto
                {
                    ProductId = op.Product.Id,
                    Name = op.Product.Name,
                    Description = op.Product.Description,
                    Price = op.Product.Price,
                    Quantity = op.Quantity
                }).ToList()
            };
        }
    }
} 
