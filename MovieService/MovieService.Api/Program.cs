using MovieService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключаемся ко ВТОРОЙ базе данных в Docker
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=cinema_movie_db;Username=admin;Password=superpassword"));

// Подключаем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Настройка Swagger для MovieService
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    { 
        Title = "MovieService.Api", 
        Version = "v1" 
    });
});

var app = builder.Build();

// 3. Включаем Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MovieService.Api v1");
    });
}

// Обязательно маппим контроллеры перед запуском
app.MapControllers();

// 4. Автоматически создаем базу фильма и таблицу Movies при старте в Docker
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();