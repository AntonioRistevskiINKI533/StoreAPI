using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DayOfWeekSaleSumsViewController : ControllerBase
    {

        private readonly ILogger<DayOfWeekSaleSumsViewController> _logger;
        private readonly DayOfWeekSaleSumsViewService _dayOfWeekSaleSumsViewService;

        public DayOfWeekSaleSumsViewController(ILogger<DayOfWeekSaleSumsViewController> logger,
                                          DayOfWeekSaleSumsViewService dayOfWeekSaleSumsViewService)
        {
            _logger = logger;
            _dayOfWeekSaleSumsViewService = dayOfWeekSaleSumsViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<DayOfWeekSaleSumsViewData>), 200)]
        public async Task<ActionResult> GetAllDayOfWeekSales(int pageIndex, int pageSize)
        {
            try
            {
                var dayOfWeekSaleSumsViews = await _dayOfWeekSaleSumsViewService.GetAll(pageIndex, pageSize);

                return Ok(dayOfWeekSaleSumsViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<DayOfWeekSaleSumsViewData>), 200)]
        //public async Task<ActionResult> GetOne(int dayOfWeekSaleSumsViewId)
        //{
        //    var dayOfWeekSaleSumsViews = await _dayOfWeekSaleSumsViewService.GetOne(dayOfWeekSaleSumsViewId);
        //
        //    return Ok(dayOfWeekSaleSumsViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertDayOfWeekSaleSumsViewRequest request)
        //{
        //    var response = await _dayOfWeekSaleSumsViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _dayOfWeekSaleSumsViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> UpdayOfWeek(UpdayOfWeekDayOfWeekSaleSumsViewRequest request)
        //{
        //    var response = await _dayOfWeekSaleSumsViewService.UpdayOfWeek(request);
        //
        //    return Ok(response);
        //}
    }
}