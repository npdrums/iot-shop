using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.DTOs;
using AutoMapper;
using API.Errors;
using Microsoft.AspNetCore.Http;

using API.Helpers;

namespace API.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IMapper _mapper;
        public ProductsController(IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductBrand> productBrandRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IMapper mapper)
        {
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _productsRepo = productsRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDTO>>> GetProducts(
            [FromQuery] ProductSpecificationParameters productParams)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(productParams);
            var countSpecification = new ProductsWithFiltersCountSpecification(productParams);
            var totalItems = await _productsRepo.CountAsync(countSpecification);
            var products = await _productsRepo.ListAsync(specification);
            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDTO>>(products);
            
            return Ok(new Pagination<ProductDTO>(productParams.PageIndex, productParams.PageSize,
                totalItems, data));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var specification = new ProductsWithTypesAndBrandsSpecification(id);
            var product = await _productsRepo.GetEntityWithSpec(specification);

            if (product == null)
            {
                return NotFound(new ApiErrorResponse(404));
            }

            return _mapper.Map<Product, ProductDTO>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<List<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepo.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<List<ProductType>>> GetProductTypes()
        {
            return Ok(await _productTypeRepo.ListAllAsync());
        }
    }
}