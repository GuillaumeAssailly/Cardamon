using Microsoft.EntityFrameworkCore;
using Pok�Service.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Pok�ServiceContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Pok�ServiceContext") ?? throw new InvalidOperationException("Connection string 'Pok�ServiceContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.EnableTryItOutByDefault();
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
