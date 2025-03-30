using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce.API.Controllers;
using E_Commerce.Application.DTOs;
using E_Commerce.Application.Services;
using E_Commerce.Application.Validators;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace E_Commerce.Tests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<IValidator<CustomerCreateDto>> _mockValidator;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockValidator = new Mock<IValidator<CustomerCreateDto>>();
            _controller = new CustomersController(_mockCustomerService.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task GetAllCustomers_ReturnsOkResult_WithListOfCustomers()
        {
            // Arrange
            var customers = new List<CustomerDto>
            {
                new CustomerDto { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "123456789" },
                new CustomerDto { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "987654321" }
            };

            _mockCustomerService.Setup(service => service.GetAllCustomersAsync())
                .ReturnsAsync(customers);

            // Act
            var result = await _controller.GetAllCustomers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomers = Assert.IsType<List<CustomerDto>>(okResult.Value);
            Assert.Equal(2, returnedCustomers.Count);
        }

        [Fact]
        public async Task GetCustomerById_WithExistingId_ReturnsOkResult()
        {
            // Arrange
            var customerId = 1;
            var customer = new CustomerDto { Id = customerId, Name = "John Doe", Email = "john@example.com", Phone = "123456789" };

            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            // Act
            var result = await _controller.GetCustomerById(customerId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomer = Assert.IsType<CustomerDto>(okResult.Value);
            Assert.Equal(customerId, returnedCustomer.Id);
            Assert.Equal("John Doe", returnedCustomer.Name);
        }

        [Fact]
        public async Task GetCustomerById_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            var customerId = 99;
            
            _mockCustomerService.Setup(service => service.GetCustomerByIdAsync(customerId))
                .ReturnsAsync((CustomerDto)null);

            // Act
            var result = await _controller.GetCustomerById(customerId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateCustomer_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var customerDto = new CustomerCreateDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "123456789"
            };

            var createdCustomer = new CustomerDto
            {
                Id = 1,
                Name = customerDto.Name,
                Email = customerDto.Email,
                Phone = customerDto.Phone
            };

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CustomerCreateDto>(), default))
                .ReturnsAsync(new ValidationResult());

            _mockCustomerService.Setup(service => service.CreateCustomerAsync(customerDto))
                .ReturnsAsync(createdCustomer);

            // Act
            var result = await _controller.CreateCustomer(customerDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetCustomerById", createdAtActionResult.ActionName);
            var returnedCustomer = Assert.IsType<CustomerDto>(createdAtActionResult.Value);
            Assert.Equal(1, returnedCustomer.Id);
            Assert.Equal(customerDto.Name, returnedCustomer.Name);
        }

        [Fact]
        public async Task CreateCustomer_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var customerDto = new CustomerCreateDto
            {
                // Missing required Name
                Email = "john@example.com",
                Phone = "123456789"
            };

            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };

            var validationResult = new ValidationResult(validationFailures);

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CustomerCreateDto>(), default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.CreateCustomer(customerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
} 
