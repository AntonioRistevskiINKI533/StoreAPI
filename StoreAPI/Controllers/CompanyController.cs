using StoreAPI.Services;
using StoreAPI.Models.Datas;
using Microsoft.AspNetCore.Mvc;
using StoreAPI.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StoreAPI.Exceptions;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {

        private readonly ILogger<CompanyController> _logger;
        private readonly CompanyService _companyService;

        public CompanyController(ILogger<CompanyController> logger, CompanyService companyService)
        {
            _logger = logger;
            _companyService = companyService;
        }

        //TODO should employee be able to delete/update a company?
        [HttpPost("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> AddCompany(AddCompanyRequest request)
        {
            try
            {
                await _companyService.AddCompany(request);

                return Ok();
            }
            catch (ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> UpdateCompany(UpdateCompanyRequest request)
        {
            try
            {
                await _companyService.UpdateCompany(request);

                return Ok();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("[action]")]
        [ProducesResponseType(typeof(CompanyData), 200)]
        public async Task<ActionResult<CompanyData>> GetCompany(int companyId)
        {
            try
            {
                var company = await _companyService.GetCompany(companyId);

                return Ok(company);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(PagedModel<CompanyData>), 200)]
        public async Task<ActionResult<PagedModel<CompanyData>>> GetAllCompaniesPaged(int pageIndex, int pageSize, string? name)
        {
            try
            {
                var companyies = await _companyService.GetAllCompaniesPaged(pageIndex, pageSize, name);

                return Ok(companyies);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("[action]")]
        [ProducesResponseType(typeof(ActionResult), 200)]
        public async Task<ActionResult> RemoveCompany(int companyId)
        {
            try
            {
                await _companyService.RemoveCompany(companyId);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}