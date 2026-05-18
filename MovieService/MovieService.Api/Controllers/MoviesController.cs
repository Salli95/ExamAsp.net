using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieService.Infrastructure;

namespace MovieService.Api.Controllers
{
    [ApiController]
    [Route("api/movies")] // Переписали роут жестко текстом, без токенов []
    public class MoviesController : ControllerBase
    {
        private readonly MovieDbContext _context;

        public MoviesController(MovieDbContext context)
        {
            _context = context;
        }

        // Явно указываем имя эндпоинта для Swagger
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _context.Movies.ToListAsync();
            return Ok(movies);
        }

        // Явно указываем имя эндпоинта для Swagger
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Фильм успешно добавлен!", movieId = movie.Id });
        }
    }
}