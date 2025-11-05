using MassTransit;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.NotificationService
{
    public class InventoryAdjustedConsumer : IConsumer<InventoryAdjusted>
    {
        private readonly ILogger<InventoryAdjustedConsumer> _logger;

        public InventoryAdjustedConsumer(ILogger<InventoryAdjustedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<InventoryAdjusted> context)
        {
            var message = context.Message;
            _logger.LogWarning("Notification: Order {OrderId} completed successfully for product {ProductName} - {Quantity}.",
                message.OrderId, message.ProductName, message.Quantity);

            // Here, we can send email / SMS / push notification
            return Task.CompletedTask;
        }
    }
}
