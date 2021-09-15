using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CustomerShoppingCartDTO
    {
        [Required]
        public string Id { get; set; }
        public List<ShoppingCartItemDTO> Items { get; set; }
    }
}