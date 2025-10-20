using BookStoreEcommerce.Data;
using BookStoreEcommerce.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddDbContext<BookStoreEcommerce.Data.StoreDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
// );

builder.Services.AddScoped<IProductRepo, ProductRepo>();

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
