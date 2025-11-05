var builder = DistributedApplication.CreateBuilder(args);

// Create PostgreSQL container
var postgres = builder.AddPostgres("postgres")
    .WithVolume("postgres_data", "/var/lib/postgresql/data")
    .WithPgAdmin();

// Each service gets its own database
var orderDb = postgres.AddDatabase("orderdb");
var paymentDb = postgres.AddDatabase("paymentdb");
var inventoryDb = postgres.AddDatabase("inventorydb");

// Add RabbitMQ
var rabbit = builder.AddRabbitMQ("rabbitmq");

// Register microservices with references + wait dependencies
builder.AddProject<Projects.MyMicroservicesSaga_OrderService>("orderservice")
    .WithReference(orderDb)
    .WithReference(rabbit)
    .WaitFor(postgres)
    .WaitFor(orderDb)
    .WaitFor(rabbit);

builder.AddProject<Projects.MyMicroservicesSaga_PaymentService>("paymentservice")
    .WithReference(paymentDb)
    .WithReference(rabbit)
    .WaitFor(postgres)
    .WaitFor(paymentDb)
    .WaitFor(rabbit);

builder.AddProject<Projects.MyMicroservicesSaga_InventoryService>("inventoryservice")
    .WithReference(inventoryDb)
    .WithReference(rabbit)
    .WaitFor(postgres)
    .WaitFor(inventoryDb)
    .WaitFor(rabbit);

builder.AddProject<Projects.MyMicroservicesSaga_NotificationService>("notificationservice")
    .WithReference(rabbit)
    .WaitFor(rabbit);

builder.AddProject<Projects.SagaOrchestratorService>("sagaorchestratorservice")
    .WithReference(rabbit)
    .WaitFor(rabbit);

builder.Build().Run();
