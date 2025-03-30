using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce.Application.DTOs;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Interfaces;

namespace E_Commerce.Application.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto> GetCustomerByIdAsync(int id);
        Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto customerDto);
        Task<bool> UpdateCustomerAsync(int id, CustomerCreateDto customerDto);
        Task<bool> DeleteCustomerAsync(int id);
    }
    
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            
            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone
            });
        }
        
        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            
            if (customer == null)
                return null;
            
            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone
            };
        }
        
        public async Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto customerDto)
        {
            // Check if email is unique
            var isEmailUnique = await _unitOfWork.Customers.IsEmailUniqueAsync(customerDto.Email);
            if (!isEmailUnique)
                throw new Exception("Email is already in use");
                
            var customer = new Customer
            {
                Name = customerDto.Name,
                Email = customerDto.Email,
                Phone = customerDto.Phone
            };
            
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            
            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone
            };
        }
        
        public async Task<bool> UpdateCustomerAsync(int id, CustomerCreateDto customerDto)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
                return false;
                
            // Check if email is unique (excluding current customer)
            var isEmailUnique = await _unitOfWork.Customers.IsEmailUniqueAsync(customerDto.Email, id);
            if (!isEmailUnique)
                throw new Exception("Email is already in use by another customer");
                
            customer.Name = customerDto.Name;
            customer.Email = customerDto.Email;
            customer.Phone = customerDto.Phone;
            
            await _unitOfWork.Customers.UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var exists = await _unitOfWork.Customers.ExistsAsync(id);
            if (!exists)
                return false;
                
            await _unitOfWork.Customers.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }
    }
} 
