using System;
using System.Threading.Tasks;
using E_Commerce.Application.DTOs;
using E_Commerce.Application.Services;
using E_Commerce.Application.Validators;
using E_Commerce.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IValidator<OrderCreateDto> _createValidator;
        private readonly IValidator<OrderStatusUpdateDto> _statusValidator;

        public OrdersController(
            IOrderService orderService,
            IValidator<OrderCreateDto> createValidator,
            IValidator<OrderStatusUpdateDto> statusValidator)
        {
            _orderService = orderService;
            _createValidator = createValidator;
            _statusValidator = statusValidator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            var validationResult = await _createValidator.ValidateAsync(orderDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
                
            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            
            if (order == null)
                return NotFound(new { message = "Order not found" });
                
            return Ok(order);
        }

        [HttpPost("UpdateOrderStatus/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdateDto statusDto)
        {
            var validationResult = await _statusValidator.ValidateAsync(statusDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            try
            {
                var success = await _orderService.UpdateOrderStatusAsync(id, statusDto);
                
                if (!success)
                {
                    return NotFound(new { message = "Order not found" });
                }
                
                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrdersByCustomerId(int customerId)
        {
            var customer = await _orderService.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }
            
            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }
    }
} 