using Core.Entities;

namespace Core.Specifications
{
    public class ProductsWithFiltersCountSpecification : BaseSpecification<Product>
    {
        public ProductsWithFiltersCountSpecification(ProductSpecificationParameters productParams)
            : base(p =>
               (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
               (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId) &&
               (!productParams.TypeId.HasValue || p.ProductTypeId == productParams.TypeId))
        {
        }
    }
}