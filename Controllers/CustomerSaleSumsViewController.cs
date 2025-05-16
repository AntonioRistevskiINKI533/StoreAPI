using StoreAPI.Services;
using StoreAPI.Models;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerSaleSumsViewController : ControllerBase
    {

        private readonly ILogger<CustomerSaleSumsViewController> _logger;
        private readonly CustomerSaleSumsViewService _customerSaleSumsViewService;

        public CustomerSaleSumsViewController(ILogger<CustomerSaleSumsViewController> logger,
                                          CustomerSaleSumsViewService customerSaleSumsViewService)
        {
            _logger = logger;
            _customerSaleSumsViewService = customerSaleSumsViewService;
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<CustomerSaleSumsViewData>), 200)]
        public async Task<ActionResult> GetAllCustomerSales(int pageIndex, int pageSize, string? name = "", string? surname = "")
        {
            try
            {
                var customerSaleSumsViews = await _customerSaleSumsViewService.GetAll(pageIndex, pageSize, name, surname);

                return Ok(customerSaleSumsViews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("[action]")]
        //[ProducesResponseType(typeof(List<CustomerSaleSumsViewData>), 200)]
        //public async Task<ActionResult> GetOne(int customerSaleSumsViewId)
        //{
        //    var customerSaleSumsViews = await _customerSaleSumsViewService.GetOne(customerSaleSumsViewId);
        //
        //    return Ok(customerSaleSumsViews);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Insert(InsertCustomerSaleSumsViewRequest request)
        //{
        //    var response = await _customerSaleSumsViewService.Insert(request);
        //
        //    return Ok(response);
        //}
        //
        //[HttpDelete("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    var response = await _customerSaleSumsViewService.Delete(Id);
        //
        //    return Ok(response);
        //}
        //
        //[HttpPost("[action]")]
        //[ProducesResponseType(typeof(IdResponse), 200)]
        //public async Task<ActionResult> Update(UpdateCustomerSaleSumsViewRequest request)
        //{
        //    var response = await _customerSaleSumsViewService.Update(request);
        //
        //    return Ok(response);
        //}
    }
}