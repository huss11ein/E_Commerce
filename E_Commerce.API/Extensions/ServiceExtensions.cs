using E_Commerce.Application.Services;
using E_Commerce.Application.Validators;
using E_Commerce.Domain.Interfaces;
using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.Repositories;
using E_Commerce.Infrastructure.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ECommerceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
        }

        public static void ConfigureValidators(this IServiceCollection services)
        {
            services.AddScoped<CustomerCreateValidator>();
            services.AddScoped<OrderCreateValidator>();
            services.AddScoped<OrderStatusUpdateValidator>();
            services.AddScoped<ProductCreateValidator>();
            services.AddScoped<ProductUpdateValidator>();
        }
    }
} 