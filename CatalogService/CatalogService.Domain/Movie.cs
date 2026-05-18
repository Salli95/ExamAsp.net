namespace CatalogService.Domain;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Genre { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int ViewsCount { get; set; } // Будет обновляться через RabbitMQ
}