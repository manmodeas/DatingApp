using API.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(ops =>
{
    ops.UseSqlite(builder.Configuration.GetConnectionString("MyDatabase"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.


app.MapControllers();

app.Run();
