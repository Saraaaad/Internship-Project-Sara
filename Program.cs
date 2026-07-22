using InternshipProjectSara.Data.Context;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
var app = builder.Build();

// Create Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReactApp");  // ← This must be before UseAuthentication

// init GlobalExceptionHandler
GlobalExceptionHandler.Configure(app);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();