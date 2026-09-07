using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TimeRecord.DTO.Markings;
using TimeRecord.Services;

namespace TimeRecord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("Management of time markings. All endpoints require a JWT token.")]
    public class TimeRecordsController(TimeRecordsService timeRecordsService) : ControllerBase
    {

        [HttpPost]
        [Authorize]
        [SwaggerOperation(
            Summary = "Registers a time marking.",
            Description = "Creates a time marking for the employee matriculation with the current date and time (UTC).")]
        [ProducesResponseType(typeof(TimeRecordsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateAsync(
            [FromBody]
            [SwaggerRequestBody("Matriculation of the employee that is registering the marking.")]
            TimeRecordsCreateDto createRequestDto)
        {
            var createdMarking = await timeRecordsService.CreateMarkingAsync(createRequestDto);
            return Ok(createdMarking);
        }

  
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists the time markings.",
            Description = "Returns all time markings in the system.")]
        [ProducesResponseType(typeof(IEnumerable<TimeRecordsResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllAsync()
        {
            var markings = await timeRecordsService.GetAllMarkingsAsync();
            return Ok(markings);
        }

     
        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Lists one time marking.",
            Description = "Returns the time marking with the given id.")]
        [ProducesResponseType(typeof(TimeRecordsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(
            [SwaggerParameter("Time marking id.")] int id)
        {
            var marking = await timeRecordsService.GetMarkingsAsync(id);
            return Ok(marking);
        }

     
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Deletes a time marking.",
            Description = "Deletes the time marking and returns a confirmation message.")]
        [ProducesResponseType(typeof(TimeRecordsMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            [SwaggerParameter("Time marking id.")] int id)
        {
            var deletedMarking = await timeRecordsService.DeleteMarkingAsync(id);
            return Ok(deletedMarking);
        }

      
        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Updates a time marking.",
            Description = "Sets the time marking to the current date and time (UTC) and returns it.")]
        [ProducesResponseType(typeof(TimeRecordsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(TimeRecord.Models.ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync(
            [SwaggerParameter("Time marking id.")] int id)
        {
            var updatedMarking = await timeRecordsService.UpdateMarkingAsync(id);
            return Ok(updatedMarking);
        }
    }
}
