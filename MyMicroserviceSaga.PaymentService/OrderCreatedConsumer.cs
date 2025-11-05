using MassTransit;
using MyMicroservicesSaga.PaymentService.Data;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.PaymentService
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly PaymentDbContext _dbContext;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(PaymentDbContext dbContext, ILogger<OrderCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = context.Message;
            _logger.LogWarning("Processing payment for OrderId: {OrderId}", message.OrderId);

            // Simulate payment process
            var success = new Random().Next(1, 10) > 3; // 70% chance of success

            if (success)
            {
                _dbContext.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = message.OrderId,
                    Amount = message.Amount,
                    Status = "Success"
                });

                await _dbContext.SaveChangesAsync();

                // Publish PaymentSucceeded
                await context.Publish(new PaymentSucceeded(message.OrderId,message.ProductName,message.Quantity,message.Amount));
                _logger.LogWarning("Payment succeeded for Order {OrderId}", message.OrderId);
            }
            else
            {
                _dbContext.Payments.Add(new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = message.OrderId,
                    Amount = message.Amount,
                    Status = "Failed"
                });

                await _dbContext.SaveChangesAsync();

                // Publish PaymentFailed
                await context.Publish(new PaymentFailed(message.OrderId, "Payment declined"));
                _logger.LogWarning("Payment failed for Order {OrderId}", message.OrderId);
            }
        }
    }
}
