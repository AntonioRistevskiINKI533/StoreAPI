using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CitySaleSumsViewController : ControllerBase
    {

        private readonly ILogger<CitySaleSumsViewController> _logger;
        private readonly CitySaleSumsViewService _citySaleSumsViewService;

        public CitySaleSumsViewController(ILogger<CitySaleSumsViewController> logger,
                                          CitySaleSumsViewService citySaleSumsViewService)
        {
            _logger = logger;
            _citySaleSumsViewService = citySaleSumsViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<CitySaleSumsViewData>), 200)]
        public async Task<ActionResult> GetAllCitySales(int pageIndex, int pageSize, string? name = "")
        {
            try
            {
                var citySaleSumsViews = await _citySaleSumsViewService.GetAll(pageIndex, pageSize, name);

                return Ok(citySaleSumsViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<CitySaleSumsViewData>), 200)]
        //public async Task<ActionResult> GetOne(int citySaleSumsViewId)
        //{
        //    var citySaleSumsViews = await _citySaleSumsViewService.GetOne(citySaleSumsViewId);
        //
        //    return Ok(citySaleSumsViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertCitySaleSumsViewRequest request)
        //{
        //    var response = await _citySaleSumsViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _citySaleSumsViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateCitySaleSumsViewRequest request)
        //{
        //    var response = await _citySaleSumsViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}