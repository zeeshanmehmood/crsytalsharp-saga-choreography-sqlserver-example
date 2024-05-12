using System;
using CrystalSharp.Domain.Infrastructure;
using Newtonsoft.Json;

namespace CSSagaChoreographySqlServerExample.Application.Domain.Aggregates.OrderAggregate.Events
{
    public class OrderRejectedDomainEvent : DomainEvent
    {
        public string OrderStatus { get; set; }

        public OrderRejectedDomainEvent(Guid streamId, string orderStatus)
        {
            StreamId = streamId;
            OrderStatus = orderStatus;
        }

        [JsonConstructor]
        public OrderRejectedDomainEvent(Guid streamId,
            string orderStatus,
            int entityStatus,
            DateTime createdOn,
            DateTime? modifiedOn,
            long version)
        {
            StreamId = streamId;
            OrderStatus = orderStatus;
            EntityStatus = entityStatus;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
            Version = version;
        }
    }
}
