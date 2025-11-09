using System.Security.Claims;
using System.Text;
using BookStoreEcommerce.Auth;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Jobs;
using BookStoreEcommerce.Messaging.Consumers;
using BookStoreEcommerce.Models;
using BookStoreEcommerce.Profiles;
using BookStoreEcommerce.Services;
using BookStoreEcommerce.Services.Auth;
using BookStoreEcommerce.Services.Caching;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["http://localhost:4200"];

var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<StoreDbContext>(opt =>
    opt.UseNpgsql(cs));

var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAppAuthorization();

var redisCfg = builder.Configuration.GetSection("Redis");
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisCfg["Configuration"];
    options.InstanceName = redisCfg["InstanceName"]; // prefix all keys
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:Configuration"]!));

builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UsePostgreSqlStorage(builder.Configuration["Hangfire:PostgresConnection"]);
});
builder.Services.AddHangfireServer();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // consumers
    x.AddConsumer<InactiveCustomerReengageConsumer>();

    x.UsingRabbitMq((context, cfgRabbit) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        var port = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672");
        var user = builder.Configuration["RabbitMq:Username"] ?? "guest";
        var pass = builder.Configuration["RabbitMq:Password"] ?? "guest";
        var vhost = builder.Configuration["RabbitMq:VirtualHost"] ?? "/";

        cfgRabbit.Host(host, vhost, h =>
        {
            h.Username(user);
            h.Password(pass);
        });

        cfgRabbit.ConfigureEndpoints(context);

    });
});


builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();
builder.Services.AddScoped<IOrderItemsRepo, OrderItemsRepo>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemsService, OrderItemsService>();
builder.Services.AddScoped<IUserActivityService, UserActivityService>();

builder.Services.AddScoped<IReengageInactiveUsersJob, ReengageInactiveUsersJob>();


builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
          .SetIsOriginAllowed(origin =>
          {
              if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
              return (uri.Host == "localhost" || uri.Host == "127.0.0.1") && uri.Port == 4200;
          })
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});


builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

PrepDb.PrepPopulation(app);

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (_context, next) =>
{
    if (_context.User.Identity?.IsAuthenticated == true)
    {
        var userIdClaim = _context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            var userActivityService = _context.RequestServices.GetRequiredService<IUserActivityService>();
            // fire and forget
            await userActivityService.TouchAsync(userId);
        }
    }

    await next();
});

app.UseHangfireDashboard(builder.Configuration["Hangfire:DashboardPath"] ?? "/hangfire");

app.MapControllers();

RecurringJob.AddOrUpdate<IReengageInactiveUsersJob>(
    "reengage-inactive-7d",
    job => job.Run(CancellationToken.None),
    Cron.Daily,                      // every day at 00:00 server time
    timeZone: TimeZoneInfo.Local
);

app.Run();
