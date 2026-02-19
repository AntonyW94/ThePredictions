using ThePredictions.API;
using ThePredictions.API.Middleware;
using ThePredictions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices();
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSecurityHeaders();

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();