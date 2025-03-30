using FluentValidation;
using E_Commerce.Application.DTOs;

namespace E_Commerce.Application.Validators
{
    public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("Valid customer is required");
                
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item")
                .Must(x => x.Count > 0).WithMessage("Order must contain at least one item");
                
            RuleForEach(x => x.Items).ChildRules(item => {
                item.RuleFor(x => x.ProductId)
                    .GreaterThan(0).WithMessage("Valid product is required");
                    
                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than zero");
            });
        }
    }
    
    public class OrderItemValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Valid product is required");
                
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero");
        }
    }
    
    public class OrderStatusUpdateValidator : AbstractValidator<OrderStatusUpdateDto>
    {
        public OrderStatusUpdateValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid order status");
        }
    }
} 
