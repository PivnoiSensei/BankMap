using BankMap.Application.Features.Branches.Commands.ImportJson;
using BankMap.Application.Features.Branches.Commands.UpdateBranchStatus;
using BankMap.Application.Features.Branches.Queries.GetAllBranches;
using Microsoft.AspNetCore.Mvc;

namespace BankMap.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ApiControllerBase
    {
        //GET api/branches
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => await SendAsync(new GetAllBranchesQuery());

        //PATCH api/branches/${id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBranchStatus(int id, [FromBody] UpdateBranchStatusBody body)
        {
            return await SendAsync(
                new UpdateBranchStatusCommand(id, body.IsTemporaryClosed)
            );
        }
           

        //POST api/branches/import-json
        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJson(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("JSON file is required");

            using var stream = file.OpenReadStream();
            return await SendAsync(new ImportBranchesCommand(stream));
        }

        public record UpdateBranchStatusBody(bool IsTemporaryClosed);
    }
}
