using System;
using System.Threading.Tasks;
using E_Commerce.Domain.Interfaces;
using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.Repositories;

namespace E_Commerce.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _context;
        private IProductRepository _productRepository;
        private ICustomerRepository _customerRepository;
        private IOrderRepository _orderRepository;
        private bool _disposed = false;

        public UnitOfWork(ECommerceDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);

        public ICustomerRepository Customers => _customerRepository ??= new CustomerRepository(_context);

        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
} 
