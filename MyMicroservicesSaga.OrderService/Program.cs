using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyMicroservicesSaga.OrderService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Postgres Connection
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("orderdb")));

// MassTransit + RabbitMQ setup
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        
        // Retrieve RabbitMQ connection string provided by Aspire
        var host = configuration.GetConnectionString("rabbitmq"); // Name must match AddRabbitMQ("rabbitmq")
        cfg.Host(host);

        cfg.ConfigureEndpoints(context); // Automatically configures consumers
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Apply Migration
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        dbContext.Database.Migrate();
    }
}

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
