using OrderingService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключаемся к ТРЕТЬЕЙ базе данных в Docker
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=cinema_ordering_db;Username=admin;Password=superpassword"));

// Подключаем контроллеры
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Настройка Swagger для OrderingService
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    { 
        Title = "OrderingService.Api", 
        Version = "v1" 
    });
});

var app = builder.Build();

// 3. Включаем Swagger UI в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderingService.Api v1");
    });
}

// Обязательно маппим контроллеры перед запуском
app.MapControllers();

// 4. Автоматически создаем базу заказов и таблицу Orders при старте в Docker
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();