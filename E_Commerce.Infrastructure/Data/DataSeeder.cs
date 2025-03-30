using System;
using System.Collections.Generic;
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            SeedProducts(modelBuilder);
            SeedCustomers(modelBuilder);
            SeedOrders(modelBuilder);
            SeedOrderProducts(modelBuilder);
        }
        
        private static void SeedProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop",
                    Description = "High performance laptop",
                    Price = 1200.00,
                    Stock = 50,
                    CreatedAt = new DateTime(2023, 1, 1)
                },
                new Product
                {
                    Id = 2,
                    Name = "Smartphone",
                    Description = "Latest model smartphone",
                    Price = 800.00,
                    Stock = 100,
                    CreatedAt = new DateTime(2023, 1, 1)
                },
                new Product
                {
                    Id = 3,
                    Name = "Headphones",
                    Description = "Wireless noise-cancelling headphones",
                    Price = 200.00,
                    Stock = 150,
                    CreatedAt = new DateTime(2023, 1, 1)
                },
                new Product
                {
                    Id = 4,
                    Name = "Tablet",
                    Description = "10-inch tablet with retina display",
                    Price = 500.00,
                    Stock = 75,
                    CreatedAt = new DateTime(2023, 1, 1)
                },
                new Product
                {
                    Id = 5,
                    Name = "Smartwatch",
                    Description = "Fitness tracking smartwatch",
                    Price = 250.00,
                    Stock = 80,
                    CreatedAt = new DateTime(2023, 1, 1)
                }
            );
        }
        
        private static void SeedCustomers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    Phone = "123-456-7890"
                },
                new Customer
                {
                    Id = 2,
                    Name = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Phone = "987-654-3210"
                }
            );
        }

        private static void SeedOrders(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    CustomerId = 1,
                    OrderDate = new DateTime(2023, 1, 15),
                    Status = OrderStatus.Delivered,
                    TotalPrice = 1200.00
                },
                new Order
                {
                    Id = 2,
                    CustomerId = 2,
                    OrderDate = new DateTime(2023, 2, 5),
                    Status = OrderStatus.Delivered,
                    TotalPrice = 1050.00
                },
                new Order
                {
                    Id = 3,
                    CustomerId = 1,
                    OrderDate = new DateTime(2023, 3, 10),
                    Status = OrderStatus.Pending,
                    TotalPrice = 700.00
                }
            );
        }

        private static void SeedOrderProducts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderProduct>().HasData(
                new OrderProduct
                {
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1
                },
                
                new OrderProduct
                {
                    OrderId = 2,
                    ProductId = 2,
                    Quantity = 1
                },
                new OrderProduct
                {
                    OrderId = 2,
                    ProductId = 5,
                    Quantity = 1
                },
                
                new OrderProduct
                {
                    OrderId = 3,
                    ProductId = 3,
                    Quantity = 1
                },
                new OrderProduct
                {
                    OrderId = 3,
                    ProductId = 4,
                    Quantity = 1
                }
            );
        }
    }
} 