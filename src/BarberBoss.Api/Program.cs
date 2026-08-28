using BarberBoss.Api.Filters;
using BarberBoss.Api.Middlewares;
using BarberBoss.Application;
using BarberBoss.Exception;
using BarberBoss.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware to handle culture based on Accept-Language header
app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/health", () =>
{
    return Results.Ok(new { status = ResourceErrorMessages.HEALTH });
});

app.MapControllers();

app.Run();
