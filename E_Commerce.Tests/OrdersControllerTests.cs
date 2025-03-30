using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce.API.Controllers;
using E_Commerce.Application.DTOs;
using E_Commerce.Application.Services;
using E_Commerce.Application.Validators;
using E_Commerce.Domain.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace E_Commerce.Tests
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IValidator<OrderCreateDto>> _mockCreateValidator;
        private readonly Mock<IValidator<OrderStatusUpdateDto>> _mockStatusValidator;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockCreateValidator = new Mock<IValidator<OrderCreateDto>>();
            _mockStatusValidator = new Mock<IValidator<OrderStatusUpdateDto>>();
            _controller = new OrdersController(
                _mockOrderService.Object,
                _mockCreateValidator.Object,
                _mockStatusValidator.Object);
        }

        [Fact]
        public async Task CreateOrder_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var orderDto = new OrderCreateDto
            {
                CustomerId = 1,
                Items = new List<OrderItemDto> 
                { 
                    new OrderItemDto { ProductId = 1, Quantity = 2 },
                    new OrderItemDto { ProductId = 2, Quantity = 1 },
                    new OrderItemDto { ProductId = 3, Quantity = 3 }
                }
            };

            var createdOrder = new OrderDto
            {
                Id = 1,
                CustomerId = orderDto.CustomerId,
                CustomerName = "John Doe",
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                TotalPrice = 190.00,
                ProductCount = orderDto.Items.Count,
                Products = new List<OrderProductDto>
                {
                    new OrderProductDto { ProductId = 1, Name = "Product 1", Price = 30.0, Quantity = 2 },
                    new OrderProductDto { ProductId = 2, Name = "Product 2", Price = 30.0, Quantity = 1 },
                    new OrderProductDto { ProductId = 3, Name = "Product 3", Price = 40.0, Quantity = 3 }
                }
            };

            _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<OrderCreateDto>(), default))
                .ReturnsAsync(new ValidationResult());

            _mockOrderService.Setup(service => service.CreateOrderAsync(orderDto))
                .ReturnsAsync(createdOrder);

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetOrderById", createdAtActionResult.ActionName);
            var returnedOrder = Assert.IsType<OrderDto>(createdAtActionResult.Value);
            Assert.Equal(1, returnedOrder.Id);
            Assert.Equal(orderDto.CustomerId, returnedOrder.CustomerId);
            Assert.Equal(orderDto.Items.Count, returnedOrder.ProductCount);
        }

        [Fact]
        public async Task CreateOrder_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var orderDto = new OrderCreateDto
            {
                CustomerId = 1,
                // Missing items (empty list)
                Items = new List<OrderItemDto>()
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Items", "Order must contain at least one item")
            };

            var validationResult = new ValidationResult(validationFailures);

            _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<OrderCreateDto>(), default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetOrderById_WithExistingId_ReturnsOkResult()
        {
            // Arrange
            var orderId = 1;
            var orderDetails = new OrderDetailsDto 
            { 
                Id = orderId, 
                CustomerName = "John Doe", 
                Status = OrderStatus.Pending, 
                ProductCount = 3,
                OrderDate = DateTime.Now,
                TotalPrice = 190.00,
                Products = new List<OrderProductDto>
                {
                    new OrderProductDto { ProductId = 1, Name = "Product 1", Price = 30.0, Quantity = 2 },
                    new OrderProductDto { ProductId = 2, Name = "Product 2", Price = 30.0, Quantity = 1 }
                }
            };

            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId))
                .ReturnsAsync(orderDetails);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<OrderDetailsDto>(okResult.Value);
            Assert.Equal(orderId, returnedOrder.Id);
            Assert.Equal("John Doe", returnedOrder.CustomerName);
            Assert.Equal(3, returnedOrder.ProductCount);
        }

        [Fact]
        public async Task GetOrderById_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            var orderId = 99;
            
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId))
                .ReturnsAsync((OrderDetailsDto)null);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateOrderStatus_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var orderId = 1;
            var statusDto = new OrderStatusUpdateDto
            {
                Status = OrderStatus.Delivered
            };

            _mockStatusValidator.Setup(v => v.ValidateAsync(It.IsAny<OrderStatusUpdateDto>(), default))
                .ReturnsAsync(new ValidationResult());

            _mockOrderService.Setup(service => service.UpdateOrderStatusAsync(orderId, statusDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, statusDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task UpdateOrderStatus_WithNonExistingOrder_ReturnsNotFound()
        {
            // Arrange
            var orderId = 99;
            var statusDto = new OrderStatusUpdateDto
            {
                Status = OrderStatus.Delivered
            };

            _mockStatusValidator.Setup(v => v.ValidateAsync(It.IsAny<OrderStatusUpdateDto>(), default))
                .ReturnsAsync(new ValidationResult());

            _mockOrderService.Setup(service => service.UpdateOrderStatusAsync(orderId, statusDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, statusDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateOrderStatus_WithInvalidStatus_ReturnsBadRequest()
        {
            // Arrange
            var orderId = 1;
            var statusDto = new OrderStatusUpdateDto
            {
                // Invalid status - this is simulated by validation failure
                Status = OrderStatus.Pending
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Status", "Invalid order status")
            };

            var validationResult = new ValidationResult(validationFailures);

            _mockStatusValidator.Setup(v => v.ValidateAsync(It.IsAny<OrderStatusUpdateDto>(), default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, statusDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
} 
