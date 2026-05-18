using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderingService.Infrastructure;

namespace OrderingService.Api.Controllers
{
    [ApiController]
    [Route("api/orders")] // Текстовый роут
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public OrdersController(OrderDbContext context)
        {
            _context = context;
        }

        // Эндпоинт для просмотра всех заказов
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders.ToListAsync();
            return Ok(orders);
        }

        // Эндпоинт для создания нового заказа (бронирования)
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Заказ успешно оформлен!", orderId = order.Id });
        }
    }
}