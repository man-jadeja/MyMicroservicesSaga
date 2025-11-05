namespace MyMicroservicesSaga.SharedContract
{
    public record StartOrderSagaCommand(Guid OrderId, string ProductName, int Quantity, decimal Amount);
    public record RollbackOrderCommand(Guid OrderId, string Reason);
    public record ProcessPaymentCommand(Guid OrderId, string ProductName, int Quantity, decimal Amount);
    public record RollbackPaymentCommand(Guid OrderId, string Reason);
    public record AdjustInventoryCommand(Guid OrderId, string ProductName, int Quantity);
    public record RollbackInventoryCommand(Guid OrderId, string Reason);
    public record FailOrderCommand(Guid OrderId, string Reason);
    public record CompleteOrderCommand(Guid OrderId, string ProductName, int Quantity);
    public record NotifyCustomerCommand(Guid OrderId, string ProductName, int Quantity, decimal Amount, string Message);

    // Internal Events
    public record PaymentSucceededEvent(Guid OrderId, string ProductName, int Quantity, decimal Amount);
    public record PaymentFailedEvent(Guid OrderId, string Reason);
    public record InventoryAdjustedEvent(Guid OrderId, string ProductName, int Quantity);
    public record InventoryFailedEvent(Guid OrderId, string Reason);
    public record PaymentRolledBackEvent(Guid OrderId, string Reason);
    public record OrderFailedEvent(Guid OrderId, string Reason);
}
