using API.DTOs.IdentityDTOs;

namespace API.DTOs
{
    public class OrderDTO
    {
        public string ShoppingCartId { get; set; }
        public int DeliveryMethodId { get; set; }
        public AddressDTO ShipToAddress { get; set; }
    }
}