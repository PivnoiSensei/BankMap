using Microsoft.EntityFrameworkCore;
using BankMap.Api.Data;
using BankMap.Api.Models;
using Microsoft.AspNetCore.Mvc;

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

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

        //POST api/branches
        [HttpPost]
        public async Task<ActionResult<BankBranch>> PostBranch(BankBranch branch)
        {
            _context.Branches.Add(branch); 
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBranches), new { id = branch.Id }, branch);
        }

        //UPDATE Branch data PUT api/branches/:id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, BankBranch branch)
        {
            if (id != branch.Id) return BadRequest();

            _context.Entry(branch).State = EntityState.Modified;
            branch.LastUpdated = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Branches.Any(e => e.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        //DELETE Branch 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null) return NotFound();

            _context.Branches.Remove(branch);
            branch.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        //Import CSV Data 
        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file selected");

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    HeaderValidated = null,
                    MissingFieldFound = null
                });

                var records = csv.GetRecords<BankBranch>().ToList();

                foreach (var record in records)
                {
                    record.Id = 0;
                    record.LastUpdated = DateTime.UtcNow;
                }

                _context.Branches.AddRange(records);
                await _context.SaveChangesAsync();

                return Ok(new { count = records.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Importing file: {ex.Message}");
            }
        }
    }
}
