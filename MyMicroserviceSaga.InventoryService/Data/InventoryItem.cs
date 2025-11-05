using MyMicroservicesSaga.SharedContract;

namespace MyMicroservicesSaga.InventoryService.Data
{
    public class InventoryItem : BaseEntity
    {
        public string ProductName { get; set; } = string.Empty;

        public int Stock { get; set; }
    }
}
