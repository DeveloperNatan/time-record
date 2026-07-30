using Microsoft.AspNetCore.Mvc;
using TimeRecord.DTO.Company;
using TimeRecord.Services;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController(CompanyService companyService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists the companies in the system.",
            Description = "Returns all companies in the system."
        )]
        public async Task<IActionResult> GetAllAsync()
        {
            var companies = await companyService.GetUserAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(int id)
        {
            var company = await companyService.GetUserAsync(id);
            return Ok(company);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync(CompanyCreateDto createRequestDto)
        {
            var companyCreated = await companyService.CreateCompanyAsync(createRequestDto);
            return Ok(companyCreated);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAsync(CompanyCreateDto createRequestDto, int id)
        {
            var updatedCompany = await companyService.UpdateCompanyAsync(createRequestDto, id);
            return Ok(updatedCompany);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deletedCompany = await companyService.DeleteCompanyAsync(id);
            return Ok(deletedCompany);
        }
    }
}