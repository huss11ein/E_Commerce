using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce.Application.DTOs;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Interfaces;

namespace E_Commerce.Application.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(ProductCreateDto productDto);
        Task<ProductDto> UpdateProductAsync(int id, ProductUpdateDto productDto);
        Task<bool> DeleteProductAsync(int id);
    }
    
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            
            return products.Select(MapToDto);
        }
        
        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            
            if (product == null)
                return null;
                
            return MapToDto(product);
        }
        
        public async Task<ProductDto> CreateProductAsync(ProductCreateDto productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException(nameof(productDto));
                
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CreatedAt = DateTime.Now
            };
            
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            
            return MapToDto(product);
        }
        
        public async Task<ProductDto> UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            if (productDto == null)
                throw new ArgumentNullException(nameof(productDto));
                
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            
            if (product == null)
                return null;
                
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Stock = productDto.Stock;
            product.UpdatedAt = DateTime.Now;
            
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            
            return MapToDto(product);
        }
        
        public async Task<bool> DeleteProductAsync(int id)
        {
            var exists = await _unitOfWork.Products.ExistsAsync(id);
            
            if (!exists)
                return false;
                
            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            
            return true;
        }
        
        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
} 