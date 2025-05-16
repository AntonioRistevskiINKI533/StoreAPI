using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductPurchaseSumsViewController : ControllerBase
    {

        private readonly ILogger<ProductPurchaseSumsViewController> _logger;
        private readonly ProductPurchaseSumsViewService _productPurchaseSumsViewService;

        public ProductPurchaseSumsViewController(ILogger<ProductPurchaseSumsViewController> logger,
                                          ProductPurchaseSumsViewService productPurchaseSumsViewService)
        {
            _logger = logger;
            _productPurchaseSumsViewService = productPurchaseSumsViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<ProductPurchaseSumsViewData>), 200)]
        public async Task<ActionResult> GetAllProductPurchases(int pageIndex, int pageSize, string? name = null)
        {
            try
            {
                var productPurchaseSumsViews = await _productPurchaseSumsViewService.GetAll(pageIndex, pageSize, name);

                return Ok(productPurchaseSumsViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<ProductPurchaseSumsViewData>), 200)]
        //public async Task<ActionResult> GetOne(int productPurchaseSumsViewId)
        //{
        //    var productPurchaseSumsViews = await _productPurchaseSumsViewService.GetOne(productPurchaseSumsViewId);
        //
        //    return Ok(productPurchaseSumsViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertProductPurchaseSumsViewRequest request)
        //{
        //    var response = await _productPurchaseSumsViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _productPurchaseSumsViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateProductPurchaseSumsViewRequest request)
        //{
        //    var response = await _productPurchaseSumsViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}