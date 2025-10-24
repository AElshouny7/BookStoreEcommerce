using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.DBContext;
using BookStoreEcommerce.Profiles;
using BookStoreEcommerce.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<StoreDbContext>(opt =>
    opt.UseNpgsql(cs));

builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


builder.Services.AddControllers();
builder.Services.AddAutoMapper(
    typeof(Program),              // web project
    typeof(CategoriesProfile),
    // typeof(CommonsProfile),
    typeof(OrdersProfile),
    typeof(OrderItemsProfile),
    typeof(ProductsProfile),      // domain library markers if any
    typeof(UsersProfile)
);// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
