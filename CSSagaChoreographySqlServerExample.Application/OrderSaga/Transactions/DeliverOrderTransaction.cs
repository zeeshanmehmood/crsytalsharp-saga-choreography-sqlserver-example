using System;
using CrystalSharp.Sagas;

namespace CSSagaChoreographySqlServerExample.Application.OrderSaga.Transactions
{
    public class DeliverOrderTransaction : ISagaTransaction
    {
        public Guid GlobalUId { get; set; }
    }
}
