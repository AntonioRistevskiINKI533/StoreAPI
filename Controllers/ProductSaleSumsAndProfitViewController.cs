using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductSaleSumsAndProfitViewController : ControllerBase
    {

        private readonly ILogger<ProductSaleSumsAndProfitViewController> _logger;
        private readonly ProductSaleSumsAndProfitViewService _productSaleSumsAndProfitViewService;

        public ProductSaleSumsAndProfitViewController(ILogger<ProductSaleSumsAndProfitViewController> logger,
                                          ProductSaleSumsAndProfitViewService productSaleSumsAndProfitViewService)
        {
            _logger = logger;
            _productSaleSumsAndProfitViewService = productSaleSumsAndProfitViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<ProductSaleSumsAndProfitViewData>), 200)]
        public async Task<ActionResult> GetAllProductSales(int pageIndex, int pageSize, string? name = null)
        {
            try
            {
                var productSaleSumsAndProfitViews = await _productSaleSumsAndProfitViewService.GetAll(pageIndex, pageSize, name);

                return Ok(productSaleSumsAndProfitViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<ProductSaleSumsAndProfitViewData>), 200)]
        //public async Task<ActionResult> GetOne(int productSaleSumsAndProfitViewId)
        //{
        //    var productSaleSumsAndProfitViews = await _productSaleSumsAndProfitViewService.GetOne(productSaleSumsAndProfitViewId);
        //
        //    return Ok(productSaleSumsAndProfitViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertProductSaleSumsAndProfitViewRequest request)
        //{
        //    var response = await _productSaleSumsAndProfitViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _productSaleSumsAndProfitViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateProductSaleSumsAndProfitViewRequest request)
        //{
        //    var response = await _productSaleSumsAndProfitViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}