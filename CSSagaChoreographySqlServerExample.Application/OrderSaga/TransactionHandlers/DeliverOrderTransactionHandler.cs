using System.Threading;
using System.Threading.Tasks;
using CrystalSharp.Infrastructure.EventStoresPersistence;
using CrystalSharp.Sagas;
using CSSagaChoreographySqlServerExample.Application.Domain.Aggregates.OrderAggregate;
using CSSagaChoreographySqlServerExample.Application.OrderSaga.Transactions;

namespace CSSagaChoreographySqlServerExample.Application.OrderSaga.TransactionHandlers
{
    public class DeliverOrderTransactionHandler : SagaTransactionHandler<DeliverOrderTransaction>
    {
        private readonly IAggregateEventStore<int> _eventStore;

        public DeliverOrderTransactionHandler(IAggregateEventStore<int> eventStore)
        {
            _eventStore = eventStore;
        }

        public override async Task<SagaTransactionResult> Handle(DeliverOrderTransaction request, CancellationToken cancellationToken = default)
        {
            Order order = await _eventStore.Get<Order>(request.GlobalUId, cancellationToken).ConfigureAwait(false);

            if (order == null)
            {
                return await Fail(request.GlobalUId, "Order not found.");
            }

            order.Deliver();
            await _eventStore.Store(order, cancellationToken).ConfigureAwait(false);

            return await Ok(order.GlobalUId);
        }
    }
}
