using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DateSaleSumsViewController : ControllerBase
    {

        private readonly ILogger<DateSaleSumsViewController> _logger;
        private readonly DateSaleSumsViewService _dateSaleSumsViewService;

        public DateSaleSumsViewController(ILogger<DateSaleSumsViewController> logger,
                                          DateSaleSumsViewService dateSaleSumsViewService)
        {
            _logger = logger;
            _dateSaleSumsViewService = dateSaleSumsViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<DateSaleSumsViewData>), 200)]
        public async Task<ActionResult> GetAllDateSales(int pageIndex, int pageSize, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            try
            {
                var dateSaleSumsViews = await _dateSaleSumsViewService.GetAll(pageIndex, pageSize, dateFrom, dateTo);

                return Ok(dateSaleSumsViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<DateSaleSumsViewData>), 200)]
        //public async Task<ActionResult> GetOne(int dateSaleSumsViewId)
        //{
        //    var dateSaleSumsViews = await _dateSaleSumsViewService.GetOne(dateSaleSumsViewId);
        //
        //    return Ok(dateSaleSumsViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertDateSaleSumsViewRequest request)
        //{
        //    var response = await _dateSaleSumsViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _dateSaleSumsViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateDateSaleSumsViewRequest request)
        //{
        //    var response = await _dateSaleSumsViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}