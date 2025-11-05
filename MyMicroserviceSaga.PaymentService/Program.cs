using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyMicroservicesSaga.PaymentService.Data;

var builder = WebApplication.CreateBuilder(args);

// Postgres Connection
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("paymentdb")));

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

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Apply Migration
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
