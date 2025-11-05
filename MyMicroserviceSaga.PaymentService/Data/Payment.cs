using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.PaymentService.Data
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
