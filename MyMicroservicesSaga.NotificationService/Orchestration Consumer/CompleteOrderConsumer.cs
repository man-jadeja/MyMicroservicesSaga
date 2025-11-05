using MassTransit;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.NotificationService.Orchestration_Consumer
{
    public class CompleteOrderConsumer : IConsumer<CompleteOrderCommand>
    {
        private readonly ILogger<CompleteOrderConsumer> _logger;

        public CompleteOrderConsumer(ILogger<CompleteOrderConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<CompleteOrderCommand> context)
        {
            var message = context.Message;
            _logger.LogWarning("Notification: Order {OrderId} completed successfully for product {ProductName} - {Quantity}.",
                message.OrderId, message.ProductName, message.Quantity);

            // Here, we can send email / SMS / push notification
            return Task.CompletedTask;
        }
    }
}
