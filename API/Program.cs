using API.Database;
using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddIdentityService(builder.Configuration); //Check Identity Token of user to check if its valid user request

var app = builder.Build();

//Exception Handling Middlerware goes at top of the Middlerware
//Global Exception handling at Server Side
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseCors("myPolicy");

//order needs to be proper authentication before authorize
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scoped = app.Services.CreateScope();
var services = scoped.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();

    await context.Database.MigrateAsync();

    await Seed.SeedUsers(context);
     
}
catch(Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration.");
}

app.Run();
      