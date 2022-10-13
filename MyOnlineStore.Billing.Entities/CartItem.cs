namespace MyOnlineStore.Billing.Entities
{
    public class CartItem
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
    }
}