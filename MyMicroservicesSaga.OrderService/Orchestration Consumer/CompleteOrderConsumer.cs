using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyMicroservicesSaga.OrderService.Data;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.OrderService.Orchestration_Consumer
{
    public class CompleteOrderConsumer : IConsumer<CompleteOrderCommand>
    {
        private readonly OrderDbContext _dbContext;
        private readonly ILogger<CompleteOrderConsumer> _logger;

        public CompleteOrderConsumer(OrderDbContext dbContext, ILogger<CompleteOrderConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CompleteOrderCommand> context)
        {
            var message = context.Message;

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == message.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for InventoryAdjusted event. OrderId: {OrderId}", message.OrderId);
                return;
            }

            order.Status = "Completed";
            await _dbContext.SaveChangesAsync();

            _logger.LogWarning("✅ Order {OrderId} marked as Completed after InventoryAdjusted", message.OrderId);
        }
    }
}
