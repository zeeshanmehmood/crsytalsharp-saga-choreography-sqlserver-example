using System;
using CrystalSharp.Domain.Infrastructure;
using Newtonsoft.Json;

namespace CSSagaChoreographySqlServerExample.Application.Domain.Aggregates.OrderAggregate.Events
{
    public class OrderDeliveredDomainEvent : DomainEvent
    {
        public bool Delivered { get; set; }
        public string OrderStatus { get; set; }

        public OrderDeliveredDomainEvent(Guid streamId, bool delivered, string orderStatus)
        {
            StreamId = streamId;
            Delivered = delivered;
            OrderStatus = orderStatus;
        }

        [JsonConstructor]
        public OrderDeliveredDomainEvent(Guid streamId,
            bool delivered,
            string orderStatus,
            int entityStatus,
            DateTime createdOn,
            DateTime? modifiedOn,
            long version)
        {
            StreamId = streamId;
            Delivered = delivered;
            OrderStatus = orderStatus;
            EntityStatus = entityStatus;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
            Version = version;
        }
    }
}
