namespace OrderingService.Infrastructure;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty; // Идентификатор пользователя из IdentityService
    public int MovieId { get; set; }                    // Идентификатор фильма из MovieService
    public int SeatsCount { get; set; }                 // Кол-во купленных мест/билетов
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}