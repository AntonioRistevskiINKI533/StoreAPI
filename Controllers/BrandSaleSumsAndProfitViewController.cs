using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BrandSaleSumsAndProfitViewController : ControllerBase
    {

        private readonly ILogger<BrandSaleSumsAndProfitViewController> _logger;
        private readonly BrandSaleSumsAndProfitViewService _brandSaleSumsAndProfitViewService;

        public BrandSaleSumsAndProfitViewController(ILogger<BrandSaleSumsAndProfitViewController> logger,
                                          BrandSaleSumsAndProfitViewService brandSaleSumsAndProfitViewService)
        {
            _logger = logger;
            _brandSaleSumsAndProfitViewService = brandSaleSumsAndProfitViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<BrandSaleSumsAndProfitViewData>), 200)]
        public async Task<ActionResult> GetAllBrandSales(int pageIndex, int pageSize, string? name = null)
        {
            try
            {
                var brandSaleSumsAndProfitViews = await _brandSaleSumsAndProfitViewService.GetAll(pageIndex, pageSize, name);

                return Ok(brandSaleSumsAndProfitViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<BrandSaleSumsAndProfitViewData>), 200)]
        //public async Task<ActionResult> GetOne(int brandSaleSumsAndProfitViewId)
        //{
        //    var brandSaleSumsAndProfitViews = await _brandSaleSumsAndProfitViewService.GetOne(brandSaleSumsAndProfitViewId);
        //
        //    return Ok(brandSaleSumsAndProfitViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertBrandSaleSumsAndProfitViewRequest request)
        //{
        //    var response = await _brandSaleSumsAndProfitViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _brandSaleSumsAndProfitViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateBrandSaleSumsAndProfitViewRequest request)
        //{
        //    var response = await _brandSaleSumsAndProfitViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}