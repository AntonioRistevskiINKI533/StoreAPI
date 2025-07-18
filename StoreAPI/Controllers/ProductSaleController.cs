using StoreAPI.Services;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StoreAPI.Exceptions;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductSaleController : ControllerBase
    {

        private readonly ILogger<ProductSaleController> _logger;//TODO what is this?
        private readonly ProductSaleService _productSaleService;

        public ProductSaleController(ILogger<ProductSaleController> logger, ProductSaleService productSaleService)
        {
            _logger = logger;
            _productSaleService = productSaleService;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> AddProductSale(AddProductSaleRequest request)
        {
            try
            {
                await _productSaleService.AddProductSale(request);

                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> UpdateProductSale(UpdateProductSaleRequest request)
        {
            try
            {
                await _productSaleService.UpdateProductSale(request);

                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(ProductSaleData), 200)]
        public async Task<ActionResult<ProductSaleData>> GetProductSale(int productSaleId)
        {
            try
            {
                var productSale = await _productSaleService.GetProductSale(productSaleId);

                return Ok(productSale);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<ProductSaleData>), 200)]
        public async Task<ActionResult<PagedModel<ProductSaleData>>> GetAllProductSalesPaged(int pageIndex, int pageSize, DateTime? dateFrom, DateTime? dateTo, int? productId)
        {
            try
            {
                var productSales = await _productSaleService.GetAllProductSalesPaged(pageIndex, pageSize, dateFrom, dateTo, productId);

                return Ok(productSales);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> RemoveProductSale(int productSaleId)
        {
            try
            {
                await _productSaleService.RemoveProductSale(productSaleId);

                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(List<ProductSaleSumsData>), 200)]
        public async Task<ActionResult<List<ProductSaleSumsData>>> GetAllProductSaleSums(DateTime? dateFrom, DateTime? dateTo)
        {
            try
            {
                var productSaleSums = await _productSaleService.GetSums(dateFrom, dateTo);

                return Ok(productSaleSums);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}