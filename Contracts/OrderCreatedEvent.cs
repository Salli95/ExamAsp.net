namespace Contracts;

public record OrderCreatedEvent(
    int OrderId,
    string UserId,
    int MovieId,
    int SeatsCount,
    DateTime OrderDate
);