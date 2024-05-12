using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrystalSharp.Application;
using CrystalSharp.Common.Extensions;
using CrystalSharp.Common.Serialization;
using CrystalSharp.Infrastructure.EventStoresPersistence;
using CrystalSharp.Sagas;
using CSSagaChoreographySqlServerExample.Application.Domain.Aggregates.OrderAggregate;
using CSSagaChoreographySqlServerExample.Application.Domain.Aggregates.OrderAggregate.Events;
using CSSagaChoreographySqlServerExample.Application.OrderSaga.Transactions;

namespace CSSagaChoreographySqlServerExample.Application.OrderSaga
{
    public class OrderSaga : SagaChoreography<OrderSagaLocator, PlaceOrderTransaction>,
    IAmStartedBy<PlaceOrderTransaction>,
    ISagaChoreographyEvent<Order, int, OrderPlacedDomainEvent>,
    ISagaChoreographyEvent<Order, int, PaymentTransferredDomainEvent>,
    ISagaChoreographyEvent<Order, int, OrderDeliveredDomainEvent>
    {
        private readonly IAggregateEventStore<int> _eventStore;
        private readonly ISagaLocator _sagaLocator;
        private readonly ISagaStore _sagaStore;

        public OrderSaga(IAggregateEventStore<int> eventStore,
            ISagaStore sagaStore,
            OrderSagaLocator sagaLocator,
            ISagaTransactionExecutor sagaTransactionExecutor)
            : base(sagaStore, sagaLocator, sagaTransactionExecutor)
        {
            _eventStore = eventStore;
            _sagaLocator = sagaLocator;
            _sagaStore = sagaStore;
        }

        public override async Task<SagaTransactionResult> Handle(PlaceOrderTransaction request, CancellationToken cancellationToken = default)
        {
            Order order = Order.PlaceOrder(request.Product,
                request.Quantity,
                request.UnitPrice,
                request.AmountPaid);

            await _eventStore.Store(order, cancellationToken).ConfigureAwait(false);

            string correlationId = await _sagaLocator.Locate(order.GlobalUId);
            SagaTransactionMeta sagaTransactionMeta = await _sagaStore.Get(correlationId, cancellationToken).ConfigureAwait(false);

            if (sagaTransactionMeta.State == SagaState.Aborted)
            {
                string[] errors = Array.Empty<string>();

                if (!string.IsNullOrEmpty(sagaTransactionMeta.ErrorTrail))
                {
                    IEnumerable<Error> errorTrail = Serializer.Deserialize<IEnumerable<Error>>(sagaTransactionMeta.ErrorTrail);

                    if (errorTrail.HasAny())
                    {
                        errors = errorTrail.Select(x => x.Message).ToArray();
                    }
                }

                return await Fail(order.GlobalUId, errors);
            }

            return await Ok(order.GlobalUId);
        }

        public async Task Handle(OrderPlacedDomainEvent @event, CancellationToken cancellationToken = default)
        {
            TransferPaymentTransaction transaction = new() { GlobalUId = @event.StreamId };

            async Task compensation() { await RejectOrder(@event.StreamId, cancellationToken).ConfigureAwait(false); }

            await Execute(@event.StreamId, transaction, compensation, cancellationToken).ConfigureAwait(false);
        }

        public async Task Handle(PaymentTransferredDomainEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event.PaymentTransferred)
            {
                DeliverOrderTransaction transaction = new() { GlobalUId = @event.StreamId };

                await Execute(@event.StreamId, transaction, null, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task Handle(OrderDeliveredDomainEvent @event, CancellationToken cancellationToken = default)
        {
            await MarkAsComplete(@event.StreamId, cancellationToken).ConfigureAwait(false);
        }

        private async Task RejectOrder(Guid globalUId, CancellationToken cancellationToken)
        {
            Order order = await _eventStore.Get<Order>(globalUId, cancellationToken).ConfigureAwait(false);

            if (order != null)
            {
                order.Reject();

                await _eventStore.Store(order, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
