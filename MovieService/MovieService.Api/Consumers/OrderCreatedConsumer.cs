using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MovieService.Infrastructure;

namespace MovieService.Api.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly MovieDbContext _context;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(MovieDbContext context, ILogger<OrderCreatedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == message.MovieId);

        if (movie == null)
        {
            _logger.LogWarning("Movie {MovieId} was not found for order {OrderId}.", message.MovieId, message.OrderId);
            return;
        }

        if (movie.AvailableSeats < message.SeatsCount)
        {
            _logger.LogWarning(
                "Not enough seats for movie {MovieId}. Requested: {Requested}, Available: {Available}",
                message.MovieId,
                message.SeatsCount,
                movie.AvailableSeats
            );

            return;
        }

        movie.AvailableSeats -= message.SeatsCount;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Movie {MovieId} stock updated after order {OrderId}. Remaining seats: {Seats}",
            movie.Id,
            message.OrderId,
            movie.AvailableSeats
        );
    }
}