using IdentityService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;
using IdentityService.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка подключения к базе данных PostgreSQL в Docker напрямую
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=cinema_identity_db;Username=admin;Password=superpassword"));
builder.Services.AddScoped<JwtTokenGenerator>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization();
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
app.UseAuthentication();
app.UseAuthorization();
// 5. Настраиваем маршрутизацию
// app.UseAuthorization(); // Раскомментируй позже, когда добавим роли
app.MapControllers(); // Это ключевая строчка, которая связывает AuthController со Swagger

// 6. Автоматически проверяем наличие базы и таблиц при старте
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    // Теперь база уже создана, метод просто будет проверять, что всё на месте
    dbContext.Database.EnsureCreated();
    if (!dbContext.Users.Any(u => u.Email == "admin@cinema.local"))
{
    dbContext.Users.Add(new IdentityService.Domain.User
    {
        Id = Guid.NewGuid(),
        Email = "admin@cinema.local",
        PasswordHash = IdentityService.Api.Services.PasswordHasher.Hash("Admin123!"),
        Role = "Admin"
    });

    dbContext.SaveChanges();
}
}





app.Run();