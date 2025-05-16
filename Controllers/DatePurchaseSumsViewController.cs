using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatePurchaseSumsViewController : ControllerBase
    {

        private readonly ILogger<DatePurchaseSumsViewController> _logger;
        private readonly DatePurchaseSumsViewService _datePurchaseSumsViewService;

        public DatePurchaseSumsViewController(ILogger<DatePurchaseSumsViewController> logger,
                                          DatePurchaseSumsViewService datePurchaseSumsViewService)
        {
            _logger = logger;
            _datePurchaseSumsViewService = datePurchaseSumsViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<DatePurchaseSumsViewData>), 200)]
        public async Task<ActionResult> GetAllDatePurchases(int pageIndex, int pageSize, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            try
            {
                var datePurchaseSumsViews = await _datePurchaseSumsViewService.GetAll(pageIndex, pageSize, dateFrom, dateTo);

                return Ok(datePurchaseSumsViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<DatePurchaseSumsViewData>), 200)]
        //public async Task<ActionResult> GetOne(int datePurchaseSumsViewId)
        //{
        //    var datePurchaseSumsViews = await _datePurchaseSumsViewService.GetOne(datePurchaseSumsViewId);
        //
        //    return Ok(datePurchaseSumsViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertDatePurchaseSumsViewRequest request)
        //{
        //    var response = await _datePurchaseSumsViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _datePurchaseSumsViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateDatePurchaseSumsViewRequest request)
        //{
        //    var response = await _datePurchaseSumsViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}