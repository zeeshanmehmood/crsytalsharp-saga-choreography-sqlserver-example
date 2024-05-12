using System;
using CrystalSharp.Domain.Infrastructure;
using Newtonsoft.Json;

namespace CSSagaChoreographySqlServerExample.Application.Domain.Aggregates.OrderAggregate.Events
{
    public class PaymentTransferredDomainEvent : DomainEvent
    {
        public bool PaymentTransferred { get; set; }

        public PaymentTransferredDomainEvent(Guid streamId,
     bool paymentTransferred)
        {
            StreamId = streamId;
            PaymentTransferred = paymentTransferred;
        }

        [JsonConstructor]
        public PaymentTransferredDomainEvent(Guid streamId,
            bool paymentTransferred,
            int entityStatus,
            DateTime createdOn,
            DateTime? modifiedOn,
            long version)
        {
            StreamId = streamId;
            PaymentTransferred = paymentTransferred;
            EntityStatus = entityStatus;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
            Version = version;
        }
    }
}
