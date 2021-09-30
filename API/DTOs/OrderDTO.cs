namespace API.DTOs
{
    public class OrderDTO
    {
        public string ShoppingCartId { get; set; }
        public int DeliveryMethodId { get; set; }
        public UserAddressDTO ShipToAddress { get; set; }
    }
}