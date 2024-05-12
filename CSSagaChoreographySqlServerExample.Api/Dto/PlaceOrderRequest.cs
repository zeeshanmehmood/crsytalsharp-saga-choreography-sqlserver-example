namespace CSSagaChoreographySqlServerExample.Api.Dto
{
    public class PlaceOrderRequest
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal AmountPaid { get; set; }
    }
}
