using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.OrderService.Data
{
    public class Order : BaseEntity
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
