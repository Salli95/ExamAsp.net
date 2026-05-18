namespace OrderingService.Domain;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // Кто купил (из IdentityService)
    public Guid MovieId { get; set; } // Что купил (из CatalogService)
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpirationDate { get; set; } // До какого числа доступен фильм
    public string Status { get; set; } = "Paid"; // Статус заказа
}