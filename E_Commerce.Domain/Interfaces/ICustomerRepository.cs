using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? id = null);
    }
} 
