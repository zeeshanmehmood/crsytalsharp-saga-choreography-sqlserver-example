using CrystalSharp.Domain;
using CSSagaChoreographySqlServerExample.Application.Domain.Aggregates.OrderAggregate.Events;

namespace CSSagaChoreographySqlServerExample.Application.Domain.Aggregates.OrderAggregate
{
    public class Order : AggregateRoot<int>
    {
        public string Product { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Amount { get; private set; }
        public decimal AmountPaid { get; private set; }
        public bool PaymentTransferred { get; private set; } = false;
        public bool Delivered { get; private set; } = false;
        public string OrderStatus { get; private set; } = string.Empty;

        public static Order PlaceOrder(string product,
            int quantity,
            decimal unitPrice,
            decimal amountPaid)
        {
            Order order = new()
            {
                Product = product,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Amount = quantity * unitPrice,
                AmountPaid = amountPaid
            };

            order.Raise(new OrderPlacedDomainEvent(order.GlobalUId,
                order.Product,
                order.Quantity,
                order.UnitPrice,
                order.Amount,
                order.AmountPaid,
                order.PaymentTransferred,
                order.Delivered));

            return order;
        }

        public void TransferPayment()
        {
            ValidatePayment(Amount, AmountPaid);

            PaymentTransferred = true;

            Raise(new PaymentTransferredDomainEvent(GlobalUId, PaymentTransferred));
        }

        public void Deliver()
        {
            ValidatePaymentTransfer(PaymentTransferred);

            Delivered = true;
            OrderStatus = "Delivered";

            Raise(new OrderDeliveredDomainEvent(GlobalUId, Delivered, OrderStatus));
        }

        public void Reject()
        {
            OrderStatus = "Rejected";

            Raise(new OrderRejectedDomainEvent(GlobalUId, OrderStatus));
        }

        private void ValidatePayment(decimal totalAmount, decimal amountPaid)
        {
            if (amountPaid < totalAmount)
            {
                ThrowDomainException("The paid amount is less than the total amount.");
            }
        }

        private void ValidatePaymentTransfer(bool paymentTransferred)
        {
            if (!paymentTransferred)
            {
                ThrowDomainException("The payment was not transferred. Please transfer the payment before the order is delivered.");
            }
        }

        private void Apply(OrderPlacedDomainEvent @event)
        {
            Product = @event.Product;
            Quantity = @event.Quantity;
            UnitPrice = @event.UnitPrice;
            Amount = @event.Amount;
            AmountPaid = @event.AmountPaid;
            PaymentTransferred = @event.PaymentTransferred;
            Delivered = @event.Delivered;
        }

        private void Apply(PaymentTransferredDomainEvent @event)
        {
            PaymentTransferred = @event.PaymentTransferred;
        }

        private void Apply(OrderDeliveredDomainEvent @event)
        {
            Delivered = @event.Delivered;
            OrderStatus = @event.OrderStatus;
        }

        private void Apply(OrderRejectedDomainEvent @event)
        {
            OrderStatus = @event.OrderStatus;
        }
    }
}
