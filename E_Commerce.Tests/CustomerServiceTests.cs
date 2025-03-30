using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce.Application.DTOs;
using E_Commerce.Application.Services;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Interfaces;
using Moq;
using Xunit;

namespace E_Commerce.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(uow => uow.Customers).Returns(_mockCustomerRepository.Object);
            _customerService = new CustomerService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllCustomersAsync_ReturnsAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", Phone = "123456789" },
                new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", Phone = "987654321" }
            };

            _mockCustomerRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            var customerDtos = result.ToList();
            Assert.Equal(2, customerDtos.Count);
            Assert.Equal(customers[0].Id, customerDtos[0].Id);
            Assert.Equal(customers[0].Name, customerDtos[0].Name);
            Assert.Equal(customers[1].Id, customerDtos[1].Id);
            Assert.Equal(customers[1].Name, customerDtos[1].Name);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_WithExistingId_ReturnsCustomer()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "John Doe", Email = "john@example.com", Phone = "123456789" };

            _mockCustomerRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customer.Id, result.Id);
            Assert.Equal(customer.Name, result.Name);
            Assert.Equal(customer.Email, result.Email);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var customerId = 99;
            
            _mockCustomerRepository.Setup(repo => repo.GetByIdAsync(customerId)).ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCustomerAsync_WithUniqueEmail_ReturnsCustomerDto()
        {
            // Arrange
            var customerDto = new CustomerCreateDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "123456789"
            };

            _mockCustomerRepository.Setup(repo => repo.IsEmailUniqueAsync(customerDto.Email, null)).ReturnsAsync(true);
            
            _mockCustomerRepository.Setup(repo => repo.AddAsync(It.IsAny<Customer>()))
                .ReturnsAsync((Customer customer) => customer);

            // Act
            var result = await _customerService.CreateCustomerAsync(customerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerDto.Name, result.Name);
            Assert.Equal(customerDto.Email, result.Email);
            Assert.Equal(customerDto.Phone, result.Phone);
            
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCustomerAsync_WithDuplicateEmail_ThrowsException()
        {
            // Arrange
            var customerDto = new CustomerCreateDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "123456789"
            };

            _mockCustomerRepository.Setup(repo => repo.IsEmailUniqueAsync(customerDto.Email, null)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _customerService.CreateCustomerAsync(customerDto));
            Assert.Contains("Email is already in use", exception.Message);
            
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }
    }
} 
