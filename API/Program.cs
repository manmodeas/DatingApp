using API.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(ops =>
{
    ops.UseSqlite(builder.Configuration.GetConnectionString("MyDatabase"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("myPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("myPolicy");

app.MapControllers();

app.Run();
