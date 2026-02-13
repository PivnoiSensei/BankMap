using Microsoft.EntityFrameworkCore;
using BankMap.Api.Data;
using BankMap.Api.Models;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

//Controllers define API endpoints, use context for SQL queries
namespace BankMap.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET api/branches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankBranch>>> GetBranches()
        {
            return await _context.Branches.ToListAsync();
        }

        //UPDATE Branch data PATCH api/branches/:id
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateBranchStatus(int id, [FromBody] BranchStatusUpdateDto dto)
        {
            var branch = await _context.Branches.FindAsync(id);

            if (branch == null) return NotFound();

            branch.IsTemporaryClosed = dto.IsTemporaryClosed;
            branch.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Import JSON Data
        [HttpPost("import-json")]
        public async Task<IActionResult> ImportJsonFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("JSON file is required");

            try
            {
                using var stream = file.OpenReadStream();

                var root = await JsonSerializer.DeserializeAsync<DepartmentsImportRootDto>(
                    stream,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (root?.List == null || !root.List.Any())
                    return BadRequest("Invalid JSON structure");

                // Полная очистка таблицы
                _context.Branches.RemoveRange(_context.Branches);
                await _context.SaveChangesAsync();

                var branches = root.List.Select(dto => new BankBranch
                {
                    ExternalDepartmentId = dto.DepartmentId,
                    DepartmentType = dto.DepartmentType,
                    Name = dto.DepartmentName,
                    BaseCity = dto.Address?.BaseCity ?? "",
                    FullAddress = dto.FullAddress,
                    Latitude = dto.Address?.GeoLocation?.Lat ?? 0,
                    Longitude = dto.Address?.GeoLocation?.Long ?? 0,
                    IsTemporaryClosed = dto.IsTemporaryClosed,
                    IsRegular = dto.IsRegular,
                    DataJson = JsonSerializer.Serialize(dto),
                    LastUpdated = DateTime.UtcNow
                }).ToList();

                await _context.Branches.AddRangeAsync(branches);
                await _context.SaveChangesAsync();

                return Ok(new { imported = branches.Count });
            }
            catch (JsonException)
            {
                return BadRequest("Invalid JSON format");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}
