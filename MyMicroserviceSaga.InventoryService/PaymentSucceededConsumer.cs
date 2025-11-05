using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyMicroservicesSaga.InventoryService.Data;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.InventoryService
{
    public class PaymentSucceededConsumer : IConsumer<PaymentSucceeded>
    {
        private readonly InventoryDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentSucceededConsumer> _logger;

        public PaymentSucceededConsumer(
            InventoryDbContext dbContext,
            IPublishEndpoint publishEndpoint,
            ILogger<PaymentSucceededConsumer> logger)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentSucceeded> context)
        {
            var message = context.Message;

            try
            {
                // Try adjusting inventory
                var item = await _dbContext.InventoryItems.FirstOrDefaultAsync(p => p.ProductName == message.ProductName);

                if (item == null || item.Stock < message.Quantity)
                {
                    await _publishEndpoint.Publish(new InventoryFailed(message.OrderId, "Out of stock"));
                    _logger.LogWarning("Inventory failed for Order {OrderId}: Out of stock", message.OrderId);
                    return;
                }

                item.Stock -= message.Quantity;
                await _dbContext.SaveChangesAsync();

                await _publishEndpoint.Publish(new InventoryAdjusted(message.OrderId, item.ProductName, message.Quantity));
                _logger.LogWarning("Inventory adjusted for Order {OrderId}", message.OrderId);
            }
            catch (Exception ex)
            {
                await _publishEndpoint.Publish(new InventoryFailed(message.OrderId, ex.Message));
                _logger.LogError(ex, "Inventory update failed for Order {OrderId}", message.OrderId);
            }
        }
    }
}