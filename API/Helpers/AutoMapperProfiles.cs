using API.DTOs;
using API.DTOs.IdentityDTOs;
using AutoMapper;
using Core.Entities;
using Core.Entities.Orders;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(pDto => pDto.ProductBrand, o => o.MapFrom(p => p.ProductBrand.Name))
                .ForMember(pDto => pDto.ProductType, o => o.MapFrom(p => p.ProductType.Name))
                .ForMember(pDto => pDto.PictureUrl, o => o.MapFrom<UrlResolverHelper>());
            CreateMap<Core.Entities.Identity.Address, AddressDTO>().ReverseMap();
            CreateMap<ShoppingCartItemDTO, ShoppingCartItem>();
            CreateMap<CustomerShoppingCartDTO, CustomerShoppingCart>();
            CreateMap<AddressDTO, Core.Entities.Orders.Address>();
            CreateMap<Order, OrderToReturnDTO>()
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.ShippingPrice, o => o.MapFrom(s => s.DeliveryMethod.Price));
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ItemOrdered.ProductItemId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());
        }
    }
}