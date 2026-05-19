using System.Security.Claims;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderingService.Infrastructure;

namespace OrderingService.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(OrderDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            if (order.MovieId <= 0)
                return BadRequest(new { message = "MovieId must be greater than zero." });

            if (order.SeatsCount <= 0)
                return BadRequest(new { message = "SeatsCount must be greater than zero." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized(new { message = "User id not found in token." });

            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new OrderCreatedEvent(
                order.Id,
                order.UserId,
                order.MovieId,
                order.SeatsCount,
                order.OrderDate
            ));

            return Ok(new
            {
                message = "Заказ успешно оформлен!",
                orderId = order.Id
            });
        }
    }
}