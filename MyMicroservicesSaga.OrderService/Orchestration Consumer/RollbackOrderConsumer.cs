using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyMicroservicesSaga.OrderService.Data;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.OrderService.Orchestration_Consumer
{
    public class RollbackOrderConsumer : IConsumer<RollbackOrderCommand>
    {
        private readonly OrderDbContext _dbContext;
        private readonly ILogger<RollbackOrderConsumer> _logger;

        public RollbackOrderConsumer(OrderDbContext dbContext, ILogger<RollbackOrderConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RollbackOrderCommand> context)
        {
            var message = context.Message;
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == message.OrderId);

            if (order != null)
            {
                _logger.LogWarning("Rolling back order {OrderId} due to payment rollback", order.Id);

                // Option 1: Mark as failed
                order.Status = "Failed";

                // Option 2 (Alternative): Delete the order
                // _dbContext.Orders.Remove(order);

                await _dbContext.SaveChangesAsync();

                _logger.LogWarning("Order {OrderId} marked as failed and OrderFailed event published.", order.Id);

                // Publish OrderFailed event (optional, for notifications)
                await context.Publish(new OrderFailedEvent(order.Id, message.Reason));
            }
            else
            {
                _logger.LogWarning("Order {OrderId} not found for rollback.", message.OrderId);
            }
        }
    }
}
