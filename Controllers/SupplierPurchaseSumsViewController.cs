using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupplierPurchaseSumsViewController : ControllerBase
    {

        private readonly ILogger<SupplierPurchaseSumsViewController> _logger;
        private readonly SupplierPurchaseSumsViewService _supplierPurchaseSumsViewService;

        public SupplierPurchaseSumsViewController(ILogger<SupplierPurchaseSumsViewController> logger,
                                          SupplierPurchaseSumsViewService supplierPurchaseSumsViewService)
        {
            _logger = logger;
            _supplierPurchaseSumsViewService = supplierPurchaseSumsViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<SupplierPurchaseSumsViewData>), 200)]
        public async Task<ActionResult> GetAllSuppliersPurchases(int pageIndex, int pageSize, string? name = null)
        {
            try
            {
                var supplierPurchaseSumsViews = await _supplierPurchaseSumsViewService.GetAll(pageIndex, pageSize, name);

                return Ok(supplierPurchaseSumsViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<SupplierPurchaseSumsViewData>), 200)]
        //public async Task<ActionResult> GetOne(int supplierPurchaseSumsViewId)
        //{
        //    var supplierPurchaseSumsViews = await _supplierPurchaseSumsViewService.GetOne(supplierPurchaseSumsViewId);
        //
        //    return Ok(supplierPurchaseSumsViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertSupplierPurchaseSumsViewRequest request)
        //{
        //    var response = await _supplierPurchaseSumsViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _supplierPurchaseSumsViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateSupplierPurchaseSumsViewRequest request)
        //{
        //    var response = await _supplierPurchaseSumsViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}