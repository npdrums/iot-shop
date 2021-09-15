using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;

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
            CreateMap<UserAddress, UserAddressDTO>().ReverseMap();
            CreateMap<ShoppingCartItemDTO, ShoppingCartItem>();
            CreateMap<CustomerShoppingCartDTO, CustomerShoppingCart>();
        }
    }
}