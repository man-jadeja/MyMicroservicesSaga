using MassTransit;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.NotificationService.Choreography_Consumer
{
    public class OrderFailedConsumer : IConsumer<OrderFailed>
    {
        private readonly ILogger<OrderFailedConsumer> _logger;

        public OrderFailedConsumer(ILogger<OrderFailedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<OrderFailed> context)
        {
            var message = context.Message;
            _logger.LogWarning("Received OrderFailed event for OrderId: {OrderId} - {Reason}", message.OrderId, message.Reason);

            // Here, we can send email / SMS / push notification
            return Task.CompletedTask;
        }
    }
}
