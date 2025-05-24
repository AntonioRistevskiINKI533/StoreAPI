using StoreAPI.Services;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;

        public ProductController(ILogger<ProductController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        //TODO should employee be able to delete/update a product?
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> AddProduct(AddProductRequest request)
        {
            try
            {
                await _productService.AddProduct(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> UpdateProduct(UpdateProductRequest request)
        {
            try
            {
                await _productService.UpdateProduct(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ActionResult<ProductData>), 200)]
        public async Task<ActionResult<ProductData>> GetProduct(int productId)
        {
            try
            {
                var product = await _productService.GetProduct(productId);

                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<ProductData>), 200)]
        public async Task<ActionResult<PagedModel<ProductData>>> GetAllProductsPaged(int pageIndex, int pageSize, int? companyId, string? productName)
        {
            try
            {
                var products = await _productService.GetAllProductsPaged(pageIndex, pageSize, companyId, productName);

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> RemoveProduct(int productId)
        {
            try
            {
                await _productService.RemoveProduct(productId);

                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}