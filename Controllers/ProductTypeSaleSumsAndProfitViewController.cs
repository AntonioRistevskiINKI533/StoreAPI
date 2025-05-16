using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductTypeSaleSumsAndProfitViewController : ControllerBase
    {

        private readonly ILogger<ProductTypeSaleSumsAndProfitViewController> _logger;
        private readonly ProductTypeSaleSumsAndProfitViewService _productTypeSaleSumsAndProfitViewService;

        public ProductTypeSaleSumsAndProfitViewController(ILogger<ProductTypeSaleSumsAndProfitViewController> logger,
                                          ProductTypeSaleSumsAndProfitViewService productTypeSaleSumsAndProfitViewService)
        {
            _logger = logger;
            _productTypeSaleSumsAndProfitViewService = productTypeSaleSumsAndProfitViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<ProductTypeSaleSumsAndProfitViewData>), 200)]
        public async Task<ActionResult> GetAllProductTypeSales(int pageIndex, int pageSize, string? name = null)
        {
            try
            {
                var productTypeSaleSumsAndProfitViews = await _productTypeSaleSumsAndProfitViewService.GetAll(pageIndex, pageSize, name);

                return Ok(productTypeSaleSumsAndProfitViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<ProductTypeSaleSumsAndProfitViewData>), 200)]
        //public async Task<ActionResult> GetOne(int productTypeSaleSumsAndProfitViewId)
        //{
        //    var productTypeSaleSumsAndProfitViews = await _productTypeSaleSumsAndProfitViewService.GetOne(productTypeSaleSumsAndProfitViewId);
        //
        //    return Ok(productTypeSaleSumsAndProfitViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertProductTypeSaleSumsAndProfitViewRequest request)
        //{
        //    var response = await _productTypeSaleSumsAndProfitViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _productTypeSaleSumsAndProfitViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateProductTypeSaleSumsAndProfitViewRequest request)
        //{
        //    var response = await _productTypeSaleSumsAndProfitViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}