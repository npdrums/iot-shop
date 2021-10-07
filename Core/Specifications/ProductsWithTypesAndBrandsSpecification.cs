using Core.Entities;

namespace Core.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(ProductSpecificationParameters productParams)
            : base(p =>
               (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
               (!productParams.BrandId.HasValue || p.ProductBrandId == productParams.BrandId) &&
               (!productParams.TypeId.HasValue || p.ProductTypeId == productParams.TypeId))
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
            AddOrderBy(p => p.Name);
            ApplyPagination(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

            if (!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "byPriceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "byPriceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;
                    default:
                        AddOrderBy(n => n.Name);
                        break;
                }
            }
        }

        public ProductsWithTypesAndBrandsSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.ProductBrand);
            AddInclude(p => p.ProductType);
        }
    }
}