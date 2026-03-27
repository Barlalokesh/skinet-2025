using core.Interface;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

using API.Middleware;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using core.Entities;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// EF Core SQL Server
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Repositories & UnitOfWork
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// CORS
builder.Services.AddCors();

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connString = config.GetConnectionString("Redis")
                     ?? throw new Exception("Redis connection string is missing");

    Console.WriteLine("Redis connection string: " + connString);

    return ConnectionMultiplexer.Connect(connString);
});

// Services
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Identity
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
       .AddEntityFrameworkStores<StoreContext>();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionMiddleware>();

// CORS must come before SignalR / controllers
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

// Map controllers
app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>();

// Map SignalR hubs **before fallback**
app.MapHub<NotificationHub>("/hubs/notifications");

// Fallback must be last
app.MapFallbackToController("Index", "Fallback");

// Database migration & seed
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    throw;
}

app.Run();