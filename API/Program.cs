using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddIdentityService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("myPolicy");

//order needs to be proper authentication before authorize
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
