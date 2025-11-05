namespace MyMicroservicesSaga.SharedContract
{
    public record OrderCreated(Guid OrderId, string ProductName, int Quantity, decimal Amount);
    public record PaymentSucceeded(Guid OrderId, string ProductName, int Quantity, decimal Amount);
    public record PaymentFailed(Guid OrderId, string Reason);
    public record InventoryAdjusted(Guid OrderId, string ProductName, int Quantity);
    public record InventoryFailed(Guid OrderId, string Reason);
    public record PaymentRolledBack(Guid OrderId,string Reason);
    public record OrderFailed(Guid OrderId, string Reason);
}
