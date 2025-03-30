using System;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICustomerRepository Customers { get; }
        IOrderRepository Orders { get; }
        
        Task<int> SaveChangesAsync();
    }
} 
