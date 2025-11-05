using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyMicroservicesSaga.OrderService.Data;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.OrderService
{
    public class PaymentFailedConsumer : IConsumer<PaymentFailed>
    {
        private readonly OrderDbContext _dbContext;
        private readonly ILogger<PaymentFailedConsumer> _logger;

        public PaymentFailedConsumer(OrderDbContext dbContext, ILogger<PaymentFailedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailed> context)
        {
            var message = context.Message;
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == message.OrderId);

            if (order == null)
            {
                _logger.LogWarning("Order not found for failed payment: {OrderId}", message.OrderId);
                return;
            }

            // ❌ Mark as failed
            order.Status = "PaymentFailed";
            await _dbContext.SaveChangesAsync();

            _logger.LogWarning("Order {OrderId} marked as failed due to payment failure: {Reason}",
                message.OrderId, message.Reason);
        }
    }
}
