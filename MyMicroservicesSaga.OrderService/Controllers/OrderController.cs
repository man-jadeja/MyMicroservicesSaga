using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMicroservicesSaga.OrderService.Data;
using MyMicroservicesSaga.OrderService.Dto;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderController> _logger;

        public OrderController(OrderDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<OrderController> logger)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order and publishes an OrderCreated event.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductName = request.ProductName,
                Quantity = request.Quantity,
                Amount = request.Amount,
                Status = "Created",
                CreatedBy = Guid.NewGuid()
            };

            try
            {
                // Save to database
                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();

                // Publish event to RabbitMQ
                var orderCreatedEvent = new OrderCreated(
                    order.Id,
                    order.ProductName,
                    order.Quantity,
                    order.Amount
                );

                await _publishEndpoint.Publish(orderCreatedEvent);

                _logger.LogWarning("Order {OrderId} created and event published.", order.Id);

                return Ok(new
                {
                    order.Id,
                    Message = "Order created successfully. Saga started."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "Error creating order");
            }
        }
    }
}
