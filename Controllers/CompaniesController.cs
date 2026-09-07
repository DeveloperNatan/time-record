using Microsoft.AspNetCore.Mvc;
using TimeRecord.DTO.Company;
using TimeRecord.Services;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Management of company profiles. All endpoints require a JWT token.")]
    public class CompaniesController(CompaniesService companiesService) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists the companies in the system.",
            Description = "Returns all companies in the system."
        )]
        [ProducesResponseType(typeof(IEnumerable<CompanyResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAsync()
        {
            var companies = await companiesService.GetUserAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "List one company in the system.",
            Description = "Returns one company by its id.")]
        [ProducesResponseType(typeof(CompanyResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync(
            [SwaggerParameter("Company id.")] int id)
        {
            var company = await companiesService.GetUserAsync(id);
            return Ok(company);
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Create profile company, not user ",
            Description = "Creates a company profile and returns it. The name must be unique.")]
        [ProducesResponseType(typeof(CompanyResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(
            [SwaggerRequestBody("Company data (name and active status).")] CompanyCreateDto createRequestDto)
        {
            var companyCreated = await companiesService.CreateCompanyAsync(createRequestDto);
            return Ok(companyCreated);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Update profile company, not user ",
            Description = "Updates a company profile and returns the altered profile.")]
        [ProducesResponseType(typeof(CompanyResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync(
            [SwaggerRequestBody("New company data.")] CompanyCreateDto createRequestDto,
            [SwaggerParameter("Company id.")] int id)
        {
            var updatedCompany = await companiesService.UpdateCompanyAsync(createRequestDto, id);
            return Ok(updatedCompany);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Delete profile company, not user ",
            Description = "Deletes a company profile and returns a confirmation message.")]
        [ProducesResponseType(typeof(CompanyMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(
            [SwaggerParameter("Company id.")] int id)
        {
            var deletedCompany = await companiesService.DeleteCompanyAsync(id);
            return Ok(deletedCompany);
        }
    }
}
