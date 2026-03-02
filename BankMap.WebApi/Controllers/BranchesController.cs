using BankMap.Application.Common;
using BankMap.Application.Features.Branches.Commands.ImportJson;
using BankMap.Application.Features.Branches.Queries.GetAllBranches;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankMap.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BranchesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //GET api/branches
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            Result<List<BranchDto>> res = await _mediator.Send(new GetAllBranchesQuery(), ct);
            return res.IsSuccess ? Ok(res.Value) : BadRequest(res.Error);
        }

        //POST api/branches/import-json
        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJson(IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("JSON file is required");

            using var stream = file.OpenReadStream();
            var result = await _mediator.Send(new ImportBranchesCommand(stream), ct);

            return result.IsSuccess ? Ok(new { imported = result.Value }) : BadRequest(result.Error);
        }
    }
}
