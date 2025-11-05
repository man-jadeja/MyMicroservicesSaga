using MassTransit;
using SagaOrchestratorService;

var builder = WebApplication.CreateBuilder(args);

// MassTransit + RabbitMQ setup
builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .InMemoryRepository();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
