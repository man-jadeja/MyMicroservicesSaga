using MassTransit;

namespace SagaOrchestratorService
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }      // Unique per Saga (OrderId)
        public string CurrentState { get; set; } = "";
        public Guid OrderId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
