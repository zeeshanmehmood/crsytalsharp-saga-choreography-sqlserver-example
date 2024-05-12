using System;
using CrystalSharp.Sagas;

namespace CSSagaChoreographySqlServerExample.Application.OrderSaga.Transactions
{
    public class TransferPaymentTransaction : ISagaTransaction
    {
        public Guid GlobalUId { get; set; }
    }
}
