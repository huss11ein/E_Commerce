# E-Commerce API

A RESTful API for managing products, customers, and orders for an e-commerce application built with ASP.NET Core, Entity Framework Core, and Fluent Validation.

## Main Libraries

This application uses the following main libraries:

- **ASP.NET Core 8.0**: Web framework for building the RESTful API endpoints
- **Entity Framework Core 8.0**: ORM for database access and management
- **FluentValidation**: Library for building strongly-typed validation rules
- **Moq**: Mocking framework used for unit testing
- **xUnit**: Testing framework for unit tests
- **Swashbuckle**: Swagger tools for API documentation
- **SQL Server**: Database provider for Entity Framework Core

## Architecture and Design Patterns

This application follows several architectural principles and design patterns:

- **Clean Architecture**: Separation of concerns with layered architecture (Domain, Application, Infrastructure, API)
- **Repository Pattern**: Abstracts data access logic and provides a clean API for working with domain entities
- **Unit of Work Pattern**: Manages database transactions and ensures consistency when multiple repositories are involved
- **Dependency Injection**: Used throughout the application for loose coupling and better testability
- **DTO Pattern**: Data Transfer Objects used to decouple API contracts from domain entities

## Project Structure

The project follows Clean Architecture principles and is organized as follows:

- **E_Commerce.Domain**: Contains entities, interfaces, and domain logic
- **E_Commerce.Application**: Contains business logic, DTOs, and validators
- **E_Commerce.Infrastructure**: Contains data access implementation, repositories, and database context
- **E_Commerce.API**: Contains controllers, middleware, and API configuration
- **E_Commerce.Tests**: Contains unit tests

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (or LocalDB)

### Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Update the connection string in `appsettings.json` if needed
4. Run the following commands:

```bash
dotnet build
dotnet ef database update --project E_Commerce.Infrastructure --startup-project E_Commerce.API
dotnet run --project E_Commerce.API/E_Commerce.API.csproj
```

5. Open your browser and navigate to `https://localhost:7000/swagger` to view the API documentation

### Running Tests

Run the following command to execute unit tests:

```bash
dotnet test
```

## Features

- RESTful API design
- Input validation using Fluent Validation
- Entity Framework Core for data access
- Clean Architecture
- Global exception handling
- Comprehensive logging
- API documentation with Swagger
- Database seeding for initial product data

## Database Structure

### Entity Relationship Diagram (ERD)

![image](https://github.com/user-attachments/assets/d3d42b2f-3439-4cba-bb89-4d4319abf004)


### Database Entities

#### Customer Entity
- **Id**: Primary key, auto-incremented integer
- **Name**: Required, max length 100 characters
- **Email**: Required, unique, max length 100 characters
- **Phone**: Optional, max length 20 characters
- Has a one-to-many relationship with Orders

#### Product Entity
- **Id**: Primary key, auto-incremented integer
- **Name**: Required, max length 100 characters
- **Description**: Optional, max length 500 characters
- **Price**: Required double value
- **Stock**: Required integer value
- **CreatedAt**: Required DateTime, defaults to current time
- **UpdatedAt**: Optional DateTime, updated when product is modified
- Has a many-to-many relationship with Orders

#### Order Entity
- **Id**: Primary key, auto-incremented integer
- **CustomerId**: Foreign key to Customer
- **OrderDate**: Required DateTime, defaults to current time
- **Status**: Enum value (Pending, Delivered)
- **TotalPrice**: Required double value
- **UpdatedAt**: Optional DateTime, updated when order status changes
- Has a one-to-many relationship with OrderProducts
- Has a many-to-one relationship with Customer

#### OrderProduct Entity
- **OrderId**: Part of composite primary key, foreign key to Order
- **ProductId**: Part of composite primary key, foreign key to Product
- **Quantity**: Required integer, number of items ordered
- Implements the many-to-many relationship between Orders and Products with quantity
- Has a many-to-one relationship with both Order and Product

## API Documentation

### Base URL
```
https://localhost:7000
```

### Customers

#### Get All Customers
- **URL**: `/api/customers`
- **Method**: `GET`
- **Description**: Retrieves a list of all customers

#### Get Customer By ID
- **URL**: `/api/customers/{id}`
- **Method**: `GET`
- **Description**: Retrieves details of a specific customer by ID

#### Create Customer
- **URL**: `/api/customers`
- **Method**: `POST`
- **Description**: Creates a new customer
- **Request Body**:
  ```json
  {
    "name": "John Doe",
    "email": "john.doe@example.com",
    "phone": "123-456-7890"
  }
  ```

### Products

#### Get All Products
- **URL**: `/api/products`
- **Method**: `GET`
- **Description**: Retrieves a list of all products

#### Get Product By ID
- **URL**: `/api/products/{id}`
- **Method**: `GET`
- **Description**: Retrieves details of a specific product by ID

#### Create Product
- **URL**: `/api/products`
- **Method**: `POST`
- **Description**: Creates a new product
- **Request Body**:
  ```json
  {
    "name": "Bluetooth Speaker",
    "description": "Portable waterproof speaker",
    "price": 79.99,
    "stock": 200
  }
  ```

#### Update Product
- **URL**: `/api/products/{id}`
- **Method**: `PUT`
- **Description**: Updates an existing product
- **Request Body**:
  ```json
  {
    "name": "Updated Laptop",
    "description": "Updated high performance laptop with extra features",
    "price": 1299.99,
    "stock": 45
  }
  ```

#### Delete Product
- **URL**: `/api/products/{id}`
- **Method**: `DELETE`
- **Description**: Deletes a product by ID

### Orders

#### Get Order By ID
- **URL**: `/api/orders/{id}`
- **Method**: `GET`
- **Description**: Retrieves details of a specific order by ID

#### Create Order
- **URL**: `/api/orders`
- **Method**: `POST`
- **Description**: Creates a new order with product quantities
- **Request Body**:
  ```json
  {
    "customerId": 1,
    "items": [
      {
        "productId": 1,
        "quantity": 2
      },
      {
        "productId": 2,
        "quantity": 1
      }
    ]
  }
  ```
- **Validation**:
  - At least one item must be included
  - All specified products must exist
  - Quantity must be greater than zero for each product
  - Product must have sufficient stock
  - Products with zero stock cannot be ordered

#### Update Order Status
- **URL**: `/api/orders/UpdateOrderStatus/{id}`
- **Method**: `POST`
- **Description**: Updates the status of an order
- **Request Body**:
  ```json
  {
    "status": 1
  }
  ```
- **Status Values**:
  - `0`: Pending
  - `1`: Delivered