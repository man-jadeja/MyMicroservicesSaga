using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyMicroservicesSaga.PaymentService.Data;
using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.PaymentService.Orchestration_Consumer
{
    public class RollbackPaymentConsumer : IConsumer<RollbackPaymentCommand>
    {
        private readonly PaymentDbContext _dbContext;
        private readonly ILogger<RollbackPaymentConsumer> _logger;

        public RollbackPaymentConsumer(PaymentDbContext dbContext, ILogger<RollbackPaymentConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RollbackPaymentCommand> context)
        {
            var message = context.Message;
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(o => o.OrderId == message.OrderId);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found for failed order: {OrderId}", message.OrderId);
                return;
            }

            // ❌ Mark as failed
            payment.Status = "PaymentFailed";

            await _dbContext.SaveChangesAsync();

            _logger.LogWarning("Payment rollback successful for OrderId: {OrderId}", message.OrderId);

            // Publish event: PaymentRolledBack
            await context.Publish(new PaymentRolledBackEvent(message.OrderId, "Inventory failure triggered payment rollback"));
        }
    }
}
