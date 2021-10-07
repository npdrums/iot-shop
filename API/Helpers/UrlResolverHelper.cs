using API.DTOs;
using AutoMapper;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
    public class UrlResolverHelper : IValueResolver<Product, ProductDTO, string>
    {
        private readonly IConfiguration _configuration;
        public UrlResolverHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Product source, ProductDTO destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
            {
                return _configuration["ApiUrl"] + source.PictureUrl;
            }
            return null;
        }
    }
}