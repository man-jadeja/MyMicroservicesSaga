using MassTransit;
using MyMicroservicesSaga.SharedContract;

namespace SagaOrchestratorService
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public State PaymentProcessing { get; private set; } = null!;
        public State InventoryProcessing { get; private set; } = null!;
        public State Completed { get; private set; } = null!;
        public State Failed { get; private set; } = null!;

        // Events
        public Event<StartOrderSagaCommand> StartOrderSaga { get; private set; } = null!;
        public Event<PaymentSucceededEvent> PaymentSucceeded { get; private set; } = null!;
        public Event<PaymentFailedEvent> PaymentFailed { get; private set; } = null!;
        public Event<InventoryAdjustedEvent> InventoryAdjusted { get; private set; } = null!;
        public Event<InventoryFailedEvent> InventoryFailed { get; private set; } = null!;

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            // Correlate events by OrderId
            Event(() => StartOrderSaga, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentSucceeded, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => PaymentFailed, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => InventoryAdjusted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => InventoryFailed, x => x.CorrelateById(m => m.Message.OrderId));

            // 1️ Initial order creation
            Initially(
                When(StartOrderSaga)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.ProductName = context.Message.ProductName;
                        context.Saga.Quantity = context.Message.Quantity;
                        context.Saga.Amount = context.Message.Amount;
                    })
                    .Publish(context => new ProcessPaymentCommand(
                        context.Saga.OrderId,
                        context.Saga.ProductName,
                        context.Saga.Quantity,
                        context.Saga.Amount
                    ))
                    .TransitionTo(PaymentProcessing)
            );

            // 2️ Handle Payment Success/Failure
            During(PaymentProcessing,
                When(PaymentSucceeded)
                    .Publish(context => new AdjustInventoryCommand(
                        context.Saga.OrderId,
                        context.Saga.ProductName,
                        context.Saga.Quantity
                    ))
                    .TransitionTo(InventoryProcessing),

                When(PaymentFailed)
                    .Publish(context => new RollbackOrderCommand(
                        context.Saga.OrderId,
                        context.Message.Reason
                    ))
                    .TransitionTo(Failed)
            );

            // 3️ Handle Inventory Success/Failure
            During(InventoryProcessing,
                When(InventoryAdjusted)
                    .Publish(context => new CompleteOrderCommand(
                        context.Saga.OrderId,
                        context.Saga.ProductName,
                        context.Saga.Quantity
                    ))
                    .Publish(context => new NotifyCustomerCommand(
                        context.Saga.OrderId,
                        context.Saga.ProductName,
                        context.Saga.Quantity,
                        context.Saga.Amount,
                        "Your order is successfully completed!"
                    ))
                    .TransitionTo(Completed),

                When(InventoryFailed)
                    .Publish(context => new RollbackPaymentCommand(
                        context.Saga.OrderId,
                        context.Message.Reason
                    ))
                    .Publish(context => new RollbackOrderCommand(
                        context.Saga.OrderId,
                        "Inventory failure after payment success"
                    ))
                    .TransitionTo(Failed)
            );

            SetCompletedWhenFinalized();
        }
    }
}
