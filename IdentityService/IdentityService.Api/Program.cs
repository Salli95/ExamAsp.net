using IdentityService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка подключения к базе данных PostgreSQL в Docker напрямую
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=cinema_identity_db;Username=admin;Password=superpassword"));

// 2. Включаем поддержку классических контроллеров и MVC
builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();

// 3. Явная настройка Swagger для генерации документации по контроллерам
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    { 
        Title = "IdentityService.Api", 
        Version = "v1" 
    });
});

var app = builder.Build();

// 4. Включаем интерфейс Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.Api v1");
    });
}

// 5. Настраиваем маршрутизацию
// app.UseAuthorization(); // Раскомментируй позже, когда добавим роли
app.MapControllers(); // Это ключевая строчка, которая связывает AuthController со Swagger

// 6. Автоматически проверяем наличие базы и таблиц при старте
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    // Теперь база уже создана, метод просто будет проверять, что всё на месте
    dbContext.Database.EnsureCreated();
}

app.Run();