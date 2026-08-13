using Microsoft.AspNetCore.Mvc;
using TimeRecord.DTO.Company;
using TimeRecord.Services;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController(CompaniesService companiesService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists the companies in the system.",
            Description = "Returns all companies in the system."
        )]
        public async Task<IActionResult> GetAllAsync()
        {
            var companies = await companiesService.GetUserAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "List one company in the system.",
            Description = "Returns one company in the system.")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var company = await companiesService.GetUserAsync(id);
            return Ok(company);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Create profile company, not user ",
            Description = "return one profile")]
        public async Task<IActionResult> CreateAsync(CompanyCreateDto createRequestDto)
        {
            var companyCreated = await companiesService.CreateCompanyAsync(createRequestDto);
            return Ok(companyCreated);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Update profile company, not user ", 
            Description = "return profile altered")]
        public async Task<IActionResult> UpdateAsync(CompanyCreateDto createRequestDto, int id)
        {
            var updatedCompany = await companiesService.UpdateCompanyAsync(createRequestDto, id);
            return Ok(updatedCompany);
        }

        [HttpDelete("{id}")]
        [Authorize]
        
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deletedCompany = await companiesService.DeleteCompanyAsync(id);
            return Ok(deletedCompany);
        }
    }
}